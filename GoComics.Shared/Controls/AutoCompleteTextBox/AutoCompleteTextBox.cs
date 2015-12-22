using System;
using System.Collections;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;

namespace GoComics.Shared.Controls
{
    public enum IconPlacement
    {
        Left,
        Right
    }

    [TemplatePart(Name = AutoCompleteTextBox.PartEditor, Type = typeof(TextBox))]
    [TemplatePart(Name = AutoCompleteTextBox.PartPopup, Type = typeof(Popup))]
    [TemplatePart(Name = AutoCompleteTextBox.PartSelector, Type = typeof(Selector))]
    public class AutoCompleteTextBox : Control
    {
        #region Constants
        public const string PartEditor = "PART_Editor";
        public const string PartPopup = "PART_Popup";
        public const string PartSelector = "PART_Selector";
        #endregion

        #region Fields

        public static readonly DependencyProperty DelayProperty = DependencyProperty.Register("Delay", typeof(int), typeof(AutoCompleteTextBox), new PropertyMetadata(200));
        public static readonly DependencyProperty DisplayMemberProperty = DependencyProperty.Register("DisplayMember", typeof(string), typeof(AutoCompleteTextBox), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty IconPlacementProperty = DependencyProperty.Register("IconPlacement", typeof(IconPlacement), typeof(AutoCompleteTextBox), new PropertyMetadata(IconPlacement.Left));
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(object), typeof(AutoCompleteTextBox), new PropertyMetadata(null));
        public static readonly DependencyProperty IconVisibilityProperty = DependencyProperty.Register("IconVisibility", typeof(Visibility), typeof(AutoCompleteTextBox), new PropertyMetadata(Visibility.Visible));
        public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(AutoCompleteTextBox), new PropertyMetadata(false));
        public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register("IsLoading", typeof(bool), typeof(AutoCompleteTextBox), new PropertyMetadata(false));
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(AutoCompleteTextBox), new PropertyMetadata(false));
        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(AutoCompleteTextBox), new PropertyMetadata(null));
        public static readonly DependencyProperty ItemTemplateSelectorProperty = DependencyProperty.Register("ItemTemplateSelector", typeof(DataTemplateSelector), typeof(AutoCompleteTextBox), new PropertyMetadata(null));
        public static readonly DependencyProperty LoadingContentProperty = DependencyProperty.Register("LoadingContent", typeof(object), typeof(AutoCompleteTextBox), new PropertyMetadata(null));
        public static readonly DependencyProperty ProviderProperty = DependencyProperty.Register("Provider", typeof(ISuggestionProvider), typeof(AutoCompleteTextBox), new PropertyMetadata(null));
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(object), typeof(AutoCompleteTextBox), new PropertyMetadata(null, OnSelectedItemChanged));
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(AutoCompleteTextBox), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.Register("Watermark", typeof(string), typeof(AutoCompleteTextBox), new PropertyMetadata(string.Empty));
        private BindingEvaluator _bindingEvaluator;

        private bool _isUpdatingText;

        private bool _selectionCancelled;

        private SuggestionsAdapter _suggestionsAdapter;

        #endregion

        static AutoCompleteTextBox()
        {
            DefaultStyleKeyProperty.GetMetadata(typeof(AutoCompleteTextBox), new PropertyMetadata(typeof(AutoCompleteTextBox)));
        }

        #region "Properties"

        public BindingEvaluator BindingEvaluator
        {
            get { return _bindingEvaluator; }
            set { _bindingEvaluator = value; }
        }

        public int Delay
        {
            get { return (int)GetValue(DelayProperty); }

            set { SetValue(DelayProperty, value); }
        }

        public string DisplayMember
        {
            get { return (string)GetValue(DisplayMemberProperty); }

            set { SetValue(DisplayMemberProperty, value); }
        }

        public TextBox Editor { get; set; }

        public DispatcherTimer FetchTimer { get; set; }

        public string Filter { get; set; }

        public object Icon
        {
            get { return GetValue(IconProperty); }

            set { SetValue(IconProperty, value); }
        }

        public IconPlacement IconPlacement
        {
            get { return (IconPlacement)GetValue(IconPlacementProperty); }

            set { SetValue(IconPlacementProperty, value); }
        }

        public Visibility IconVisibility
        {
            get { return (Visibility)GetValue(IconVisibilityProperty); }

            set { SetValue(IconVisibilityProperty, value); }
        }

        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }

            set { SetValue(IsDropDownOpenProperty, value); }
        }

        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }

            set { SetValue(IsLoadingProperty, value); }
        }

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }

            set { SetValue(IsReadOnlyProperty, value); }
        }

        public Selector ItemsSelector { get; set; }

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }

            set { SetValue(ItemTemplateProperty, value); }
        }

        public DataTemplateSelector ItemTemplateSelector
        {
            get { return ((DataTemplateSelector)(GetValue(AutoCompleteTextBox.ItemTemplateSelectorProperty))); }
            set { SetValue(AutoCompleteTextBox.ItemTemplateSelectorProperty, value); }
        }

        public object LoadingContent
        {
            get { return GetValue(LoadingContentProperty); }

            set { SetValue(LoadingContentProperty, value); }
        }

        public Popup Popup { get; set; }

        public ISuggestionProvider Provider
        {
            get { return (ISuggestionProvider)GetValue(ProviderProperty); }

            set { SetValue(ProviderProperty, value); }
        }

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }

            set { SetValue(SelectedItemProperty, value); }
        }

        public SelectionAdapter Selection { get; set; }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }

            set { SetValue(TextProperty, value); }
        }

        public string Watermark
        {
            get { return (string)GetValue(WatermarkProperty); }

            set { SetValue(WatermarkProperty, value); }
        }

        #endregion

        #region "Methods"

        public static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoCompleteTextBox act = null;
            act = d as AutoCompleteTextBox;
            if (act != null)
            {
                if (act.Editor != null & !act._isUpdatingText)
                {
                    act._isUpdatingText = true;
                    act.Editor.Text = act.BindingEvaluator.Evaluate(e.NewValue);
                    act._isUpdatingText = false;
                }
            }
        }

        private void ScrollToSelectedItem()
        {
            ListBox listBox = ItemsSelector as ListBox;
            if (listBox != null && listBox.SelectedItem != null)
                listBox.ScrollIntoView(listBox.SelectedItem);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Editor = Template.FindName(PartEditor, this) as TextBox;
            Popup = Template.FindName(PartPopup, this) as Popup;
            ItemsSelector = Template.FindName(PartSelector, this) as Selector;
            BindingEvaluator = new BindingEvaluator(new Binding(DisplayMember));

            if (Editor != null)
            {
                Editor.TextChanged += OnEditorTextChanged;
                Editor.PreviewKeyDown += OnEditorKeyDown;
                Editor.LostFocus += OnEditorLostFocus;

                if (SelectedItem != null)
                {
                    Editor.Text = BindingEvaluator.Evaluate(SelectedItem);
                }

            }

            if (Popup != null)
            {
                Popup.StaysOpen = false;
                Popup.Opened += OnPopupOpened;
                Popup.Closed += OnPopupClosed;
            }
            if (ItemsSelector != null)
            {
                Selection = new SelectionAdapter(ItemsSelector);
                SelectionAdapter.Commit += OnSelectionAdapterCommit;
                SelectionAdapter.Cancel += OnSelectionAdapterCancel;
                SelectionAdapter.SelectionChanged += OnSelectionAdapterSelectionChanged;
            }
        }
        private string GetDisplayText(object dataItem)
        {
            if (BindingEvaluator == null)
            {
                BindingEvaluator = new BindingEvaluator(new Binding(DisplayMember));
            }
            if (dataItem == null)
            {
                return string.Empty;
            }
            if (string.IsNullOrEmpty(DisplayMember))
            {
                return dataItem.ToString();
            }
            return BindingEvaluator.Evaluate(dataItem);
        }

        private void OnEditorKeyDown(object sender, KeyEventArgs e)
        {
            if (SelectionAdapter != null)
            {
                if (IsDropDownOpen)
                    SelectionAdapter.HandleKeyDown(e);
                else
                    IsDropDownOpen = e.Key == Key.Down || e.Key == Key.Up;
            }
        }

        private void OnEditorLostFocus(object sender, RoutedEventArgs e)
        {
            if (!IsKeyboardFocusWithin)
            {
                IsDropDownOpen = false;
            }
        }

        private void OnEditorTextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isUpdatingText)
                return;
            if (FetchTimer == null)
            {
                FetchTimer = new DispatcherTimer();
                FetchTimer.Interval = TimeSpan.FromMilliseconds(Delay);
                FetchTimer.Tick += OnFetchTimerTick;
            }
            FetchTimer.IsEnabled = false;
            FetchTimer.Stop();
            SetSelectedItem(null);
            if (Editor.Text.Length > 0)
            {
                IsLoading = true;
                IsDropDownOpen = true;
                ItemsSelector.ItemsSource = null;
                FetchTimer.IsEnabled = true;
                FetchTimer.Start();
            }
            else
            {
                IsDropDownOpen = false;
            }
        }

        private void OnFetchTimerTick(object sender, EventArgs e)
        {
            FetchTimer.IsEnabled = false;
            FetchTimer.Stop();
            if (Provider != null && ItemsSelector != null)
            {
                Filter = Editor.Text;
                if (_suggestionsAdapter == null)
                {
                    _suggestionsAdapter = new SuggestionsAdapter(this);
                }
                _suggestionsAdapter.GetSuggestions(Filter);
            }
        }

        private void OnPopupClosed(object sender, EventArgs e)
        {
            if (!_selectionCancelled)
            {
                OnSelectionAdapterCommit();
            }
        }

        private void OnPopupOpened(object sender, EventArgs e)
        {
            _selectionCancelled = false;
            ItemsSelector.SelectedItem = SelectedItem;
        }

        private void OnSelectionAdapterCancel()
        {
            _isUpdatingText = true;
            Editor.Text = SelectedItem == null ? Filter : GetDisplayText(SelectedItem);
            Editor.SelectionStart = Editor.Text.Length;
            Editor.SelectionLength = 0;
            _isUpdatingText = false;
            IsDropDownOpen = false;
            _selectionCancelled = true;
        }

        private void OnSelectionAdapterCommit()
        {
            if (ItemsSelector.SelectedItem != null)
            {
                SelectedItem = ItemsSelector.SelectedItem;
                _isUpdatingText = true;
                Editor.Text = GetDisplayText(ItemsSelector.SelectedItem);
                SetSelectedItem(ItemsSelector.SelectedItem);
                _isUpdatingText = false;
                IsDropDownOpen = false;
            }
        }

        private void OnSelectionAdapterSelectionChanged()
        {
            _isUpdatingText = true;
            if (ItemsSelector.SelectedItem == null)
            {
                Editor.Text = Filter;
            }
            else
            {
                Editor.Text = GetDisplayText(ItemsSelector.SelectedItem);
            }
            Editor.SelectionStart = Editor.Text.Length;
            Editor.SelectionLength = 0;
            ScrollToSelectedItem();
            _isUpdatingText = false;
        }

        private void SetSelectedItem(object item)
        {
            _isUpdatingText = true;
            SelectedItem = item;
            _isUpdatingText = false;
        }
        #endregion
    }
}

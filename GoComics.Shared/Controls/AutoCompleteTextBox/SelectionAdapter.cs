using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls.Primitives;

namespace GoComics.Shared.Controls
{
    class SelectionAdapter
    {
        #region "Fields"


        private Selector _selectorControl;
        #endregion

        #region "Constructors"

        public SelectionAdapter(Selector selector)
        {
            SelectorControl = selector;
            SelectorControl.PreviewMouseUp += OnSelectorMouseDown;
        }

        #endregion

        #region "Events"

        public delegate void CancelEventHandler();

        public delegate void CommitEventHandler();

        public delegate void SelectionChangedEventHandler();

        public event CancelEventHandler Cancel;
        public event CommitEventHandler Commit;
        public event SelectionChangedEventHandler SelectionChanged;
        #endregion

        #region "Properties"

        public Selector SelectorControl
        {
            get { return _selectorControl; }
            set { _selectorControl = value; }
        }

        #endregion

        #region "Methods"

        public void HandleKeyDown(KeyEventArgs key)
        {
            Debug.WriteLine(key.VirtualKey);
            switch (key.VirtualKey)
            {
                case VirtualKey.Down:
                    IncrementSelection();
                    break;
                case VirtualKey.Up:
                    DecrementSelection();
                    break;
                case VirtualKey.Enter:
                    if (Commit != null)
                    {
                        Commit();
                    }

                    break;
                case VirtualKey.Escape:
                    if (Cancel != null)
                    {
                        Cancel();
                    }

                    break;
                case VirtualKey.Tab:
                    if (Commit != null)
                    {
                        Commit();
                    }

                    break;
            }
        }

        private void DecrementSelection()
        {
            if (SelectorControl.SelectedIndex == -1)
            {
                SelectorControl.SelectedIndex = SelectorControl.Items.Count - 1;
            }
            else
            {
                SelectorControl.SelectedIndex -= 1;
            }
            SelectionChanged?.Invoke();
        }

        private void IncrementSelection()
        {
            if (SelectorControl.SelectedIndex == SelectorControl.Items.Count - 1)
            {
                SelectorControl.SelectedIndex = -1;
            }
            else
            {
                SelectorControl.SelectedIndex += 1;
            }
            SelectionChanged?.Invoke();
        }

        private void OnSelectorMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Commit != null)
            {
                Commit();
            }
        }

        #endregion
    }
}

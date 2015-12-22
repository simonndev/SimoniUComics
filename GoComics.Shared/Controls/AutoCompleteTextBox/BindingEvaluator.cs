using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace GoComics.Shared.Controls
{
    public class BindingEvaluator : FrameworkElement
    {
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value",
                typeof(string),
                typeof(BindingEvaluator),
                new PropertyMetadata(string.Empty));

        public BindingEvaluator(Binding binding)
        {
            ValueBinding = binding;
        }

        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public Binding ValueBinding { get; set; }

        public string Evaluate(object dataItem)
        {
            this.DataContext = dataItem;
            SetBinding(ValueProperty, ValueBinding);
            return Value;
        }
    }
}
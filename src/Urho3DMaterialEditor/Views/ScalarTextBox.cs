using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Urho3DMaterialEditor.Views
{
    public class ScalarTextBox : TextBox
    {
        private static readonly Brush _bg = new SolidColorBrush(Colors.Black);
        private static readonly Brush _fg = new SolidColorBrush(Colors.White);
        private string _path;

        public ScalarTextBox()
        {
            Background = _bg;
            Foreground = _fg;
            CaretBrush = _fg;
            BorderBrush = _fg;
            VerticalAlignment = VerticalAlignment.Center;
            this.TextChanged += UpdateStep;
        }

        private void UpdateStep(object sender, EventArgs e)
        {
            if (!this.Value.HasValue)
            {
                return;
            }

            var abs = Math.Abs((double)this.Value);
            if (abs <= 0.20)
            {
                this.SmallChange = 0.01M;
            }
            else if (abs <= 2.0)
            {
                this.SmallChange = 0.1M;
            }
            else if (abs <= 20)
            {
                this.SmallChange = 1;
            }
            else
            {
                this.SmallChange = 10;
            }
            this.LargeChange = SmallChange*10;
        }
        public decimal SmallChange { get; private set; }
        public decimal LargeChange { get; private set; }
        public string Path
        {
            get => _path;
            set
            {
                if (_path != value)
                {
                    _path = value;
                    CreateBinding();
                }
            }
        }

        private void CreateBinding()
        {
            BindingOperations.SetBinding(this, TextProperty, new Binding
            {
                Path = new PropertyPath(_path),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                ValidatesOnNotifyDataErrors = true,
                ValidatesOnDataErrors = true,
                NotifyOnValidationError = true,
                ValidationRules =
                {
                    new NumericValidationRule {ValidatesOnTargetUpdated = true, ValidationType = typeof(float)}
                }
                 
            });
        }

        public decimal? Value
        {
            get
            {
                decimal value;
                if (decimal.TryParse(Text, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    return value;
                return null;
            }
            set
            {
                if (!value.HasValue)
                {
                    Text = string.Empty;
                }
                else
                {
                    Text = value.Value.ToString(CultureInfo.InvariantCulture);
                }
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (!this.IsFocused) return;
            this.Value += (e.Delta < 0 ? -SmallChange : SmallChange);
            e.Handled=true;
        }
    }
}
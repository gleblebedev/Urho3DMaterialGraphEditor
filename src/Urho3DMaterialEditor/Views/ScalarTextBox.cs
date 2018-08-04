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
            VerticalAlignment = VerticalAlignment.Center;
            CaretBrush = _fg;
        }

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
        protected override void OnMouseWheel(MouseWheelEventArgs e) {
            var txt = this as TextBox; if (txt == null || !txt.IsFocused) return;
            float fl1 = 0;
            var st = float.TryParse(txt.Text.Replace('.', ','), out fl1);
            if (st) txt.Text = (fl1 + (e.Delta < 0 ? -.1f : .1f)).ToString().Replace(',', '.');

            //this.Text += (e.Delta < 0 ? -.1f : .1f);
            e.Handled=true;
        }
    }
}
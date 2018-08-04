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
            double fl1 = 0;
            string t = txt.Text.Trim();
            var st = double.TryParse(t.Replace('.', ','), out fl1);
            double add = 1;
            var len = t.IndexOf('.');
            if (len > -1) double.TryParse( "0,".PadRight(t.Length -len, '0')+"1",out add) ;

            if (st) txt.Text = (fl1 + (e.Delta < 0 ? -1*add : add)).ToString().Replace(',', '.');

            //this.Text += (e.Delta < 0 ? -.1f : .1f);
            e.Handled=true;
        }
    }
}
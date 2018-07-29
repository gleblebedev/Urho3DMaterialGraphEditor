using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Urho3DMaterialEditor.Views
{
    public class IntTextBox : TextBox
    {
        private static readonly Brush _bg = new SolidColorBrush(Colors.Black);
        private static readonly Brush _fg = new SolidColorBrush(Colors.White);
        private string _path;

        public IntTextBox()
        {
            Background = _bg;
            Foreground = _fg;
            VerticalAlignment = VerticalAlignment.Center;
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
                    new NumericValidationRule {ValidatesOnTargetUpdated = true, ValidationType = typeof(int)}
                }
            });
        }
    }
}
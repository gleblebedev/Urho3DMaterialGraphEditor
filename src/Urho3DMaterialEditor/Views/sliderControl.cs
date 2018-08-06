using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Urho3DMaterialEditor.Views {
    class SliderControl: Slider {
        private string _path;


        public SliderControl() {
            Orientation = Orientation.Horizontal;
            Minimum = 0;
            Maximum = 1;
        }

        public string Path {
            get => _path;
            set {
                if (_path != value) {
                    _path = value;
                    CreateBinding();
                }
            }
        }

        private void CreateBinding() {
            BindingOperations.SetBinding(this, ValueProperty, new Binding {
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
            if (this is Slider sl) {
                sl.Value = Urho.MathHelper.Clamp((float)(sl.Value + (e.Delta > 0 ? .1f : -.1f)), 0, 1);
                e.Handled = true;
            }
        }
       
    }
}

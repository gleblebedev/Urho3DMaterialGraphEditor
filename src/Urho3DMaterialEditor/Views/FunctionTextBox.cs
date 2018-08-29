using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Urho3DMaterialEditor.Views
{
    public class FunctionTextBox : TextBox
    {
        private static readonly Brush _bg = new SolidColorBrush(Colors.Black);
        private static readonly Brush _fg = new SolidColorBrush(Colors.White);
        private string _path;

        public FunctionTextBox()
        {
            Background = _bg;
            Foreground = _fg;
            CaretBrush = _fg;
            BorderBrush = _fg;
            VerticalAlignment = VerticalAlignment.Center;
            this.TextChanged += UpdateStep;


            TextWrapping = TextWrapping.NoWrap;
            AcceptsReturn = true;
            VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;

            Height = 120;
        }

        private void UpdateStep(object sender, EventArgs e)
        {
            //if (this.Value.Length<1)
            //{
            //    return;
            //}

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
                //ValidatesOnNotifyDataErrors = true,
                //ValidatesOnDataErrors = true,
                //NotifyOnValidationError = true,
                //ValidationRules =
                //{
                //    new NumericValidationRule {ValidatesOnTargetUpdated = true, ValidationType = typeof(string)}
                //}
                 
            });
        }

        public string Value;
        //{
        //    get
        //    {
        //            return Value;
        //    }
        //    set
        //    {
        //        if (value.Length<1)
        //        {
        //            Text = string.Empty;
        //        }else {
        //            Text = value;
        //        }

        //    }
        //}

       
    }
}
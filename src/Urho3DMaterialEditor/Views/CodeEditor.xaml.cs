using FastColoredTextBoxNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Urho3DMaterialEditor.Views {
    /// <summary>
    /// Логика взаимодействия для CodeEditor.xaml
    /// </summary>
    // public partial class CodeEditor : UserControl {
    //private string _path;

    //public string Path {
    //    get => _path;
    //    set {
    //        if (_path != value) {
    //            _path = value;
    //            CreateBinding();
    //        }
    //    }
    //}

    //private void CreateBinding() {
    //    BindingOperations.SetBinding(this, addprop, new Binding {
    //        Path = new PropertyPath(_path),
    //        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
    //       // ValidatesOnNotifyDataErrors = true,
    //       // ValidatesOnDataErrors = true,
    //       // NotifyOnValidationError = true,
    //    });
    //}

    //public string Text {
    //    get {
    //        return textBox.Text;
    //    }
    //    set {
    //        textBox.Text = value;
    //    }
    //}

    //public DependencyProperty TextProperty { get; private set; }

    //public static readonly DependencyProperty addprop =
    // DependencyProperty.Register("TxT", typeof(string), typeof(CodeEditor),
    //     new PropertyMetadata(null));

    //public object TxT {
    //    get =>(string) GetValue(addprop);
    //    set => SetValue(addprop, value);
    //}

    //public FastColoredTextBox textBox;
    //WindowsFormsHost host;
    //public CodeEditor() {
    //    InitializeComponent();

    //    host = new WindowsFormsHost();
    //    textBox = new FastColoredTextBox();
    //    host.Child = textBox;

    //    grid.Children.Add(host);
    //    textBox.TextChanged += Ts_TextChanged;

    //    //textBox.ForeColor =System.Drawing.Color.White;


    //    // textBox.Text = "public class Hello {  }";

    //  //  Text = "{Binding EditableValue, UpdateSourceTrigger=PropertyChanged}
    //}

    //private void Ts_TextChanged(object sender, FastColoredTextBoxNS.TextChangedEventArgs e) {

    //}


    // }

    public class CodeTextboxHost : WindowsFormsHost {
        private readonly FastColoredTextBox _innerTextbox = new FastColoredTextBox();

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(CodeTextboxHost), new PropertyMetadata("", new PropertyChangedCallback(
            (d, e) => {
                var textBoxHost = d as CodeTextboxHost;
                if (textBoxHost != null && textBoxHost._innerTextbox != null) {
                    textBoxHost._innerTextbox.Text = textBoxHost.GetValue(e.Property) as string;
                }
            }), null));

        public CodeTextboxHost() {
            Child = _innerTextbox;

            _innerTextbox.TextChanged += _innerTextbox_TextChanged;
        }

        private void _innerTextbox_TextChanged(object sender, FastColoredTextBoxNS.TextChangedEventArgs e) {
            SetValue(TextProperty, _innerTextbox.Text);
        }

        public string Text {
            get { return (string)GetValue(TextProperty); }
            set {
                SetValue(TextProperty, value);
            }
        }
    }
}

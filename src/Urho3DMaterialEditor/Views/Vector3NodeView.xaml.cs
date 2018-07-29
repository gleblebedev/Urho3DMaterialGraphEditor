using System.Windows.Controls;
using System.Windows.Input;

namespace Urho3DMaterialEditor.Views
{
    /// <summary>
    ///     Interaction logic for Vector3NodeView.xaml
    /// </summary>
    public partial class Vector3NodeView : UserControl
    {
        public Vector3NodeView()
        {
            InitializeComponent();
        }

        public void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            float res;
            if (!float.TryParse(e.Text, out res)) e.Handled = true;
        }
    }
}
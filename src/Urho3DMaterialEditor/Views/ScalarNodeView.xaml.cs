using System.Windows.Controls;

namespace Urho3DMaterialEditor.Views
{
    /// <summary>
    ///     Interaction logic for ScalarNodeView.xaml
    /// </summary>
    public partial class ScalarNodeView : UserControl
    {
        public ScalarNodeView()
        {
            InitializeComponent();
        }

        private void ScalarTextBox_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e) {
            var txt = sender as TextBox; if (txt == null) return;
            float fl1 = 0;
            var st = float.TryParse(txt.Text.Replace('.',','),out fl1);
            if (st) txt.Text = (fl1 + (e.Delta<0?-.1f:.1f)).ToString().Replace(',', '.');
            
        }
    }
}
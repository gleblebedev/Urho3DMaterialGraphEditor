using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;

namespace Urho3DMaterialEditor.Views
{
    /// <summary>
    ///     Interaction logic for FunctionNodeView.xaml
    /// </summary>
    public partial class FunctionNodeView : UserControl
    {
        public FunctionNodeView()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, System.Windows.RoutedEventArgs e) {
            var cl = e.Source as MenuItem;
            var dc = DataContext as ViewModels.FunctionViewModel;
            dc.AddPin(cl.Header.ToString() );
            
        }
    }
}
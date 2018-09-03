using System;
using System.ComponentModel;
using System.Windows.Controls;
using Urho3DMaterialEditor.Model;

namespace Urho3DMaterialEditor.Views {
    /// <summary>
    ///     Interaction logic for FunctionNodeView.xaml
    /// </summary>
    public partial class FunctionNodeView : UserControl
    {

      

        public FunctionNodeView()
        {
            InitializeComponent();
            // FillMenuPins();
        }

        private void FillMenuPins() {
            foreach (var item in PinTypes.DataTypes) {
                //LeftMenu.Items.Add(new MenuItem() { Header=item });
            }
            
            
        }

        private void MenuItem_Click(object sender, System.Windows.RoutedEventArgs e) {
            var cl = e.Source as MenuItem;
            var dc = DataContext as ViewModels.FunctionViewModel;
            dc.AddPin(cl.Header.ToString() );
            
        }

        private void MenuItem_Click_1(object sender, System.Windows.RoutedEventArgs e) {
            var cl = e.Source as MenuItem;
            var dc = DataContext as ViewModels.FunctionViewModel;
            dc.OutPin(cl.Header.ToString());
        }

        private void MenuItem_Click_2(object sender, System.Windows.RoutedEventArgs e) {
            var dc = DataContext as ViewModels.FunctionViewModel;
            dc.ClearInPins();
        }
    }
}
using System.Windows;
using Urho3DMaterialEditor.ViewModels;

namespace Urho3DMaterialEditor.Views
{
    /// <summary>
    ///     Interaction logic for AppFoldersDialog.xaml
    /// </summary>
    public partial class AppFoldersDialog : Window
    {
        public AppFoldersDialog()
        {
            InitializeComponent();
        }

        public AppFoldersDialog(AppFoldersViewModel viewModel)
        {
            InitializeComponent();
            viewModel.Reset();
            DataContext = viewModel;
        }

        private void HandleOk(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void HandleCancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
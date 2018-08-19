using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using Urho3DMaterialEditor.Model;
using Urho3DMaterialEditor.ViewModels;
using Application = Urho.Application;
using UnhandledExceptionEventArgs = Urho.UnhandledExceptionEventArgs;

namespace Urho3DMaterialEditor
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DependencyContainer _container;

        private readonly MainViewModel _viewModel;
        private PreviewApplication _app;

        public MainWindow()
        {
            InitializeComponent();
            _container = new DependencyContainer();
#if !DEBUG
            _forDev.Visibility = Visibility.Hidden;
#endif

            Application.UnhandledException += HandleException;
            _viewModel = _container.Resolve<MainViewModel>();
            DataContext = _viewModel;
            Loaded += OnLoaded;
        }

        private async void OnLoaded(object s, EventArgs e)
        {
            await CreateApp();
        }

        private async Task CreateApp()
        {
            var context = _container.Resolve<UrhoContext>();
            var options = context.ApplicationOptions;
            _app = await _urhoSurfaceCtrl.Show<PreviewApplication>(options);
            _app.OnError += (sender, a) => Dispatcher.BeginInvoke((Action) (() => Status.Text = a.Exception.Message));
            _viewModel.Application = _app;
            //Application.InvokeOnMain(() => { /*app.DoSomeStuff();*/});
        }

        private void HandleException(object sender, UnhandledExceptionEventArgs e)
        {
            Dispatcher.Invoke(() => ShowError(e.Exception));
            e.Handled = true;
        }

        private void ShowError(Exception exception)
        {
            MessageBox.Show(exception.Message, "Urho Exception", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e) {
            bool? isT = (sender as ToggleButton).IsChecked;
            if (isT==true) rowSh.Height = new GridLength(rowMn.ActualHeight/2); else rowSh.Height = new GridLength(0);
        }

        private void rowMn_SizeChanged(object sender, SizeChangedEventArgs e) {
            _viewModel.screenWidth = rowMn.ActualWidth-50;
            _viewModel.screenHeght = rowMn.ActualHeight-50;
        }
    }
}
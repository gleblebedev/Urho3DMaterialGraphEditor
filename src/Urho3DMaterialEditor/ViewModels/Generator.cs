using System;
using System.Windows;

namespace Urho3DMaterialEditor.ViewModels
{
    internal class Generator : IObserver<MainViewModel.PreviewData>
    {
        private readonly MainViewModel _mainViewModel;

        public Generator(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }

        public void OnNext(MainViewModel.PreviewData data)
        {
            if (_mainViewModel.Application == null)
                return;
            var material = _mainViewModel.GenerateMaterial("_temp", data.Script, data.PreviewPin);
            _mainViewModel.Application.UpdatePreivew(material);
        }

        public void OnError(Exception error)
        {
            MessageBox.Show(error.ToString(), "Shader generation error", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        public void OnCompleted()
        {
        }
    }
}
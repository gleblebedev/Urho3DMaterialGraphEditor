using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Dialogs;
using Toe.Scripting.WPF;
using Toe.Scripting.WPF.ViewModels;
using Urho3DMaterialEditor.Model;

namespace Urho3DMaterialEditor.ViewModels
{
    public class AppFoldersViewModel : ViewModelBase
    {
        private readonly ConfigurationRepository<AppConfiguration> _configRepository;

        public AppFoldersViewModel(ConfigurationRepository<AppConfiguration> configRepository)
        {
            _configRepository = configRepository;
            Reset();
            AddCommand = new ScriptingCommand(AddFolder);
        }

        public IList<DataFolderViewModel> DataFolders { get; set; }

        public ICommand AddCommand { get; set; }

        private void MoveUp(DataFolderViewModel dataFolderViewModel)
        {
            var index = DataFolders.IndexOf(dataFolderViewModel);
            if (index <= 0)
                return;
            DataFolders.RemoveAt(index);
            DataFolders.Insert(index - 1, dataFolderViewModel);
        }

        private void MoveDown(DataFolderViewModel dataFolderViewModel)
        {
            var index = DataFolders.IndexOf(dataFolderViewModel);
            if (index < 0 || index >= DataFolders.Count - 1)
                return;
            DataFolders.RemoveAt(index);
            DataFolders.Insert(index + 1, dataFolderViewModel);
        }

        private void Delete(DataFolderViewModel dataFolderViewModel)
        {
            var index = DataFolders.IndexOf(dataFolderViewModel);
            if (index < 0)
                return;
            DataFolders.RemoveAt(index);
        }

        public void Reset()
        {
            DataFolders = new ObservableCollection<DataFolderViewModel>(_configRepository.Value.DataFolders.Select(_ =>
                new DataFolderViewModel(this) {Path = _.Path, IsEnabled = _.IsEnabled}));
        }

        public void AddFolder()
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                DataFolders.Insert(0, new DataFolderViewModel(this) {Path = dialog.FileName, IsEnabled = true});
                UpdateArrowButtons();
            }
        }

        private void UpdateArrowButtons()
        {
            for (var i = 0; i < DataFolders.Count; ++i)
            {
                if (i == 0)
                    DataFolders[i].MoveUpCommand.CanExecute = false;
                else
                    DataFolders[i].MoveUpCommand.CanExecute = true;
                if (i == DataFolders.Count - 1)
                    DataFolders[i].MoveDownCommand.CanExecute = false;
                else
                    DataFolders[i].MoveDownCommand.CanExecute = true;
            }
        }

        public void Apply()
        {
            _configRepository.Value.DataFolders = DataFolders
                .Select(_ => new AppConfiguration.DataFolder {Path = _.Path, IsEnabled = _.IsEnabled}).ToList();
            _configRepository.Save();
            MessageBox.Show(
                "Please restart the editor to switch resource path.\nActive resource paths:\n" +
                string.Join(Environment.NewLine, _configRepository.Value.GetActiveDataFolders().Select(_ => _.Path)),
                "Restart required", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        public class DataFolderViewModel : ViewModelBase
        {
            private readonly AppFoldersViewModel _appFolders;

            public DataFolderViewModel(AppFoldersViewModel appFolders)
            {
                _appFolders = appFolders;
                MoveUpCommand = new ScriptingCommand(() => appFolders.MoveUp(this));
                MoveDownCommand = new ScriptingCommand(() => appFolders.MoveDown(this));
                DeleteCommand = new ScriptingCommand(() => appFolders.Delete(this));
            }

            public ScriptingCommand DeleteCommand { get; set; }

            public ScriptingCommand MoveDownCommand { get; set; }

            public ScriptingCommand MoveUpCommand { get; set; }

            public string Path { get; set; }

            public bool IsEnabled { get; set; }
        }
    }
}
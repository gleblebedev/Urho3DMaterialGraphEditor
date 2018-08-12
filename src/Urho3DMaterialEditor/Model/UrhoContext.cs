using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Windows;
using Microsoft.Win32;
using Urho;

namespace Urho3DMaterialEditor.Model
{
    public class UrhoContext
    {
        public static readonly string MaterialGraphs = "MaterialGraphs";

        public List<string> _folders;
        public List<Uri> _folderUris;

        public UrhoContext(ConfigurationRepository<AppConfiguration> config)
        {
            var dataFolders = config.Value.GetActiveDataFolders().ToList();
            if (dataFolders.Count == 0)
                dataFolders.Add(new AppConfiguration.DataFolder
                {
                    Path = Path.Combine(Directory.GetCurrentDirectory(), "Data"),
                    IsEnabled = true
                });
            _folders = dataFolders.Select(_ => EnsureDirEnding(_.Path)).ToList();
            _folderUris = _folders.Select(_ => new Uri(_, UriKind.Absolute)).ToList();

            DataFolder = EnsureDirEnding(dataFolders[0].Path);
            ApplicationOptions = new ApplicationOptions(string.Join(";", dataFolders.Select(_ => _.Path)));
        }

        public ApplicationOptions ApplicationOptions { get; set; }

        public string DataFolder { get; set; }


        private string EnsureDirEnding(string path)
        {
            if (!path.EndsWith(Path.DirectorySeparatorChar.ToString()))
                return path + Path.DirectorySeparatorChar;
            return path;
        }

        public FileName PickFile(string folder, params string[] extensions)
        {
            var startPath = Path.Combine(DataFolder, folder);
            Directory.CreateDirectory(startPath);

            var dialog = new OpenFileDialog();
            dialog.Filter = folder + " (" + string.Join(", ", extensions.Select(_ => "*." + _)) + ") | " +
                            string.Join("; ", extensions.Select(_ => "*." + _));
            dialog.InitialDirectory = startPath;
            var res = dialog.ShowDialog();
            if (res == true)
            {
                string file;
                if (TryGetRelFileName(dialog.FileName, out file))
                    return new FileName {Absolute = dialog.FileName, Relative = file};
                var copyTo = Path.Combine(DataFolder, folder, Path.GetFileName(dialog.FileName));
                var dres = MessageBox.Show(
                    dialog.FileName +
                    " is not in Data folder. Do you want to copy file to the data folder? Target location: " + copyTo,
                    "File is outside of application data folder", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                if (dres == MessageBoxResult.OK)
                {
                    File.Copy(dialog.FileName, copyTo);
                    if (TryGetRelFileName(copyTo, out file))
                        return new FileName {Absolute = copyTo, Relative = file};
                    throw new InvalidOperationException("File " + dialog.FileName + " moved to " + copyTo +
                                                        " but the app can't evaluate a relative path to the file. Please contact developers if this happened.");
                }
            }

            return null;
        }

        public bool TryGetAbsolteFileName(string relativeFileName, out string absoluteFileName)
        {
            absoluteFileName = null;
            foreach (var folder in _folders)
            {
                absoluteFileName = Path.Combine(folder, relativeFileName);
                if (File.Exists(absoluteFileName))
                    return true;
            }

            return false;
        }

        public bool TryGetRelFileName(string absoluteFileName, out string relativeFileName)
        {
            relativeFileName = null;
            foreach (var folder in _folderUris)
            {
                relativeFileName =
                    HttpUtility.UrlDecode(
                        folder.MakeRelativeUri(new Uri(absoluteFileName, UriKind.Absolute)).ToString());
                if (relativeFileName[1] == ':')
                    continue;
                if (relativeFileName.StartsWith("../"))
                    continue;
                return true;
            }

            return false;
        }

        public FileName PickSaveFile(string folder, params string[] extensions)
        {

            var startPath = (string.IsNullOrWhiteSpace(folder))?DataFolder:Path.Combine(DataFolder, folder);
            Directory.CreateDirectory(startPath);

            var dialog = new SaveFileDialog();
            dialog.Filter = folder + " (" + string.Join(", ", extensions.Select(_ => "*." + _)) + ") | " +
                            string.Join("; ", extensions.Select(_ => "*." + _));
            dialog.InitialDirectory = startPath;
            var res = dialog.ShowDialog();
            if (res == true)
            {
                string file;
                if (TryGetRelFileName(dialog.FileName, out file))
                    return new FileName {Absolute = dialog.FileName, Relative = file};
                MessageBox.Show(
                    dialog.FileName +
                    " is not in Data folder. Please move the file to the data folder or add the folder as a data folder via main menu.",
                    "File is outside of application data folder", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return null;
        }

        public Stream CreateFile(string absoluteFileName)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(absoluteFileName));
            return File.Open(Path.Combine(DataFolder, absoluteFileName), FileMode.Create, FileAccess.Write,
                FileShare.Read);
        }

        public void WriteAllText(string absoluteFileName, string text)
        {
            using (var file = CreateFile(absoluteFileName))
            {
                using (var writer = new StreamWriter(file, new UTF8Encoding(false)))
                {
                    writer.Write(text);
                }
            }
        }

        public string ReadAllText(string relFileName)
        {
            using (var file = File.Open(Path.Combine(DataFolder, relFileName), FileMode.Open, FileAccess.Read,
                FileShare.ReadWrite))
            {
                using (var reader = new StreamReader(file, new UTF8Encoding(false)))
                {
                    return reader.ReadToEnd();
                }
            }
        }
        public byte[] ReadAllBytes(string relFileName)
        {
            using (var file = File.Open(Path.Combine(DataFolder, relFileName), FileMode.Open, FileAccess.Read,
                FileShare.ReadWrite))
            {
                var ms = new MemoryStream();
                file.CopyTo(ms);
                return ms.ToArray();
            }
        }
        public class FileName
        {
            public string Absolute { get; set; }
            public string Relative { get; set; }
        }
    }
}
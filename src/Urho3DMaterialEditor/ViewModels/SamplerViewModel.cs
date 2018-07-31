using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using Toe.Scripting;
using Toe.Scripting.WPF;
using Toe.Scripting.WPF.ViewModels;
using Urho3DMaterialEditor.Model;

namespace Urho3DMaterialEditor.ViewModels
{
    public class SamplerViewModel : NodeViewModel
    {
        private readonly UrhoContext _context;
        private ImageSource _imageSource;

        public SamplerViewModel(ScriptViewModel script, ScriptNode node, UrhoContext context) : base(script, node)
        {
            _context = context;
            PickTextureCommand = new ScriptingCommand(PickTexture);
            ResetTextureCommand = new ScriptingCommand(ResetTexture);
            UpdateImageSource();
        }

        public ImageSource ImageSource
        {
            get => _imageSource;
            set => RaiseAndSetIfChanged(ref _imageSource, value);
        }

        public ICommand PickTextureCommand { get; set; }
        public ICommand ResetTextureCommand { get; set; }

        private void ResetTexture()
        {
            Value = null;
            UpdateImageSource();
        }

        private void PickTexture()
        {
            var value = _context.PickFile("Textures", "dds", "png", "tga", "jpg", "xml");
            if (value != null)
            {
                Value = value.Relative;
                UpdateImageSource();
            }
        }

        public static ImageSource GetImageSource(UrhoContext context, string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return null;

            string fullPath = null;
            if (!context.TryGetAbsolteFileName(fileName, out fullPath))
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(fullPath) || !File.Exists(fullPath))
            {
                return null;
            }

            if (Path.GetExtension(fullPath).ToLower() == ".xml")
            {
                var doc = XDocument.Load(fullPath);
                if (doc != null)
                {
                    var cubemap = doc.Element(XName.Get("cubemap"));
                    if (cubemap != null)
                    {
                        foreach (var face in cubemap.Elements(XName.Get("face")))
                        {
                            var name = face.Attribute(XName.Get("name"))?.Value;
                            if (!string.IsNullOrWhiteSpace(name))
                            {
                                var facePath = Path.Combine(Path.GetDirectoryName(fullPath), name);
                                var res = GetImageSource(facePath);
                                if (res != null)
                                    return res;
                            }
                        }
                    }
                }

                return null;
            }

            return GetImageSource(fullPath);
        }

        private static ImageSource GetImageSource(string fullPath)
        {
            if (fullPath.EndsWith(".xml", StringComparison.InvariantCultureIgnoreCase) ||
                fullPath.EndsWith(".tga", StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            try
            {
                return new BitmapImage(new Uri(fullPath, UriKind.Absolute));
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
            return null;
        }

        private void UpdateImageSource()
        {
            ImageSource = GetImageSource(_context, Value);
        }
    }
}
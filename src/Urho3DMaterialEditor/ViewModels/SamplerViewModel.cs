using System;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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

        private void UpdateImageSource()
        {
            if (!string.IsNullOrWhiteSpace(Value))
                if (Path.GetExtension(Value).ToLower() == ".xml")
                {
                    ImageSource = null;
                }
                else
                {
                    if (Value.EndsWith(".xml", StringComparison.InvariantCultureIgnoreCase) ||
                        Value.EndsWith(".tga", StringComparison.InvariantCultureIgnoreCase))
                    {
                        ImageSource = null;
                    }
                    else
                    {
                        string fullPath;
                        if (_context.TryGetAbsolteFileName(Value, out fullPath))
                            if (!File.Exists(fullPath))
                                ImageSource = null;
                            else
                                ImageSource = new BitmapImage(new Uri(fullPath, UriKind.Absolute));
                    }
                }
            else
                ImageSource = null;
        }
    }
}
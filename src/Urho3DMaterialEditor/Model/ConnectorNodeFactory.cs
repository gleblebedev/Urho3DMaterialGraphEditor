using System.Collections.Generic;
using System.IO;
using Toe.Scripting;

namespace Urho3DMaterialEditor.Model
{
    public class ConnectorNodeFactory : AbstractNodeFactory
    {
        private readonly string _pinType;

        public ConnectorNodeFactory(string type, string name, string pinType) : base(type, name,
            new[] {NodeTypes.Categories.Connectors})
        {
            _pinType = pinType;
        }

        public override IEnumerable<string> InputTypes
        {
            get { yield return _pinType; }
        }

        public override IEnumerable<string> OutputTypes
        {
            get { yield return _pinType; }
        }

        public override ScriptNode Build()
        {
            var node = new ScriptNode();
            node.Type = Type;
            node.Name = Name;
            node.Category = NodeCategory.Result;
            node.InputPins.Add(new Pin("", _pinType).AsPinWithConnection());
            node.OutputPins.Add(new Pin("", _pinType));

            return node;
        }
    }

    public interface IPreview
    {
        void UpdatePreivew(PreivewContent content);
    }

    public class PreivewContent
    {
        public string Name { get; set; }
        public string Material { get; set; }
        public string Technique { get; set; }
        public string GLSLShader { get; set; }
        public string HLSLShader { get; set; }

        public UrhoContext UrhoContext { get; set; }

        public void Save()
        {
            var materialFileName = Path.Combine(UrhoContext.DataFolder, "Materials", Name + ".xml");
            var techniqueFileName = Path.Combine(UrhoContext.DataFolder, "Techniques", Name + ".xml");
            var glslFileName = Path.Combine(UrhoContext.DataFolder, "Shaders", "GLSL", Name + ".glsl");
            var hlslFileName = Path.Combine(UrhoContext.DataFolder, "Shaders", "HLSL", Name + ".hlsl");

            UrhoContext.WriteAllText(glslFileName, GLSLShader);
            UrhoContext.WriteAllText(hlslFileName, HLSLShader);
            UrhoContext.WriteAllText(techniqueFileName, Technique);
            UrhoContext.WriteAllText(materialFileName, Material);
        }
    }
}
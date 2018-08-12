using System.Collections.Generic;
using System.IO.Compression;
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
}
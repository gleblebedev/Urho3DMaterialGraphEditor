using System.Collections.Generic;
using Toe.Scripting;

namespace Urho3DMaterialEditor.Model
{
    public class IfDefNodeFactory : AbstractNodeFactory
    {
        public const string Defined = "defined";
        public const string NotDefined = "not defined";

        private readonly string _pinType;

        public IfDefNodeFactory(string type, string name, string pinType) : base(type, name,
            new[] {NodeTypes.Categories.Preprocessor})
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
            node.Category = NodeCategory.Function;
            node.InputPins.Add(new Pin(Defined, _pinType).AsPinWithConnection());
            node.InputPins.Add(new Pin(NotDefined, _pinType).AsPinWithConnection());
            node.OutputPins.Add(new Pin("", _pinType));
            return node;
        }
    }
}
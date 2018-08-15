using System.Collections.Generic;
using Toe.Scripting;

namespace Urho3DMaterialEditor.Model
{
    public class AttributeNodeFactory : AbstractNodeFactory
    {
        private readonly string _pinType;

        public AttributeNodeFactory(string type, string name, string pinType, NodeFactoryVisibility visibility = NodeFactoryVisibility.Visible) : base(type, name,
            new[] {NodeTypes.Categories.Data}, visibility)
        {
            _pinType = pinType == PinTypes.Special.Color ? PinTypes.Vec4 : pinType;
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
            node.Category = NodeCategory.Parameter;
            node.OutputPins.Add(new Pin("", _pinType));
            return node;
        }
    }
}
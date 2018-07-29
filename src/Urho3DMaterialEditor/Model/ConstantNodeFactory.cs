using System.Collections.Generic;
using Toe.Scripting;

namespace Urho3DMaterialEditor.Model
{
    public class ConstantNodeFactory : AbstractNodeFactory
    {
        private readonly NodeCategory _category;
        private readonly string _pinType;

        public ConstantNodeFactory(string pinType, NodeCategory category, string name) : base(pinType, name,
            category.ToString())
        {
            _pinType = pinType == PinTypes.Special.Color ? PinTypes.Vec4 : pinType;
            _category = category;
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
            node.Value = ParameterNodeFactory.GetDefaultValue(_pinType);
            node.Category = _category;
            node.OutputPins.Add(new Pin("", _pinType));
            return node;
        }
    }
}
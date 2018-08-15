using System.Collections.Generic;
using Toe.Scripting;

namespace Urho3DMaterialEditor.Model
{
    public class OutputNodeFactory : AbstractNodeFactory
    {
        private readonly string _pinType;

        public OutputNodeFactory(string type, string name, string pinType, NodeFactoryVisibility visibility = NodeFactoryVisibility.Visible) : base(type, name,
            MaterialNodeRegistry.Categories.Output, visibility)
        {
            _pinType = pinType;
        }

        public override IEnumerable<string> InputTypes
        {
            get { yield return _pinType; }
        }

        public override ScriptNode Build()
        {
            var node = new ScriptNode();
            node.Type = Type;
            node.Name = Name;
            node.Category = NodeCategory.Result;
            node.InputPins.Add(new Pin("value", _pinType).AsPinWithConnection());

            return node;
        }
    }
}
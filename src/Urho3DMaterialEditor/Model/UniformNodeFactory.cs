using System.Collections.Generic;
using System.Windows;
using Toe.Scripting;

namespace Urho3DMaterialEditor.Model
{
    public class UniformNodeFactory : AbstractNodeFactory
    {
        private readonly NodeCategory _category;
        private readonly string _pinType;

        public UniformNodeFactory(string pinType, NodeCategory category, string name, NodeFactoryVisibility visibility = NodeFactoryVisibility.Visible) : base(
            NodeTypes.MakeType(NodeTypes.UniformPrefix, pinType), name, category.ToString(), visibility)
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
            node.Category = _category;
            node.OutputPins.Add(new Pin("", _pinType));
            return node;
        }
    }

    public class BuildInVariableNodeFactory : AbstractNodeFactory
    {
        private readonly NodeCategory _category;
        private readonly string _pinType;

        public BuildInVariableNodeFactory(string pinType, NodeCategory category, string name) : base(
            name, name, category.ToString())
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
            node.Category = _category;
            node.OutputPins.Add(new Pin("", _pinType));
            return node;
        }
    }
}
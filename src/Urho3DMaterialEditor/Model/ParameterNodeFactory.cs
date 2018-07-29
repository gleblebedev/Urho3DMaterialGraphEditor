using System;
using System.Collections.Generic;
using Toe.Scripting;

namespace Urho3DMaterialEditor.Model
{
    public class ParameterNodeFactory : AbstractNodeFactory
    {
        private readonly NodeCategory _category;
        private readonly string _pinType;
        private readonly string _value;

        public ParameterNodeFactory(string pinType, NodeCategory category, string name) : base(
            NodeTypes.MakeType(NodeTypes.ParameterPrefix, pinType), name, category.ToString())
        {
            _value = GetDefaultValue(pinType);
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
            node.Value = _value; //GetDefaultValue(_pinType);
            node.Category = _category;
            node.OutputPins.Add(new Pin("", _pinType));
            return node;
        }

        public static string GetDefaultValue(string pinType)
        {
            switch (pinType)
            {
                case PinTypes.Bool:
                    return "false";
                case PinTypes.Bvec2:
                    return "false false";
                case PinTypes.Bvec3:
                    return "false false false";
                case PinTypes.Bvec4:
                    return "false false false false";
                case PinTypes.Float:
                    return "0.0";
                case PinTypes.Vec2:
                    return "0.0 0.0";
                case PinTypes.Vec3:
                    return "0.0 0.0 0.0";
                case PinTypes.Special.Color:
                    return "1.0 1.0 1.0 1.0";
                case PinTypes.Vec4:
                    return "0.0 0.0 0.0 0.0";
                case PinTypes.Int:
                    return "0";
                case PinTypes.Ivec2:
                    return "0 0";
                case PinTypes.Ivec3:
                    return "0 0 0";
                case PinTypes.Ivec4:
                    return "0 0 0 0";
                case PinTypes.Mat2:
                    return "1.0 0.0 0.0 1.0";
                case PinTypes.Mat3:
                    return "1.0 0.0 0.0 0.0 1.0 0.0 0.0 0.0 1.0";
                case PinTypes.Mat4:
                    return "1.0 0.0 0.0 0.0 0.0 1.0 0.0 0.0 0.0 0.0 1.0 0.0 0.0 0.0 0.0 1.0";
                case PinTypes.Mat4x3:
                    return "0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0 0.0";
            }

            throw new NotImplementedException("Can't make a default value for " + pinType);
        }
    }
}
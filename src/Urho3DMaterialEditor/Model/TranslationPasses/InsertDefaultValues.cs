using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ScriptHelper = Toe.Scripting.Helpers.ScriptHelper<Urho3DMaterialEditor.Model.TranslatedMaterialGraph.NodeInfo>;
using NodeHelper = Toe.Scripting.Helpers.NodeHelper<Urho3DMaterialEditor.Model.TranslatedMaterialGraph.NodeInfo>;
using PinHelper = Toe.Scripting.Helpers.PinHelper<Urho3DMaterialEditor.Model.TranslatedMaterialGraph.NodeInfo>;

namespace Urho3DMaterialEditor.Model.TranslationPasses
{
    public class InsertDefaultValues
    {
        private readonly Dictionary<string, NodeHelper> _defaultValues = new Dictionary<string, NodeHelper>();
        private readonly ScriptHelper _script;

        public InsertDefaultValues(ScriptHelper script)
        {
            _script = script;
        }

        public void Apply()
        {
            foreach (var nodeHelper in _script.Nodes.ToList())
            foreach (var inputPin in nodeHelper.InputPins)
                if (inputPin.Links.Count == 0)
                    if (inputPin.Type == PinTypes.Sampler2D
                        // || inputPin.Type == PinTypes.Sampler3D
                        || inputPin.Type == PinTypes.SamplerCube
                        || inputPin.Type == PinTypes.VertexLights
                        || inputPin.Type == PinTypes.LightMatrices
                    )
                    {
                        foreach (var outputPin in nodeHelper.OutputPins)
                        {
                            var subsNode = GetOrAddDefaultValue(outputPin.Type);
                            _script.CopyConnections(outputPin, subsNode.OutputPins[0]);
                        }

                        _script.Nodes.Remove(nodeHelper);
                        break;
                    }
                    else
                    {
                        _script.LinkData(GetOrAddDefaultValue(inputPin.Type), inputPin);
                    }
        }

        private NodeHelper GetOrAddDefaultValue(string type)
        {
            switch (type)
            {
                case PinTypes.VertexLights:
                case PinTypes.LightMatrices:
                case PinTypes.SamplerCube:
                case PinTypes.Sampler2D:
                    //case PinTypes.Sampler3D:
                    throw new InvalidOperationException("Can't create default value for type " + type);
            }

            NodeHelper node;
            if (!_defaultValues.TryGetValue(type, out node))
            {
                node = new NodeHelper();
                node.Type = NodeTypes.Special.Default;
                node.Name = type;
                node.OutputPins.Add(new PinHelper(type));
                _script.Nodes.Add(node);
            }

            return node;
        }

        public void OptimizeArithmetic()
        {
            foreach (var nodeHelper in _script.Nodes.ToList())
                switch (nodeHelper.Type)
                {
                    case NodeTypes.MultiplyFloatFloat:
                    case NodeTypes.MultiplyVec2Float:
                    case NodeTypes.MultiplyVec3Float:
                    case NodeTypes.MultiplyVec4Float:
                    case NodeTypes.MultiplyVec2Vec2:
                    case NodeTypes.MultiplyVec2Mat2:
                    case NodeTypes.MultiplyVec3Vec3:
                    case NodeTypes.MultiplyVec4Vec4:
                    case NodeTypes.MultiplyVec4Mat4:
                    case NodeTypes.MultiplyVec3Mat3:
                    case NodeTypes.MultiplyMat3Vec3:
                    case NodeTypes.MultiplyMat4x3Float:
                    case NodeTypes.MultiplyMat4Float:
                    case NodeTypes.MultiplyVec4Mat4x3:
                        OptimizeMultiply(nodeHelper);
                        break;
                    case NodeTypes.BreakVec2:
                    case NodeTypes.BreakVec3:
                    case NodeTypes.BreakVec4:
                    case NodeTypes.BreakVec4ToVec3AndFloat:
                    case NodeTypes.BreakVec3ToVec2AndFloat:
                    case NodeTypes.BreakVec4ToVec2AndVec2:
                    case NodeTypes.BreakVec4ToVec2AndFloats:
                        OptimizeMultiply(nodeHelper);
                        break;
                    case NodeTypes.AddFloatFloat:
                    case NodeTypes.AddVec2Vec2:
                    case NodeTypes.AddVec3Vec3:
                    case NodeTypes.AddVec4Vec4:
                    case NodeTypes.AddMat4Mat4:
                    case NodeTypes.AddMat4x3Mat4x3:
                        OptimizeAdd(nodeHelper);
                        break;
            }
        }

        private void OptimizeAdd(NodeHelper nodeHelper)
        {
            //TODO: Optimize +
        }

        private void OptimizeMultiply(NodeHelper nodeHelper)
        {
            foreach (var pin in nodeHelper.InputPins)
            {
                if (pin.Links.Count == 0)
                {
                    ReplaceWithDefault(nodeHelper);
                    return;
                }

                if (pin.Links.Count == 1)
                {
                    var from = pin.Links.First.From;
                    if (from.Node.Type == NodeTypes.Special.Default)
                    {
                        ReplaceWithDefault(nodeHelper);
                        return;
                    }

                    if (NodeTypes.IsConstant(from.Node.Type))
                        if (ReplaceWithDefaultIfZeroConstant(from.Node, nodeHelper))
                            return;
                }
            }
        }

        private bool ReplaceWithDefaultIfZeroConstant(NodeHelper constant, NodeHelper nodeHelper)
        {
            if (constant.Value == null)
            {
                ReplaceWithDefault(nodeHelper);
                return true;
            }

            var components = constant.Value.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var component in components)
            {
                float val;
                if (float.TryParse(component, NumberStyles.Any, CultureInfo.InvariantCulture, out val))
                    if (val != 0.0f)
                        return false;
            }

            ReplaceWithDefault(nodeHelper);
            return true;
        }

        private void ReplaceWithDefault(NodeHelper nodeHelper)
        {
            foreach (var pin in nodeHelper.OutputPins)
            {
                var newNode = GetOrAddDefaultValue(pin.Type);
                foreach (var target in pin.ConnectedPins.ToList()) _script.LinkData(newNode, target);
            }

            _script.Nodes.Remove(nodeHelper);
        }
    }
}
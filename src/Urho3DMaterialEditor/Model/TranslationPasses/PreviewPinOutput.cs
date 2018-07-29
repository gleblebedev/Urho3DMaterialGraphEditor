using System.Linq;
using Toe.Scripting;
using ScriptHelper = Toe.Scripting.Helpers.ScriptHelper<Urho3DMaterialEditor.Model.TranslatedMaterialGraph.NodeInfo>;
using NodeHelper = Toe.Scripting.Helpers.NodeHelper<Urho3DMaterialEditor.Model.TranslatedMaterialGraph.NodeInfo>;
using PinHelper = Toe.Scripting.Helpers.PinHelper<Urho3DMaterialEditor.Model.TranslatedMaterialGraph.NodeInfo>;


namespace Urho3DMaterialEditor.Model.TranslationPasses
{
    public class PreviewPinOutput
    {
        private readonly ScriptHelper _script;

        public PreviewPinOutput(ScriptHelper script)
        {
            _script = script;
        }

        public void Apply(Connection connection)
        {
            if (connection == null) return;

            foreach (var scriptNode in _script.Nodes.ToList())
            {
                switch (scriptNode.Type)
                {
                    case NodeTypes.Discard:
                    case NodeTypes.Opacity:
                    case NodeTypes.Special.FinalColor:
                    case NodeTypes.Special.FinalData0:
                    case NodeTypes.Special.FinalData1:
                    case NodeTypes.Special.FinalData2:
                    case NodeTypes.Special.FinalData3:
                    case NodeTypes.AmbientColor:
                    case NodeTypes.LightColor:
                    case NodeTypes.DeferredOutput:
                        _script.Nodes.Remove(scriptNode);
                        break;
                }

                if (scriptNode.Id == connection.NodeId)
                    foreach (var pin in scriptNode.OutputPins)
                        if (pin.Id == connection.PinId)
                        {
                            var convertorOutput = CreateConvertor(pin);

                            {
                                var finalColor = CreateSinkNode(NodeTypes.Special.FinalColor, PinTypes.Vec4);
                                finalColor.Value = ShaderGeneratorContext.BasePass;
                                _script.LinkData(convertorOutput, finalColor);
                                _script.Nodes.Add(finalColor);
                            }
                            {
                                var finalColor = CreateSinkNode(NodeTypes.Special.FinalColor, PinTypes.Vec4);
                                finalColor.Value = ShaderGeneratorContext.LightBasePass;
                                _script.LinkData(convertorOutput, finalColor);
                                _script.Nodes.Add(finalColor);
                            }
                            {
                                {
                                    var finalColor0 = CreateSinkNode(NodeTypes.Special.FinalData0, PinTypes.Vec4);
                                    finalColor0.Value = ShaderGeneratorContext.PrePass;
                                    _script.Nodes.Add(finalColor0);
                                    var finalColor1 = CreateSinkNode(NodeTypes.Special.FinalData1, PinTypes.Vec4);
                                    finalColor1.Value = ShaderGeneratorContext.PrePass;
                                    _script.Nodes.Add(finalColor1);
                                }
                                {
                                    var finalColor0 = CreateSinkNode(NodeTypes.Special.FinalData0, PinTypes.Vec4);
                                    finalColor0.Value = ShaderGeneratorContext.DeferredPass;
                                    _script.Nodes.Add(finalColor0);
                                    var finalColor1 = CreateSinkNode(NodeTypes.Special.FinalData1, PinTypes.Vec4);
                                    finalColor1.Value = ShaderGeneratorContext.DeferredPass;
                                    _script.Nodes.Add(finalColor1);
                                    var finalColor2 = CreateSinkNode(NodeTypes.Special.FinalData2, PinTypes.Vec4);
                                    finalColor2.Value = ShaderGeneratorContext.DeferredPass;
                                    _script.Nodes.Add(finalColor2);
                                    var finalColor3 = CreateSinkNode(NodeTypes.Special.FinalData3, PinTypes.Vec4);
                                    finalColor3.Value = ShaderGeneratorContext.DeferredPass;
                                    _script.Nodes.Add(finalColor3);

                                    _script.LinkData(convertorOutput, finalColor0);
                                }
                            }
                        }
            }
        }

        private PinHelper CreateConvertor(PinHelper pin)
        {
            if (pin.Type == PinTypes.Vec4) return pin;

            switch (pin.Type)
            {
                case PinTypes.Float:
                    return CreateFloatConvertor(pin);
                case PinTypes.Vec2:
                    return CreateVec2Convertor(pin);
                case PinTypes.Vec3:
                    return CreateVec3Convertor(pin);
            }

            var node = new NodeHelper
            {
                Type = NodeTypes.Special.Default,
                Name = NodeTypes.Special.Default
            };
            node.OutputPins.Add(new PinHelper("", PinTypes.Vec4));
            _script.Nodes.Add(node);
            return node.OutputPins[0];
        }

        private NodeHelper CreateNode(string type)
        {
            var node = new NodeHelper(MaterialNodeRegistry.Instance.ResolveFactory(type).Build());
            _script.Nodes.Add(node);
            return node;
        }

        private PinHelper CreateVec3Convertor(PinHelper pin)
        {
            var node = CreateNode(NodeTypes.MakeVec4FromVec3AndFloat);
            _script.Link(pin, node.InputPins[0]);
            return node.OutputPins[0];
        }

        private PinHelper CreateVec2Convertor(PinHelper pin)
        {
            var node = CreateNode(NodeTypes.MakeVec4FromVec2AndVec2);
            _script.Link(pin, node.InputPins[0]);
            return node.OutputPins[0];
        }

        private PinHelper CreateFloatConvertor(PinHelper pin)
        {
            var node = CreateNode(NodeTypes.MakeVec4);
            _script.Link(pin, node.InputPins[0]);
            _script.Link(pin, node.InputPins[1]);
            _script.Link(pin, node.InputPins[2]);
            _script.Link(pin, node.InputPins[3]);
            return node.OutputPins[0];
        }

        private NodeHelper CreateSinkNode(string type, string pinType)
        {
            var node = new NodeHelper
            {
                Type = type,
                Name = type
            };
            node.InputPins.Add(new PinHelper("", pinType));
            _script.Nodes.Add(node);
            return node;
        }
    }
}
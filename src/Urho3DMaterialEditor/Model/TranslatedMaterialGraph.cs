using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Toe.Scripting;
using Urho;
using Urho3DMaterialEditor.Model.TranslationPasses;
using ScriptHelper = Toe.Scripting.Helpers.ScriptHelper<Urho3DMaterialEditor.Model.TranslatedMaterialGraph.NodeInfo>;
using NodeHelper = Toe.Scripting.Helpers.NodeHelper<Urho3DMaterialEditor.Model.TranslatedMaterialGraph.NodeInfo>;
using PinHelper = Toe.Scripting.Helpers.PinHelper<Urho3DMaterialEditor.Model.TranslatedMaterialGraph.NodeInfo>;

namespace Urho3DMaterialEditor.Model
{
    public class TranslatedMaterialGraph
    {
        private HashSet<string> _defines = new HashSet<string>();
        private HashSet<string> _undefines = new HashSet<string>();


        internal TranslatedMaterialGraph(ScriptHelper graph)
        {
            Script = graph;
        }

        public ScriptHelper Script { get; }

        public IEnumerable<string> Defines => _defines;

        public IEnumerable<string> Undefines => _undefines;

        public NodeHelper Opacity { get; set; }

        public List<NodeHelper> Samplers { get; set; }

        public List<NodeHelper> PixelShaderUniforms { get; set; }

        public List<NodeHelper> VertexShaderAttributes { get; set; }

        public NodeHelper LightColor { get; set; }

        public NodeHelper AmbientColor { get; set; }


        public NodeHelper CameraData { get; set; }

        public NodeHelper RenderData { get; set; }

        public NodeHelper OutputPosition { get; private set; }


        public IList<NodeHelper> VertexShaderUniforms { get; private set; }

        public IList<NodeHelper> Discards { get; private set; }

        public IList<NodeHelper> RenderTargets { get; private set; }

        public IList<NodeHelper> VertexShaderVaryings { get; private set; }

        public IList<NodeHelper> PixelShaderVaryings { get; private set; }

        public static TranslatedMaterialGraph Translate(Script script)
        {
            return Translate(new ScriptHelper(script));
        }

        public static TranslatedMaterialGraph Translate(ScriptHelper script)
        {
            var a = new TranslatedMaterialGraph(script);
            a.Visit();
            return a;
        }

        internal void Visit()
        {
            var elimitateKnownDefines = new ElimitateKnownDefines(Script);
            _defines = elimitateKnownDefines.Defines;
            _undefines = elimitateKnownDefines.Undefines;
            FindOutputPosition();
            elimitateKnownDefines.FindDefines();
            ElimitateConnectors();
            ElimitateTempOutputs();
            elimitateKnownDefines.Apply();
            new InsertDefaultValues(Script).Apply();
            new InsertDefaultValues(Script).OptimizeArithmetic();
            ElimitateDeadEnds();
            new ElimitateDuplicates(Script).Apply();
            new EnsureInfo(Script).Apply();
            new EstimateCalculationCost(Script).Apply();
            new PaintGraph(Script).Apply();
            new InsertVaryings(Script).Apply();
            ElimitateDeadEnds();
            new PaintDefines(Script).Apply();
            //EnsureInfo();
            InsertVariables();
            FillCollections();
        }


        private void ElimitateConnectors()
        {
            foreach (var nodeHelper in Script.Nodes.ToArray())
                if (NodeTypes.IsConnectorType(nodeHelper.Type))
                    ElimitateConnector(nodeHelper);
        }


        private void ElimitateTempOutputs()
        {
            foreach (var nodeHelper in Script.Nodes.ToArray())
                switch (nodeHelper.Type)
                {
                    case NodeTypes.Opacity:
                    case NodeTypes.LightColor:
                    case NodeTypes.DeferredOutput:
                    case NodeTypes.AmbientColor:
                    case NodeTypes.RefractionColor:
                        ElimitateConnector(nodeHelper);
                        break;
                }
        }

        private void ElimitateConnector(NodeHelper nodeHelper)
        {
            if (nodeHelper.InputPins.Count != 1) return;
            if (nodeHelper.OutputPins.Count != 1) return;
            var oldLinks = nodeHelper.InputPins.First().Links.ToList();
            foreach (var linkHelper in nodeHelper.OutputPins.First().Links.ToList())
            foreach (var oldLink in oldLinks)
                Script.Link(oldLink.From, linkHelper.To);
            Script.Nodes.Remove(nodeHelper);
        }

        private void ElimitateDeadEnds()
        {
            foreach (var node in Script.Nodes.ToArray()) ElimitateDeadEnd(node);
        }

        private void ElimitateDeadEnd(NodeHelper node)
        {
            if (node.Script == null)
                return;
            if (node.OutputPins.Count > 0 && !node.OutputPins.Links.Any())
            {
                var deps = node.InputPins.Links.Select(_ => _.From.Node).ToList();
                Script.Nodes.Remove(node);
                foreach (var dep in deps) ElimitateDeadEnd(dep);
            }
        }

        private void InsertVariables()
        {
            var index = 0;
            foreach (var node in Script.Nodes.ToArray())
            {
                if (!NeedsVariable(node))
                    continue;
                var varNode = new NodeHelper {Type = NodeTypes.Special.Variable, Name = "var" + index};
                varNode.Extra = node.Extra;
                varNode.InputPins.Add(new PinHelper("", node.OutputPins[0].Type));
                varNode.OutputPins.Add(new PinHelper("", node.OutputPins[0].Type));
                Script.Nodes.Add(varNode);
                foreach (var link in node.OutputPins[0].Links.Reverse().ToArray())
                {
                    Script.Link(varNode.OutputPins[0], link.To);
                    Script.RemoveLink(link);
                }

                Script.Link(node.OutputPins[0], varNode.InputPins[0]);
                ++index;
            }
        }

        private bool NeedsVariable(NodeHelper node)
        {
            if (node.OutputPins.Count == 0)
                return false;
            if (node.OutputPins[0].Links.Count == 0)
                return false;
            if (NodeTypes.IsUniform(node.Type) || NodeTypes.IsSampler(node.Type) || NodeTypes.IsAttribute(node.Type) ||
                NodeTypes.IsConstant(node.Type))
                return false;
            if (node.Type == NodeTypes.Special.Default)
                return false;
            if (node.OutputPins[0].Links.Count <= 1)
                return false;
            return true;
        }

        private NodeHelper CreateNode(string type)
        {
            var node = new NodeHelper(MaterialNodeRegistry.Instance.ResolveFactory(type).Build());
            Script.Nodes.Add(node);
            return node;
        }

        private NodeHelper CreateConstant(float value)
        {
            var node = new NodeHelper(MaterialNodeRegistry.Instance.ResolveFactory(PinTypes.Float).Build());
            node.Value = string.Format("{0:G9}", value);
            Script.Nodes.Add(node);
            return node;
        }

        private NodeHelper CreateConstant(Vector2 value)
        {
            var node = new NodeHelper(MaterialNodeRegistry.Instance.ResolveFactory(PinTypes.Vec2).Build());
            node.Value = string.Format("{0:G9} {1:G9}", value.X, value.Y);
            Script.Nodes.Add(node);
            return node;
        }

        private NodeHelper CreateConstant(Vector3 value)
        {
            var node = new NodeHelper(MaterialNodeRegistry.Instance.ResolveFactory(PinTypes.Vec3).Build());
            node.Value = string.Format("{0:G9} {1:G9} {2:G9}", value.X, value.Y, value.Z);
            Script.Nodes.Add(node);
            return node;
        }

        private NodeHelper CreateConstant(Vector4 value)
        {
            var node = new NodeHelper(MaterialNodeRegistry.Instance.ResolveFactory(PinTypes.Vec4).Build());
            node.Value = string.Format("{0:G9} {1:G9} {2:G9} {3:G9}", value.X, value.Y, value.Z, value.W);
            Script.Nodes.Add(node);
            return node;
        }

        private NodeHelper CreateSinkNode(string type, NodeHelper node)
        {
            return CreateSinkNode(type, node.OutputPins[0]);
        }

        private NodeHelper CreateSinkNode(string type, PinHelper source)
        {
            var node = CreateSinkNode(type, source.Type);
            Script.Link(source, node.InputPins[0]);
            return node;
        }

        private NodeHelper CreateSinkNode(string type, string pinType)
        {
            var node = new NodeHelper
            {
                Type = type,
                Name = type
            };
            node.InputPins.Add(new PinHelper("", pinType));
            Script.Nodes.Add(node);
            return node;
        }

        private void FillCollections()
        {
            Discards = new List<NodeHelper>();
            RenderTargets = new List<NodeHelper>();
            VertexShaderVaryings = new List<NodeHelper>();
            PixelShaderVaryings = new List<NodeHelper>();
            VertexShaderUniforms = new List<NodeHelper>();
            PixelShaderUniforms = new List<NodeHelper>();
            VertexShaderAttributes = new List<NodeHelper>();
            Samplers = new List<NodeHelper>();

            foreach (var node in Script.Nodes)
            {
                if (NodeTypes.IsAttribute(node.Type))
                {
                    VertexShaderAttributes.Add(node);
                    continue;
                }

                if (NodeTypes.IsParameter(node.Type) || NodeTypes.IsUniform(node.Type))
                {
                    //if (node.Extra.UsedInPixelShader && node.Extra.UsedInVertexShader)
                    //{
                    //    throw new MaterialCompilationException("This node should be split during transformation", node.Id);
                    //}

                    if (node.Extra.RequiredInPixelShader)
                    {
                        PixelShaderUniforms.Add(new NodeHelper
                        {
                            Name = "c" + node.Name,
                            Type = node.OutputPins.First().Type
                        });
                    }
                    else //if (node.Extra.UsedInPixelShader)
                    {
                        VertexShaderUniforms.Add(new NodeHelper
                        {
                            Name = "c" + node.Name,
                            Type = node.OutputPins.First().Type
                        });
                    }

                    continue;
                }

                switch (node.Type)
                {
                    case NodeTypes.Special.ShadowMapOutput:
                    case NodeTypes.Special.FinalColor:
                    case NodeTypes.Special.FragData0:
                    case NodeTypes.Special.FragData1:
                    case NodeTypes.Special.FragData2:
                    case NodeTypes.Special.FragData3:
                        RenderTargets.Add(node);
                        break;
                    case NodeTypes.Discard:
                        Discards.Add(node);
                        break;
                    case NodeTypes.AmbientColor:
                        AmbientColor = node;
                        break;
                    case NodeTypes.Opacity:
                        Opacity = node;
                        break;
                    case NodeTypes.LightColor:
                        LightColor = node;
                        break;
                    case NodeTypes.Special.SetVarying:
                        VertexShaderVaryings.Add(node);
                        break;
                    case NodeTypes.Special.GetVarying:
                        PixelShaderVaryings.Add(node);
                        break;
                    case NodeTypes.LightData:
                    case NodeTypes.CameraData:
                    case NodeTypes.ZoneData:
                    case NodeTypes.ObjectData:
                    {
                        throw new MaterialCompilationException("This node should be split during transformation", node.Id);
                    }
                        //}
                        //if (node.Extra.Attribution == NodeAttribution.PixelShader)
                        //    PixelShaderUniforms.Add(new NodeHelper
                        //    {
                        //        Name = "c" + node.OutputPins.First().Id,
                        //        Type = node.OutputPins.First().Type
                        //    });
                        //else
                        //    VertexShaderUniforms.Add(new NodeHelper
                        //    {
                        //        Name = "c" + node.OutputPins.First().Id,
                        //        Type = node.OutputPins.First().Type
                        //    });
                        //break;
                    case NodeTypes.Sampler2D:
                    case NodeTypes.Sampler3D:
                    case NodeTypes.SamplerCube:
                        Samplers.Add(node);
                        break;
                }
            }
        }

        private NodeHelper GetOrAdd(string type)
        {
            return GetOrAdd(type, () => CreateNode(type));
        }

        private NodeHelper GetOrAdd(string type, Func<NodeHelper> factory)
        {
            var res = Script.Nodes.FirstOrDefault(_ => _.Type == type);
            if (res == null)
            {
                res = factory();
                res.Type = type;
                Script.Nodes.Add(res);
            }

            return res;
        }

        private void SplitOutputs()
        {
            foreach (var scriptNode in Script.Nodes.ToArray())
                if (scriptNode.OutputPins.Count > 1)
                {
                    if (scriptNode.Type == NodeTypes.VertexData)
                        SplitAttributeOutputs(scriptNode);
                    else if (NodeTypes.IsUniform(scriptNode.Type))
                        SplitUniformOutputs(scriptNode);
                    else
                        SplitNodeOutputs(scriptNode);
                    Script.Nodes.Remove(scriptNode);
                }
        }

        private void SplitOutputs(NodeHelper scriptNode, string nodeTypePrefix)
        {
            for (var i = 0; i < scriptNode.OutputPins.Count; ++i)
            {
                var outputPin = scriptNode.OutputPins[i];
                if (outputPin.Links.Count > 0)
                {
                    var node = new NodeHelper();
                    node.Id = scriptNode.Id;
                    node.Name = outputPin.Id;
                    node.Type = NodeTypes.MakeType(nodeTypePrefix, outputPin.Type);
                    node.OutputPins.Add(new PinHelper(outputPin.Type));
                    Script.Nodes.Add(node);
                    Script.CopyConnections(outputPin, node.OutputPins[0]);
                }
            }
        }

        private void SplitUniformOutputs(NodeHelper scriptNode)
        {
            SplitOutputs(scriptNode, NodeTypes.UniformPrefix);
        }

        private void SplitAttributeOutputs(NodeHelper scriptNode)
        {
            SplitOutputs(scriptNode, NodeTypes.AttributePrefix);
        }

        private void SplitNodeOutputs(NodeHelper scriptNode)
        {
            for (var i = 0; i < scriptNode.OutputPins.Count; ++i)
                if (scriptNode.OutputPins[i].Links.Count > 0)
                {
                    var clone = scriptNode.CloneWithConnections();
                    for (var j = 0; j < clone.OutputPins.Count; ++j)
                        if (i != j)
                            clone.OutputPins[j].RemoveLinks();

                    var pin = clone.OutputPins[i];
                    clone.OutputPins.RemoveWhere(_ => !RuntimeHelpers.Equals(_, pin));
                }
        }

        public static void Preprocess(ScriptHelper graph, Connection previewPin = null)
        {
            var g = new TranslatedMaterialGraph(graph);
            new PreviewPinOutput(graph).Apply(previewPin);
            new InlineFunctions(g.Script).Apply();
            g.CreateFinalPosition();
            g.SplitOutputs();
            g.JoinUniforms();
            g.JoinParameters();
            if (previewPin == null)
                g.CreateFinalColors();
            else
                g.CreateShadowPass(graph.Nodes.FirstOrDefault(_ => _.Type == NodeTypes.PositionOutput));
            g.SplitOutputs();
        }

        private void CreateShadowPass(NodeHelper positionOutput)
        {
            if (positionOutput == null)
                return;

            var finalColor = CreateSinkNode(NodeTypes.Special.ShadowMapOutput, PinTypes.Vec2);
            finalColor.Value = ShaderGeneratorContext.ShadowPass;

            var breakPos = CreateNode(NodeTypes.BreakVec4ToVec2AndVec2);
            Script.CopyConnections(positionOutput.InputPins[0], breakPos.InputPins[0]);
            Script.Link(breakPos.OutputPins[1], finalColor.InputPins[0]);
        }

        private void CreateFinalColors()
        {
            NodeHelper ambientColor = null;
            NodeHelper lightColor = null;
            NodeHelper opacity = null;
            NodeHelper refractionColor = null;
            NodeHelper positionOutput = null;
            NodeHelper cameraData = null;

            var finalColors = new List<NodeHelper>(); //TODO: allow final colors to be defined by user
            foreach (var node in Script.Nodes)
                switch (node.Type)
                {
                    case NodeTypes.CameraData:
                        cameraData = node;
                        break;
                    case NodeTypes.PositionOutput:
                        if (positionOutput != null)
                            throw new MaterialCompilationException("Duplicate output", node.Id);
                        positionOutput = node;
                        break;
                    case NodeTypes.AmbientColor:
                        if (ambientColor != null)
                            throw new MaterialCompilationException("Duplicate output", node.Id);
                        ambientColor = node;
                        break;
                    case NodeTypes.LightColor:
                        if (lightColor != null)
                            throw new MaterialCompilationException("Duplicate output", node.Id);
                        lightColor = node;
                        break;
                    case NodeTypes.Opacity:
                        if (opacity != null)
                            throw new MaterialCompilationException("Duplicate output", node.Id);
                        opacity = node;
                        break;
                    case NodeTypes.RefractionColor:
                        if (refractionColor != null)
                            throw new MaterialCompilationException("Duplicate output", node.Id);
                        refractionColor = node;
                        break;
                    case NodeTypes.Special.FinalColor:
                    case NodeTypes.Special.FragData0:
                    case NodeTypes.Special.FragData1:
                    case NodeTypes.Special.FragData2:
                    case NodeTypes.Special.FragData3:
                        var other = finalColors.FirstOrDefault(_ => _.Type == node.Type && _.Value == node.Value);
                        if (other != null)
                            throw new MaterialCompilationException("Duplicate output", node.Id, other.Id);
                        finalColors.Add(node);
                        break;
                }
            cameraData = cameraData ?? CreateNode(NodeTypes.CameraData);
            if (refractionColor != null)
            {
                var finalColor = CreateSinkNode(NodeTypes.Special.FinalColor, PinTypes.Vec4);
                finalColor.Value = ShaderGeneratorContext.RefractionPass;
                Script.CopyConnections(refractionColor.InputPins[0], finalColor.InputPins[0]);
            }
            else
            {
                ambientColor = ambientColor ?? CreateNode(NodeTypes.AmbientColor);
            }

            if (opacity != null)
            {
                ambientColor = ambientColor ?? CreateNode(NodeTypes.AmbientColor);
                {
                    var finalColor = CreateSinkNode(NodeTypes.Special.FinalColor, PinTypes.Vec4);
                    finalColor.Value = ShaderGeneratorContext.AlphaPass;
                    var breakCol = CreateNode(NodeTypes.BreakVec4ToVec3AndFloat);
                    var makeCol = CreateNode(NodeTypes.MakeVec4FromVec3AndFloat);
                    Script.CopyConnections(ambientColor.InputPins[0], breakCol.InputPins[0]);
                    Script.Link(breakCol.OutputPins[0], makeCol.InputPins[0]);
                    Script.CopyConnections(opacity.InputPins[0], makeCol.InputPins[1]);
                    Script.LinkData(ambientColor.InputPins[0], breakCol);
                    Script.LinkData(makeCol, finalColor);
                }
                if (lightColor != null)
                {
                    //var finalColor = CreateSinkNode(NodeTypes.Special.FinalColor, PinTypes.Vec4);
                    //finalColor.Value = ShaderGeneratorContext.AlphaLightPass;
                    //TODO: generate alpha light pass
                }
            }
            else
            {
                if (ambientColor != null)
                {
                    var finalColor = CreateSinkNode(NodeTypes.Special.FinalColor, PinTypes.Vec4);
                    finalColor.Value = ShaderGeneratorContext.BasePass;
                    Script.CopyConnections(ambientColor.InputPins[0], finalColor.InputPins[0]);
                }

                if (lightColor != null)
                {
                    {
                        var finalColor = CreateSinkNode(NodeTypes.Special.FinalColor, PinTypes.Vec4);
                        finalColor.Value = ShaderGeneratorContext.LightPass;
                        Script.CopyConnections(lightColor.InputPins[0], finalColor.InputPins[0]);
                    }
                    if (ambientColor != null)
                    {
                        var finalColor = CreateSinkNode(NodeTypes.Special.FinalColor, PinTypes.Vec4);
                        finalColor.Value = ShaderGeneratorContext.LightBasePass;
                        var sum = CreateNode(NodeTypes.AddVec4Vec4);
                        Script.CopyConnections(ambientColor.InputPins[0], sum.InputPins[0]);
                        Script.CopyConnections(lightColor.InputPins[0], sum.InputPins[1]);
                        Script.LinkData(sum, finalColor);
                    }
                }

                {
                    CreateShadowPass(positionOutput);
                }
            }

            if (ambientColor != null)
                Script.Nodes.Remove(ambientColor);
            if (lightColor != null)
                Script.Nodes.Remove(lightColor);
            if (refractionColor != null)
                Script.Nodes.Remove(refractionColor);
            if (opacity != null)
                Script.Nodes.Remove(opacity);
        }

        private void FindOutputPosition()
        {
            OutputPosition = GetOrAdd(NodeTypes.PositionOutput);
            RenderData = GetOrAdd(NodeTypes.ObjectData);
            CameraData = GetOrAdd(NodeTypes.CameraData);
            if (OutputPosition == null)
                throw new InvalidOperationException(NodeTypes.PositionOutput +
                                                    " node not found. Please run Preprocess method first.");
            if (!OutputPosition.InputPins.Links.Any())
                throw new InvalidOperationException(NodeTypes.PositionOutput +
                                                    " in not connected to data source. Please run Preprocess method first.");
        }

        private void JoinUniforms()
        {
            var uniformsByType = Script.Nodes.Where(_ => NodeTypes.IsUniform(_.Type)).ToLookup(_ => _.Name);
            foreach (var uniformGroup in uniformsByType) JoinUniforms(uniformGroup);
        }

        private void JoinParameters()
        {
            var uniformsByName = Script.Nodes.Where(_ =>
                    NodeTypes.IsParameter(_.Type) || _.Type != NodeTypes.VertexData && NodeTypes.IsAttribute(_.Type))
                .ToLookup(_ => _.Name);
            foreach (var uniformGroup in uniformsByName) JoinUniforms(uniformGroup);
        }

        private void JoinUniforms(IEnumerable<NodeHelper> uniformGroup)
        {
            using (var e = uniformGroup.GetEnumerator())
            {
                if (!e.MoveNext())
                    return;
                var masterNode = e.Current;
                while (e.MoveNext())
                {
                    var currentNode = e.Current;
                    if (masterNode.Type != currentNode.Type)
                        new MaterialCompilationException(
                            "Node type doesn't match the same parameter defined at a different node", currentNode.Id);
                    foreach (var pins in masterNode.OutputPins.Zip(currentNode.OutputPins,
                        (m, c) => new {MasterPin = m, Pin = c}))
                    {
                        if (pins.MasterPin.Id != pins.Pin.Id)
                            new MaterialCompilationException("Pin name doesn't match the same pin at a different node",
                                pins.Pin.Node.Id);
                        while (pins.Pin.Links.Count > 0)
                        {
                            var link = pins.Pin.Links.First;
                            Script.Link(pins.MasterPin, link.To);
                            Script.RemoveLink(link);
                        }
                    }

                    Script.Nodes.Remove(e.Current);
                }
            }
        }

        private void CreateFinalPosition()
        {
            var OutputPosition = GetOrAdd(NodeTypes.PositionOutput);

            if (OutputPosition.InputPins[0].Links.Count == 0)
            {
                var worldPos = CreateNode(NodeTypes.GetWorldPos);
                var clipPos = CreateNode(NodeTypes.GetClipPosVec3);
                Script.LinkData(worldPos, clipPos);
                Script.LinkData(clipPos, OutputPosition);

                new InlineFunctions(Script).Apply();
            }

            HasNoPSDependencies(OutputPosition);
        }

        private void HasNoPSDependencies(NodeHelper node)
        {
            if (PaintGraph.IsPixelShaderOnly(node))
            {
                throw new MaterialCompilationException(node.Name + " can't be used to calculate vertex position", node.Id);
            }

            foreach (var pin in node.InputPins.ConnectedPins)
            {
                HasNoPSDependencies(pin.Node);
            }
        }

        public class NodeInfo
        {
            public bool UsedInPixelShader { get; set; }

            public bool UsedInVertexShader { get; set; }

            public bool RequiredInPixelShader { get; set; }

            public PaintDefines.Container Define { get; set; }

            public bool IsDefineOptimized { get; set; }

            public double? EstimatedCost { get; set; }

            public NodeHelper PixelShaderCopy { get; set; }

            public NodeInfo Clone()
            {
                return new NodeInfo()
                {
                    UsedInPixelShader = UsedInPixelShader,
                    UsedInVertexShader = UsedInVertexShader,
                    RequiredInPixelShader = RequiredInPixelShader,
                    Define = Define?.Clone(),
                    IsDefineOptimized = IsDefineOptimized,
                    EstimatedCost = EstimatedCost,
                    PixelShaderCopy = PixelShaderCopy
                };
            }
        }
    }
}
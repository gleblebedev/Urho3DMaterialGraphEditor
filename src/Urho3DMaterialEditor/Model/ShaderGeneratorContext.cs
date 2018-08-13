using System;
using System.Collections.Generic;
using System.Linq;
using Toe.Scripting;
using Toe.Scripting.Helpers;
using Urho;
using NodeHelper = Toe.Scripting.Helpers.NodeHelper<Urho3DMaterialEditor.Model.TranslatedMaterialGraph.NodeInfo>;

namespace Urho3DMaterialEditor.Model
{
    public class ShaderGeneratorContext
    {
        private readonly CullMode _cull = Urho.CullMode.Ccw;
        private readonly CullMode _shadowCull = Urho.CullMode.Ccw;
        private readonly FillMode _fill = Urho.FillMode.Solid;
        public const string BasePass = "BASEPASS";
        public const string AlphaPass = "ALPHAPASS";
        public const string LightPass = "LIGHTPASS";
        public const string LightBasePass = "LIGHTBASEPASS";
        public const string AlphaLightPass = "ALPHALIGHTPASS";
        public const string ShadowPass = "SHADOWPASS";
        public const string DepthPass = "DEPTHPASS";
        public const string RefractionPass = "REFRACTIONPASS";
        public const string PrePass = "PREPASS";
        public const string DeferredPass = "DEFERREDPASS";

        public ShaderGeneratorContext(ScriptHelper<TranslatedMaterialGraph.NodeInfo> graph, string name,
            Connection previewPin = null)
        {
            Name = name;

            foreach (var nodeHelper in graph.Nodes)
            {
                switch (nodeHelper.Type)
                {
                    case NodeTypes.Cull:
                        ParseEnumValue(nodeHelper.Value, ref _cull);
                        break;
                    case NodeTypes.ShadowCull:
                        ParseEnumValue(nodeHelper.Value, ref _shadowCull);
                        break;
                    case NodeTypes.Fill:
                        ParseEnumValue(nodeHelper.Value, ref _fill);
                        break;
                }
            }
            Parameters = graph.Nodes.Where(_ => NodeTypes.IsParameter(_.Type)).ToList();
            Samplers = graph.Nodes.Where(_ => NodeTypes.IsSampler(_.Type)).ToList();

            TranslatedMaterialGraph.Preprocess(graph, previewPin);

            var finalColorByPass = graph.Nodes.Where(_ => NodeTypes.IsFinalColor(_.Type)).ToLookup(_ => _.Value);
            foreach (var pass in finalColorByPass)
            {
                var passGraph = graph.Clone();
                passGraph.Nodes.RemoveWhere(_ => NodeTypes.IsFinalColor(_.Type) && _.Value != pass.Key);
                Passes.Add(new Pass {Key = pass.Key, Graph = TranslatedMaterialGraph.Translate(passGraph)});
            }
        }

        private void ParseEnumValue<T>(string value, ref T cull)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;
            cull = (T)Enum.Parse(typeof(T), value);
        }

        public IList<Pass> Passes { get; } = new List<Pass>();
        public IList<NodeHelper> Parameters { get; set; }
        public IList<NodeHelper> Samplers { get; set; }
        public string Name { get; }

        public Urho.CullMode Cull => _cull;

        public Urho.CullMode ShadowCull => _shadowCull;

        public Urho.FillMode Fill => _fill;

        private NodeHelper<TranslatedMaterialGraph.NodeInfo> GetOrAdd(
            ScriptHelper<TranslatedMaterialGraph.NodeInfo> graph, string nodeType)
        {
            return graph.Nodes.FindByType(nodeType) ?? BuildNode(graph, nodeType);
        }

        private static NodeHelper<TranslatedMaterialGraph.NodeInfo> BuildSinkNode(
            ScriptHelper<TranslatedMaterialGraph.NodeInfo> script, string type, string pinType)
        {
            var node = new NodeHelper<TranslatedMaterialGraph.NodeInfo>();
            node.Type = type;
            node.Name = type;
            node.InputPins.Add(new PinHelper<TranslatedMaterialGraph.NodeInfo>("value", pinType));
            script.Nodes.Add(node);
            return node;
        }

        private static NodeHelper<TranslatedMaterialGraph.NodeInfo> BuildNode(
            ScriptHelper<TranslatedMaterialGraph.NodeInfo> script, string type)
        {
            var node = new NodeHelper<TranslatedMaterialGraph.NodeInfo>(MaterialNodeRegistry.Instance
                .ResolveFactory(type).Build());
            script.Nodes.Add(node);
            return node;
        }

        public class Pass
        {
            public string Key { get; set; }
            public TranslatedMaterialGraph Graph { get; set; }
        }
    }
}
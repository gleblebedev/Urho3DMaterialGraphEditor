using System.Collections.Generic;
using NUnit.Framework;
using Toe.Scripting;
using Toe.Scripting.Helpers;
using Urho3DMaterialEditor.Model;
using Urho3DMaterialEditor.Model.Templates;

namespace Urho3DMaterialEditor
{
    [TestFixture]
    public class GlslCodeGenTextFixtute
    {
        public static IEnumerable<INodeFactory> AllFactories
        {
            get
            {
                foreach (var factory in MaterialNodeRegistry.Instance) yield return factory;
            }
        }

        [Test]
        [TestCaseSource(nameof(AllFactories))]
        public void TestAllNodes(INodeFactory factory)
        {
            var node = factory.Build();
            foreach (var nodeOutputPin in node.OutputPins)
            {
                var script = new Script();
                script.Add(node);
                var type = nodeOutputPin.Type;
                var connection = new Connection(node.Id, nodeOutputPin.Id);
                switch (type)
                {
                    case PinTypes.LightMatrices:
                        return;
                    case PinTypes.VertexLights:
                        return;
                    case PinTypes.Sampler2D:
                    {
                        var t = MaterialNodeRegistry.Instance.ResolveFactory(NodeTypes.Texture2DSampler2DVec2)
                            .Build();
                        script.Add(t);
                        t.InputPins[0].Connection = new Connection(node.Id, nodeOutputPin.Id);
                        type = t.OutputPins[0].Type;
                        connection = new Connection(t.Id, t.OutputPins[0].Id);
                    }
                        break;
                    case PinTypes.SamplerCube:
                    {
                        var t = MaterialNodeRegistry.Instance.ResolveFactory(NodeTypes.TextureCubeSamplerCubeVec3)
                            .Build();
                        script.Add(t);
                        t.InputPins[0].Connection = new Connection(node.Id, nodeOutputPin.Id);
                        type = t.OutputPins[0].Type;
                        connection = new Connection(t.Id, t.OutputPins[0].Id);
                    }
                        break;
                }

                var amb = new ScriptNode
                {
                    Type = NodeTypes.Special.FinalColor,
                    Value = "PASS",
                    InputPins = {new PinWithConnection {Id = "", Type = type}}
                };
                script.Add(amb);
                amb.InputPins[0].Connection = connection;

                var scriptHelper = new ScriptHelper<TranslatedMaterialGraph.NodeInfo>(script);
                TranslatedMaterialGraph.Preprocess(scriptHelper);
                var g = TranslatedMaterialGraph.Translate(scriptHelper);
                new GLSLPassTemplate(g).TransformText();
            }
        }
    }
}
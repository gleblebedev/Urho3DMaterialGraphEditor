using System.Linq;
using NUnit.Framework;
using Toe.Scripting.Helpers;

namespace Urho3DMaterialEditor.Model.TranslationPasses
{
    [TestFixture]
    public class InsertVaryingsTestFixture : BaseTestFixture
    {
        [Test]
        public void CloneimpleTree()
        {
            var script = new ScriptHelper<TranslatedMaterialGraph.NodeInfo>();

            var posOutput = CreateNode(script, NodeTypes.PositionOutput, PinType, NoPins);
            var colorOutput = CreateNode(script, NodeTypes.Special.FinalColor, PinType, NoPins);
            var prePos = CreateNode(script, "A", PinType, PinType);
            var preColor = CreateNode(script, NodeTypes.Sampler2D, PinType, PinType);
            var common = CreateNode(script, "B", PinType, PinType);
            var preCommon = CreateNode(script, "C", NoPins, PinType);

            script.LinkData(common, prePos);
            script.LinkData(common, preColor);
            script.LinkData(prePos, posOutput);
            script.LinkData(preColor, colorOutput);
            script.LinkData(preCommon, common);
            new PaintGraph(script).Apply();
            new EstimateCalculationCost(script).Apply();

            new InsertVaryings(script).Apply();

            var posChain = EnumerateGraphToLeft(posOutput).ToArray();
            var colorChain = EnumerateGraphToLeft(colorOutput).ToArray();

            Assert.AreEqual(posChain.Length, colorChain.Length);
            for (var index = 0; index < colorChain.Length; index++)
            {
                Assert.AreNotEqual(colorChain[index], posChain[index]);
                Assert.IsTrue(colorChain[index].Node.Extra.UsedInPixelShader);
                Assert.IsFalse(colorChain[index].Node.Extra.UsedInVertexShader);
                Assert.IsTrue(colorChain[index].Node.Extra.RequiredInPixelShader);

                Assert.IsFalse(posChain[index].Node.Extra.UsedInPixelShader);
                Assert.IsFalse(posChain[index].Node.Extra.RequiredInPixelShader);
                Assert.IsTrue(posChain[index].Node.Extra.UsedInVertexShader);
            }
        }
        [Test]
        public void InsertVaryingsSimpleTree()
        {
            var script = new ScriptHelper<TranslatedMaterialGraph.NodeInfo>();

            var posOutput = CreateNode(script, NodeTypes.PositionOutput, PinType, NoPins);
            var colorOutput = CreateNode(script, NodeTypes.Special.FinalColor, PinType, NoPins);
            var prePos = CreateNode(script, "A", PinType, PinType);
            var preColor = CreateNode(script, NodeTypes.Sampler2D, PinType, PinType);
            var common = CreateNode(script, "B", PinType, PinType);
            var preCommon = CreateNode(script, NodeTypes.MakeType(NodeTypes.AttributePrefix,PinType), NoPins, PinType);

            script.LinkData(common, prePos);
            script.LinkData(common, preColor);
            script.LinkData(prePos, posOutput);
            script.LinkData(preColor, colorOutput);
            script.LinkData(preCommon, common);
            new PaintGraph(script).Apply();
            new EstimateCalculationCost(script).Apply();

            new InsertVaryings(script).Apply();

            var posChain = EnumerateGraphToLeft(posOutput).ToArray();
            var colorChain = EnumerateGraphToLeft(colorOutput).ToArray();
            var setVar = script.Nodes.FirstOrDefault(_ => _.Type == NodeTypes.Special.SetVarying);
            var varChain = EnumerateGraphToLeft(setVar).ToArray();

            Assert.AreEqual(posChain.Length, colorChain.Length+ varChain.Length-2);

            for (var index = 0; index < colorChain.Length; index++)
            {
                Assert.AreNotEqual(colorChain[index], posChain[index]);
                Assert.IsTrue(colorChain[index].Node.Extra.UsedInPixelShader);
                Assert.IsFalse(colorChain[index].Node.Extra.UsedInVertexShader);
                Assert.IsTrue(colorChain[index].Node.Extra.RequiredInPixelShader);
            }
            for (var index = 0; index < posChain.Length; index++)
            {
                //Assert.IsFalse(posChain[index].Node.Extra.UsedInPixelShader);
                Assert.IsFalse(posChain[index].Node.Extra.RequiredInPixelShader);
                Assert.IsTrue(posChain[index].Node.Extra.UsedInVertexShader);
            }
            for (var index = 0; index < varChain.Length; index++)
            {
                //Assert.IsFalse(varChain[index].Node.Extra.UsedInPixelShader);
                Assert.IsFalse(varChain[index].Node.Extra.RequiredInPixelShader);
                Assert.IsTrue(varChain[index].Node.Extra.UsedInVertexShader);
            }
        }
    }
}
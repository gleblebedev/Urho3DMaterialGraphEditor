using NUnit.Framework;
using Toe.Scripting.Helpers;
using ScriptHelper = Toe.Scripting.Helpers.ScriptHelper<Urho3DMaterialEditor.Model.TranslatedMaterialGraph.NodeInfo>;
using NodeHelper = Toe.Scripting.Helpers.NodeHelper<Urho3DMaterialEditor.Model.TranslatedMaterialGraph.NodeInfo>;
using PinHelper = Toe.Scripting.Helpers.PinHelper<Urho3DMaterialEditor.Model.TranslatedMaterialGraph.NodeInfo>;

namespace Urho3DMaterialEditor.Model.TranslationPasses
{
    [TestFixture]
    public class PaintGraphTestFixture: BaseTestFixture
    {
        [Test]
        public void PaintSimpleTree()
        {
            var script = new ScriptHelper();

            var posOutput = CreateNode(script, NodeTypes.PositionOutput, PinType, NoPins);
            var colorOutput = CreateNode(script, NodeTypes.Special.FinalColor, PinType, NoPins);
            var prePos = CreateNode(script, "A", PinType, PinType);
            var preColor = CreateNode(script, NodeTypes.Sampler2D, PinType, PinType);
            var common = CreateNode(script, "B", NoPins, PinType);

            script.LinkData(common, prePos);
            script.LinkData(common, preColor);
            script.LinkData(prePos, posOutput);
            script.LinkData(preColor, colorOutput);

            new PaintGraph(script).Apply();

            Assert.IsTrue(common.Extra.UsedInVertexShader);
            Assert.IsTrue(common.Extra.UsedInPixelShader);
            Assert.IsFalse(common.Extra.RequiredInPixelShader);

            Assert.IsFalse(preColor.Extra.UsedInVertexShader);
            Assert.IsTrue(preColor.Extra.UsedInPixelShader);
            Assert.IsTrue(preColor.Extra.RequiredInPixelShader);

            Assert.IsFalse(colorOutput.Extra.UsedInVertexShader);
            Assert.IsTrue(colorOutput.Extra.UsedInPixelShader);
            Assert.IsTrue(colorOutput.Extra.RequiredInPixelShader);

            Assert.IsTrue(prePos.Extra.UsedInVertexShader);
            Assert.IsFalse(prePos.Extra.UsedInPixelShader);
            Assert.IsFalse(prePos.Extra.RequiredInPixelShader);

            Assert.IsTrue(posOutput.Extra.UsedInVertexShader);
            Assert.IsFalse(posOutput.Extra.UsedInPixelShader);
            Assert.IsFalse(posOutput.Extra.RequiredInPixelShader);


        }
    }
}
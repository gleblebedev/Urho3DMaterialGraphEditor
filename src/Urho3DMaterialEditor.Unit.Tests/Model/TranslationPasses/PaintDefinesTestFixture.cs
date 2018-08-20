using NUnit.Framework;
using ScriptHelper = Toe.Scripting.Helpers.ScriptHelper<Urho3DMaterialEditor.Model.TranslatedMaterialGraph.NodeInfo>;
using NodeHelper = Toe.Scripting.Helpers.NodeHelper<Urho3DMaterialEditor.Model.TranslatedMaterialGraph.NodeInfo>;
using PinHelper = Toe.Scripting.Helpers.PinHelper<Urho3DMaterialEditor.Model.TranslatedMaterialGraph.NodeInfo>;

namespace Urho3DMaterialEditor.Model.TranslationPasses
{
    [TestFixture]
    public class PaintDefinesTestFixture: BaseTestFixture
    {
        public NodeHelper CreateIfDef(ScriptHelper script, string key)
        {
            var node = CreateNode(script, NodeTypes.MakeType(NodeTypes.IfDefPrefix, PinType),
                new[] {PinType, PinType}, PinType);
            node.Value = key;
            return node;
        }



        [Test]
        public void IfDef_OneBranchAlwaysDefined_BothBranchesGenerated()
        {
            var script = new ScriptHelper();
            var nodeA = CreateNode(script, "t1", NoPins, PinType);
            var nodeB = CreateNode(script, "t1", NoPins, PinType);
            var ifDef = CreateIfDef(script, "A");
            var sinkNode = CreateNode(script, "float+float", NoPins, PinType);
            sinkNode.InputPins.Add(new PinHelper("x", PinType));
            sinkNode.InputPins.Add(new PinHelper("y", PinType));

            script.LinkData(nodeA, ifDef.InputPins[0]);
            script.LinkData(nodeB, ifDef.InputPins[1]);
            script.LinkData(ifDef, sinkNode.InputPins[0]);
            script.LinkData(nodeA, sinkNode.InputPins[1]);
            new PaintDefines(script).Apply();

            Assert.IsNotNull(ifDef.Extra.Define.IfDefExpression);
            Assert.IsNotNull(ifDef.Extra.Define.IfNotDefExpression);
        }

        [Test]
        public void NestedIfDefs_ThreeValues()
        {
            var script = new ScriptHelper();
            var nodeA = CreateNode(script, "t1", NoPins, PinType);
            var nodeB = CreateNode(script, "t2", NoPins, PinType);
            var nodeC = CreateNode(script, "t2", NoPins, PinType);
            var ifDefA = CreateIfDef(script, "A");
            var ifDefB = CreateIfDef(script, "B");
            var sinkNode = CreateNode(script, "sink", PinType, NoPins);

            script.LinkData(nodeA, ifDefB.InputPins[0]);
            script.LinkData(nodeB, ifDefB.InputPins[1]);
            script.LinkData(ifDefB, ifDefA.InputPins[0]);
            script.LinkData(nodeC, ifDefA.InputPins[1]);
            script.LinkData(ifDefA, sinkNode);
            new PaintDefines(script).Apply();

            Assert.AreEqual(1, nodeA.Extra.Define.IfDefs.Count);
            Assert.AreEqual(1, nodeB.Extra.Define.IfDefs.Count);
            Assert.AreEqual(1, nodeC.Extra.Define.IfDefs.Count);
            Assert.AreEqual("(defined(B) && defined(A))", nodeA.Extra.Define.Expression);
            Assert.AreEqual("(!defined(B) && defined(A))", nodeB.Extra.Define.Expression);
            Assert.AreEqual("!defined(A)", nodeC.Extra.Define.Expression);
            Assert.AreEqual("defined(A)", ifDefB.Extra.Define.Expression);
            Assert.IsNull(ifDefA.Extra.Define.Expression);
            Assert.IsNull(sinkNode.Extra.Define.Expression);
        }

        [Test]
        public void NoIfDefs()
        {
            var script = new ScriptHelper();
            var nodeA = CreateNode(script, "t1", NoPins, PinType);
            var nodeB = CreateNode(script, "t2", PinType, NoPins);

            script.LinkData(nodeA, nodeB);
            new PaintDefines(script).Apply();

            Assert.IsTrue(nodeA.Extra.Define.IsAlways);
            Assert.IsTrue(nodeB.Extra.Define.IsAlways);
        }

        [Test]
        public void SingleIfDef_ResueSamePin()
        {
            var script = new ScriptHelper();
            var nodeA = CreateNode(script, "t1", NoPins, PinType);
            var ifDef = CreateIfDef(script, "A");
            var sinkNode = CreateNode(script, "sink", PinType, NoPins);

            script.LinkData(nodeA, ifDef.InputPins[0]);
            script.LinkData(nodeA, ifDef.InputPins[1]);
            script.LinkData(ifDef, sinkNode);
            new PaintDefines(script).Apply();

            Assert.AreEqual(2, nodeA.Extra.Define.IfDefs.Count);
            Assert.IsNull(nodeA.Extra.Define.Expression);
            Assert.IsNull(ifDef.Extra.Define.Expression);
            Assert.IsNull(sinkNode.Extra.Define.Expression);
        }

        [Test]
        public void SingleIfDef_TwoPins()
        {
            var script = new ScriptHelper();
            var nodeA = CreateNode(script, "t1", NoPins, PinType);
            var nodeB = CreateNode(script, "t2", NoPins, PinType);
            var ifDef = CreateIfDef(script, "A");
            var sinkNode = CreateNode(script, "sink", PinType, NoPins);

            script.LinkData(nodeA, ifDef.InputPins[0]);
            script.LinkData(nodeB, ifDef.InputPins[1]);
            script.LinkData(ifDef, sinkNode);
            new PaintDefines(script).Apply();

            Assert.AreEqual(1, nodeA.Extra.Define.IfDefs.Count);
            Assert.AreEqual(1, nodeB.Extra.Define.IfDefs.Count);
            Assert.AreEqual("defined(A)", nodeA.Extra.Define.Expression);
            Assert.AreEqual("!defined(A)", nodeB.Extra.Define.Expression);
            Assert.IsNull(ifDef.Extra.Define.Expression);
            Assert.IsNull(sinkNode.Extra.Define.Expression);
        }

        [Test]
        public void TestShadows()
        {
            var script = new ScriptHelper();
            var nodeA = CreateNode(script, "sampleShadow", NoPins, PinType);
            var nodeB = CreateNode(script, "1.0", NoPins, PinType);
            var nodeC = CreateNode(script, "float+float", NoPins, PinType);
            nodeC.InputPins.Add(new PinHelper("x", PinType));
            nodeC.InputPins.Add(new PinHelper("y", PinType));
            var nodeD = CreateNode(script, "2.0", NoPins, PinType);
            var ifDefSimpleShadow = CreateIfDef(script, "SIMPLE_SHADOW");
            var ifDefPcfShadow = CreateIfDef(script, "PCF_SHADOW");
            var ifDefVsmShadow = CreateIfDef(script, "VSM_SHADOW");
            var sinkNode = CreateNode(script, "sink", PinType, NoPins);

            script.LinkData(ifDefSimpleShadow, sinkNode);
            script.LinkData(nodeA, ifDefSimpleShadow.InputPins[0]);
            script.LinkData(ifDefPcfShadow, ifDefSimpleShadow.InputPins[1]);
            script.LinkData(nodeC, ifDefPcfShadow.InputPins[0]);
            script.LinkData(ifDefVsmShadow, ifDefPcfShadow.InputPins[1]);
            script.LinkData(nodeA, ifDefVsmShadow.InputPins[0]);
            script.LinkData(nodeB, ifDefVsmShadow.InputPins[1]);
            script.LinkData(nodeA, nodeC.InputPins[0]);
            script.LinkData(nodeD, nodeC.InputPins[1]);
            new PaintDefines(script).Apply();

            Assert.AreEqual(
                "(defined(VSM_SHADOW) || (!defined(VSM_SHADOW) && (defined(SIMPLE_SHADOW) || (!defined(SIMPLE_SHADOW) && defined(PCF_SHADOW)))))",
                nodeA.Extra.Define.Expression);
        }
    }
}
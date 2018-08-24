using System.Linq;
using NUnit.Framework;
using Toe.Scripting.Helpers;

namespace Urho3DMaterialEditor.Model.TranslationPasses
{
    [TestFixture]
    public class InsertVaryingsTestFixture : BaseTestFixture
    {
        [Test]
        public void ShallowTree()
        {
            var script = new ScriptHelper<TranslatedMaterialGraph.NodeInfo>();

            var posOutput = CreateNode(script, NodeTypes.PositionOutput, PinType, NoPins);
            var colorOutput = CreateNode(script, NodeTypes.Special.FinalColor, PinType, NoPins);
            var arg = CreateNode(script, NodeTypes.MakeType(NodeTypes.ParameterPrefix, PinType), PinType, PinType);

            script.LinkData(arg, colorOutput);
            script.LinkData(arg, posOutput);
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
        public void CloneSimpleTree()
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

        [Test]
        public void AmbientColor()
        {
            var script = new ScriptHelper<TranslatedMaterialGraph.NodeInfo>();
            var defVec4 = CreateNode(script, NodeTypes.Special.Default, NoPins, PinTypes.Vec4);
            var one = CreateNode(script, PinTypes.Float, NoPins, PinTypes.Float); one.Value = "1.0";
            var color = CreateNode(script, PinTypes.Vec4, NoPins, PinTypes.Vec4); color.Value = "1.000 0.998 1.000 1.000";
            var cAmbientColor = CreateNode(script, NodeTypes.MakeType(NodeTypes.ParameterPrefix, PinTypes.Vec4), NoPins, PinTypes.Vec4); cAmbientColor.Name = "AmbientColor";
            var cMatEmissiveColor = CreateNode(script, NodeTypes.MakeType(NodeTypes.ParameterPrefix, PinTypes.Vec4), NoPins, PinTypes.Vec4); cAmbientColor.Name = "MatEmissiveColor";
            var colorOutput = CreateNode(script, NodeTypes.Special.FinalColor, PinTypes.Vec4, NoPins);
            var var6 = Multiply(defVec4, color);
            var amb = Multiply(Break(cAmbientColor, NodeTypes.BreakVec4ToVec3AndFloat, "XYZ"), Break(var6, NodeTypes.BreakVec4ToVec3AndFloat, "XYZ"));
            var emis = Break(Add(Multiply(defVec4, defVec4), cMatEmissiveColor), NodeTypes.BreakVec4ToVec3AndFloat, "XYZ");
            var add = Make(new []{Add(amb, emis), Break(var6, NodeTypes.BreakVec4, "W") }, NodeTypes.MakeVec4FromVec3AndFloat, PinTypes.Vec4);
            var breakMake = Make(new[] {Break(add, NodeTypes.BreakVec4ToVec3AndFloat, "XYZ"), one }, NodeTypes.MakeVec4FromVec3AndFloat, PinTypes.Vec4);
            Link(breakMake, colorOutput);

            new InsertDefaultValues(script).Apply();
            new InsertDefaultValues(script).OptimizeArithmetic();
            new EnsureInfo(script).Apply();
            new PaintGraph(script).Apply();
            new EstimateCalculationCost(script).Apply();
            var a = EnumerateGraphToLeft(colorOutput).ToArray();
            new InsertVaryings(script).Apply();
            var colorChain = EnumerateGraphToLeft(colorOutput).ToArray();

            Assert.AreEqual(a.Length, colorChain.Length);
        }
    }
}
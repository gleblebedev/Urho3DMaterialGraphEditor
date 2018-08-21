using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Toe.Scripting.Helpers;
using ScriptHelper = Toe.Scripting.Helpers.ScriptHelper<Urho3DMaterialEditor.Model.TranslatedMaterialGraph.NodeInfo>;
using NodeHelper = Toe.Scripting.Helpers.NodeHelper<Urho3DMaterialEditor.Model.TranslatedMaterialGraph.NodeInfo>;
using PinHelper = Toe.Scripting.Helpers.PinHelper<Urho3DMaterialEditor.Model.TranslatedMaterialGraph.NodeInfo>;
using LinkHelper = Toe.Scripting.Helpers.LinkHelper<Urho3DMaterialEditor.Model.TranslatedMaterialGraph.NodeInfo>;

namespace Urho3DMaterialEditor.Model.TranslationPasses
{
    public class BaseTestFixture
    {
        public static readonly string PinType = PinTypes.Float;
        public static readonly string[] NoPins = new string[0];

        public NodeHelper CreateNode(ScriptHelper script, string type, string input, string[] outputs)
        {
            return CreateNode(script, type, new[] { input }, outputs);
        }
        public NodeHelper Multiply(NodeHelper a, NodeHelper b)
        {
            return BinaryOperator(a,b,"*");
        }
        public NodeHelper Add(NodeHelper a, NodeHelper b)
        {
            return BinaryOperator(a, b, "+");
        }
        public NodeHelper BinaryOperator(NodeHelper a, NodeHelper b, string op)
        {
            Assert.AreEqual(1, a.OutputPins.Count);
            Assert.AreEqual(1, b.OutputPins.Count);
            Assert.AreEqual(a.OutputPins[0].Type, b.OutputPins[0].Type);
            var stringType = a.OutputPins[0].Type + op + b.OutputPins[0].Type;
            var m =  CreateNode(a.Script, stringType, new[] { a.OutputPins[0].Type, b.OutputPins[0].Type }, a.OutputPins[0].Type);
            Link(a, m.InputPins[0]);
            Link(b, m.InputPins[1]);
            return m;
        }
        public NodeHelper Make(NodeHelper[] a, string type, string outputType)
        {
            var m = CreateNode(a[0].Script, type, a.Select(_=>_.OutputPins[0].Type).ToArray() , outputType);
            for (var index = 0; index < a.Length; index++)
            {
                Link(a[index].OutputPins[0], m.InputPins[index]);
            }
            return m;
        }
        public NodeHelper Break(NodeHelper a, string type, string part)
        {
            Assert.AreEqual(1, a.OutputPins.Count);
            var outputType = "";
            switch (part)
            {
                case "XYZ":
                    outputType = PinTypes.Vec3;
                    break;
                case "X":
                case "Y":
                case "Z":
                case "W":
                    outputType = PinTypes.Float;
                    break;
                default:
                    throw new NotImplementedException();
            }
            var m = CreateNode(a.Script, type, a.OutputPins[0].Type, outputType);
            Link(a, m.InputPins[0]);
            return m;
        }
        public LinkHelper Link(NodeHelper a, NodeHelper b)
        {
            Assert.AreEqual(1, a.OutputPins.Count);
            Assert.AreEqual(1, b.InputPins.Count);
            return Link(a.OutputPins[0], b.InputPins[0]);
        }
        public LinkHelper Link(NodeHelper a, PinHelper b)
        {
            Assert.AreEqual(1, a.OutputPins.Count);
            return a.Script.LinkData(a, b);
        }
        public LinkHelper Link(PinHelper a, PinHelper b)
        {
            Assert.AreEqual(a.Type, b.Type);
            return a.Node.Script.Link(a, b);
        }
        public NodeHelper CreateNode(ScriptHelper script, string type, string[] inputs, string output)
        {
            return CreateNode(script, type, inputs, new[] { output });
        }
        public NodeHelper CreateNode(ScriptHelper script, string type, string input, string output)
        {
            return CreateNode(script, type, new[] { input}, new[] { output });
        }
        public NodeHelper CreateNode(ScriptHelper script, string type, string[] inputs, string[] outputs)
        {
            var node = new NodeHelper { Type = type, Name = type };
            node.Extra = new TranslatedMaterialGraph.NodeInfo();
            script.Add(node);
            for (var index = 0; index < inputs.Length; index++)
                node.InputPins.Add(new PinHelper(((char)('A' + index)).ToString(), inputs[index]));
            for (var index = 0; index < outputs.Length; index++)
                node.OutputPins.Add(new PinHelper(((char)('A' + index)).ToString(), outputs[index]));

            return node;
        }

        public IEnumerable<NodeAndDepth> EnumerateGraphToLeft(NodeHelper node)
        {
            HashSet<NodeHelper> visited = new HashSet<NodeHelper>();
            var queue = new Queue<NodeAndDepth>();
            queue.Enqueue(new NodeAndDepth(node,0));
            visited.Add(node);
            while (queue.Count > 0)
            {
                var item = queue.Dequeue();
                yield return item;

                foreach (var pin in item.Node.InputPins.ConnectedPins)
                {
                    if (visited.Add(pin.Node))
                    {
                        queue.Enqueue(new NodeAndDepth(pin.Node, item.Depth+1));
                    }
                }
            }
        }

        public class NodeAndDepth
        {
            public NodeAndDepth(NodeHelper node, int depth)
            {
                Node = node;
                Depth = depth;
            }

            public int Depth { get; }
            public NodeHelper Node { get; }

            public override string ToString()
            {
                return string.Format("{0}: {1}", Depth, Node);
            }
        }
    }
}
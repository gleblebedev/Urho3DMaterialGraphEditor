using System.Collections.Generic;
using ScriptHelper = Toe.Scripting.Helpers.ScriptHelper<Urho3DMaterialEditor.Model.TranslatedMaterialGraph.NodeInfo>;
using NodeHelper = Toe.Scripting.Helpers.NodeHelper<Urho3DMaterialEditor.Model.TranslatedMaterialGraph.NodeInfo>;
using PinHelper = Toe.Scripting.Helpers.PinHelper<Urho3DMaterialEditor.Model.TranslatedMaterialGraph.NodeInfo>;

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
        }
    }
}
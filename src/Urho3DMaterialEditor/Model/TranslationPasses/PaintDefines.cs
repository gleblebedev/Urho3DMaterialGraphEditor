using System;
using System.Collections.Generic;
using System.Linq;
using Toe.Scripting.Defines;
using Urho3DMaterialEditor.Model.Templates;
using ScriptHelper = Toe.Scripting.Helpers.ScriptHelper<Urho3DMaterialEditor.Model.TranslatedMaterialGraph.NodeInfo>;
using NodeHelper = Toe.Scripting.Helpers.NodeHelper<Urho3DMaterialEditor.Model.TranslatedMaterialGraph.NodeInfo>;
using PinHelper = Toe.Scripting.Helpers.PinHelper<Urho3DMaterialEditor.Model.TranslatedMaterialGraph.NodeInfo>;

namespace Urho3DMaterialEditor.Model.TranslationPasses
{
    public class PaintDefines
    {
        private readonly Dictionary<IfDefSet, ExpressionContainer> _knownReferences =
            new Dictionary<IfDefSet, ExpressionContainer>();

        private readonly ScriptHelper _script;

        private int _waveCounter;

        public PaintDefines(ScriptHelper script)
        {
            _script = script;
        }

        public void Apply()
        {
            var leafs = new List<NodeHelper>();
            var ifdefs = new List<NodeHelper>();
            foreach (var scriptNode in _script.Nodes)
            {
                if (scriptNode.Extra == null)
                    scriptNode.Extra = new TranslatedMaterialGraph.NodeInfo();
                var container = new Container
                {
                    IsIfDef = NodeTypes.IsIfDefType(scriptNode.Type)
                };
                scriptNode.Extra.Define = container;
                if (container.IsIfDef) ifdefs.Add(scriptNode);

                if (scriptNode.OutputPins.Count == 0)
                {
                    leafs.Add(scriptNode);
                    scriptNode.Extra.Define.IsAlways = true;
                }
            }

            // Mark all nodes connected to output as always present. These nodes don't need any defines.
            foreach (var nodeHelper in leafs)
                EnumerateConnectedNodes(nodeHelper, _ =>
                {
                    _.Extra.Define.IsAlways = true;
                    return !_.Extra.Define.IsIfDef;
                });

            // For each node connected to a ifdef node collect all ifdefs it is connected to.
            foreach (var nodeHelper in ifdefs)
            {
                var ref0 = new IfDefReference {Defined = true, Node = nodeHelper};
                var ref1 = new IfDefReference {Defined = false, Node = nodeHelper};
                GetOrAddExpressionContainer(new IfDefSet(ref0));
                GetOrAddExpressionContainer(new IfDefSet(ref1));
                EnumerateConnectedNodes(nodeHelper.InputPins[0], _ =>
                {
                    if (_.Extra.Define.IsAlways)
                        return false;
                    _.Extra.Define.OrIfDef(ref0);
                    return !_.Extra.Define.IsIfDef;
                });
                EnumerateConnectedNodes(nodeHelper.InputPins[1], _ =>
                {
                    if (_.Extra.Define.IsAlways)
                        return false;
                    _.Extra.Define.OrIfDef(ref1);
                    return !_.Extra.Define.IsIfDef;
                });
            }

            foreach (var node in _script.Nodes)
            {
                var expression = BuildExpression(node);
                if (expression == PreprocessorExpression.True) expression = null;

                if (expression == null) node.Extra.Define.IsAlways = true;
                node.Extra.Define.Expression = expression;
            }
        }

        private string BuildExpression(NodeHelper node)
        {
            var def = node.Extra.Define;
            if (def.IsAlways || def.IfDefs == null || def.IfDefs.Count == 0) return null;

            node.Extra.Define.IfDefs.Sort();
            var key = new IfDefSet(node.Extra.Define.IfDefs);
            var container = GetOrAddExpressionContainer(key);
            container.BuildExpression();
            return container.Expression;
        }

        private ExpressionContainer AddIfDefExpressionContainer(IfDefReference ifdef)
        {
            var expr = new FlatExpression(new Operands(ifdef.Node.Value),
                new FlatExpressionLine(ifdef.Defined ? 1ul : 0ul));

            if (ifdef.Node.Extra.Define.IfDefs?.Count > 0)
            {
                var parentIfDefs = ifdef.Node.Extra.Define.IfDefs;
                var listContainers = new List<ExpressionContainer>(parentIfDefs.Count);
                foreach (var ifDef in parentIfDefs)
                    listContainers.Add(GetOrAddExpressionContainer(new IfDefSet(ifDef)));

                var operands = new Operands(listContainers
                    .Select(_ => _.FlatExpression.Operands.Concat(expr.Operands))
                    .SelectMany(_ => _)
                    .Distinct()
                    .OrderBy(_ => _)
                    .ToArray());
                var flatExpr = new FlatExpression(operands);
                foreach (var c in listContainers) flatExpr = flatExpr.Or(c.FlatExpression);

                expr = flatExpr.And(expr);
            }

            if (ifdef.Defined)
                ifdef.Node.Extra.Define.IfDefExpression = expr.AsTreeExpression().ToString();
            else
                ifdef.Node.Extra.Define.IfNotDefExpression = expr.AsTreeExpression().ToString();
            var container = new ExpressionContainer {FlatExpression = expr};
            _knownReferences.Add(new IfDefSet(ifdef), container);
            return container;
        }

        private ExpressionContainer GetOrAddExpressionContainer(IfDefSet ifDefs)
        {
            ExpressionContainer container;
            if (_knownReferences.TryGetValue(ifDefs, out container))
                return container;

            if (ifDefs.Count == 1)
            {
                return AddIfDefExpressionContainer(ifDefs.First());
            }

            container = new ExpressionContainer();
            var listContainers = new List<ExpressionContainer>(ifDefs.Count);
            foreach (var ifDef in ifDefs) listContainers.Add(GetOrAddExpressionContainer(new IfDefSet(ifDef)));
            var operands = new Operands(listContainers.Select(_ => _.FlatExpression.Operands).SelectMany(_ => _)
                .Distinct().OrderBy(_ => _).ToArray());
            var flatExpr = new FlatExpression(operands);
            foreach (var c in listContainers) flatExpr = flatExpr.Or(c.FlatExpression);

            container.FlatExpression = flatExpr;
            _knownReferences.Add(ifDefs, container);
            return container;
        }

        private void EnumerateConnectedNodes(NodeHelper nodeHelper, Func<NodeHelper, bool> callback)
        {
            ++_waveCounter;
            var queue = new Queue<NodeHelper>();
            queue.Enqueue(nodeHelper);
            nodeHelper.Extra.Define.WaveIndex = _waveCounter;
            EnumerateConnectedNodesImpl(queue, callback);
        }

        private void EnumerateConnectedNodesImpl(Queue<NodeHelper> queue, Func<NodeHelper, bool> callback)
        {
            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                foreach (var pin in node.InputPins) EnumerateConnectedNodesImpl(pin, queue, callback);
            }
        }

        private void EnumerateConnectedNodesImpl(PinHelper pin, Queue<NodeHelper> queue,
            Func<NodeHelper, bool> callback)
        {
            foreach (var connectedNode in pin.ConnectedPins.Select(_ => _.Node))
                if (_waveCounter != connectedNode.Extra.Define.WaveIndex)
                {
                    connectedNode.Extra.Define.WaveIndex = _waveCounter;
                    if (callback(connectedNode)) queue.Enqueue(connectedNode);
                }

            EnumerateConnectedNodesImpl(queue, callback);
        }

        private void EnumerateConnectedNodes(PinHelper pinHelper, Func<NodeHelper, bool> callback)
        {
            ++_waveCounter;
            var queue = new Queue<NodeHelper>();
            pinHelper.Node.Extra.Define.WaveIndex = _waveCounter;
            EnumerateConnectedNodesImpl(pinHelper, queue, callback);
        }

        public class Container
        {
            /// <summary>
            ///     Expression for the node.
            /// </summary>
            public string Expression;

            /// <summary>
            ///     Expression for "defined" branch of ifdef. Only valid for ifdef nodes.
            /// </summary>
            public string IfDefExpression;

            public List<IfDefReference> IfDefs;

            /// <summary>
            ///     Expression for "undefined" branch of ifdef. Only valid for ifdef nodes.
            /// </summary>
            public string IfNotDefExpression;

            public bool IsAlways;
            public bool IsIfDef;
            public int WaveIndex;

            public void WriteLineIfDef(IT4Writer writer, string text)
            {
                writer.WriteLine(GetExpression(), text);
            }
            public string GetExpression()
            {
                return IsAlways ? null : Expression;
            }
            public void OrIfDef(IfDefReference reference)
            {
                (IfDefs ?? (IfDefs = new List<IfDefReference>())).Add(reference);
            }

            public Container Clone()
            {
                return new Container()
                {
                    Expression = Expression,
                    IfDefExpression = IfDefExpression,
                    IfDefs = IfDefs,
                    IfNotDefExpression = IfNotDefExpression,
                    IsAlways = IsAlways,
                    IsIfDef = IsIfDef,
                    WaveIndex = WaveIndex
                };
            }
        }

        private class ExpressionContainer
        {
            public string Expression { get; private set; }

            public FlatExpression FlatExpression { get; set; }

            public void BuildExpression()
            {
                if (FlatExpression != null)
                    Expression = FlatExpression.AsTreeExpression().ToString();
            }
        }

        //private void Apply(NodeHelper node, DefineTreeItem define)
        //{
        //    if (node.Extra == null)
        //        node.Extra = new NodeInfo();
        //    node.Extra.Define = DefineTreeItem.Or(node.Extra.Define, define);
        //    if (NodeTypes.IsIfDefType(node.Type))
        //    {
        //        var a = DefineTreeItem.And(node.Extra.Define, new Defined() { Value = node.Value });
        //        foreach (var connectedPin in node.InputPins[0].ConnectedPins.Select(_ => _.Node).Distinct())
        //        {
        //            Apply(connectedPin, a);
        //        }
        //        var b = DefineTreeItem.And(node.Extra.Define, new Undefined() { Value = node.Value });
        //        foreach (var connectedPin in node.InputPins[1].ConnectedPins.Select(_ => _.Node).Distinct())
        //        {
        //            Apply(connectedPin, b);
        //        }
        //    }
        //    else
        //    {
        //        foreach (var nodeHelper in node.InputPins.ConnectedPins.Select(_ => _.Node).Distinct())
        //        {
        //            Apply(nodeHelper, define);
        //        }
        //    }
        //}
    }
}
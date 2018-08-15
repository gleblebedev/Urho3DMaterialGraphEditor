using System.Collections.Generic;
using System.Linq;
using Toe.Scripting;
using Toe.Scripting.Helpers;
using Toe.Scripting.WPF.ViewModels;

namespace Urho3DMaterialEditor.Model
{
    public class InlineFunctionNodeFactory : INodeFactory
    {
        private readonly Script _script;
        private readonly IList<PinWithConnection> _inputPins = new List<PinWithConnection>();
        private readonly IList<Pin> _outputPins = new List<Pin>();
        private NodeFactoryVisibility _visibility = NodeFactoryVisibility.Visible;

        public InlineFunctionNodeFactory(string name, Script script)
        {
            var segments = name.Split('.');
            Category = segments.Take(segments.Length - 2).Select(_ => FilterName(_)).ToArray();
            Type = Name = FilterName(segments[segments.Length - 2]);
            _script = script.Clone();
            _script.ClearGroups();


            var nodesUsedInGraph = new HashSet<int>(_script.Nodes.SelectMany(_ => _.InputPins).Select(_ => _.Connection).Where(_ => _ != null).Select(_=>_.NodeId));

            foreach (var scriptNode in _script.Nodes)
            {
                if (NodeTypes.IsConnectorType(scriptNode.Type))
                {
                    if (scriptNode.InputPins[0].Connection == null)
                        _inputPins.Add(new PinWithConnection(scriptNode.Name, scriptNode.InputPins[0].Type));
                    else if (!nodesUsedInGraph.Contains(scriptNode.Id))
                        _outputPins.Add(new Pin(scriptNode.Name, scriptNode.OutputPins[0].Type));
                }
            }

            InputTypes = _inputPins.Select(_ => _.Type).Distinct().ToList();
            OutputTypes = _outputPins.Select(_ => _.Type).Distinct().ToList();
        }

        public IEnumerable<PinWithConnection> InputPins => _inputPins;
        public IEnumerable<Pin> OutputPins => _outputPins;

        public string Type { get; }

        public string Name { get; }

        public string[] Category { get; }

        public NodeFactoryVisibility Visibility => _visibility;

        public IEnumerable<string> InputTypes { get; }

        public IEnumerable<string> OutputTypes { get; }

        public bool HasEnterPins { get; }

        public bool HasExitPins { get; }

        public ScriptNode Build()
        {
            var node = new ScriptNode();
            node.Type = Type;
            node.Name = Name;
            if (OutputTypes.Any())
                node.Category = NodeCategory.Function;
            else
                node.Category = NodeCategory.Result;
            node.InputPins.AddRange(InputPins.Select(_ => _.Clone()));
            node.OutputPins.AddRange(OutputPins.Select(_ => _.Clone()));
            return node;
        }

        private string FilterName(string s)
        {
            return s.Replace('_', ' ');
        }

        public void Inline(ScriptHelper<TranslatedMaterialGraph.NodeInfo> script,
            NodeHelper<TranslatedMaterialGraph.NodeInfo> scriptNode)
        {
            var nodes = script.Merge(_script).ToList();

            foreach (var node in nodes)
            {
                node.Id = scriptNode.Id;
                if (NodeTypes.IsConnectorType(node.Type))
                {
                    var hasInputConnections = node.InputPins[0].Links.Any();
                    var hasOutputConnections = node.OutputPins[0].Links.Any();
                    if (!hasOutputConnections && hasInputConnections)
                    {
                        var pin = scriptNode.OutputPins[node.Name];
                        if (pin == null)
                            throw new MaterialCompilationException("Output pin " + node.Name + " not found",
                                scriptNode.Id);
                        if (pin.Type != node.OutputPins[0].Type)
                            throw new MaterialCompilationException(
                                "Output pin " + node.Name + " mismatch: expected " + node.InputPins[0].Type +
                                " but found " + pin.Type);
                        script.CopyConnections(pin, node.OutputPins[0]);
                    }
                    else if (hasOutputConnections && !hasInputConnections)
                    {
                        var pin = scriptNode.InputPins[node.Name];
                        if (pin == null)
                            throw new MaterialCompilationException("Input pin " + node.Name + " not found",
                                scriptNode.Id);
                        if (pin.Type != node.InputPins[0].Type)
                            throw new MaterialCompilationException(
                                "Input pin " + node.Name + " mismatch: expected " + node.InputPins[0].Type +
                                " but found " + pin.Type);
                        script.CopyConnections(pin, node.InputPins[0]);
                    }
                }
            }

            script.Nodes.Remove(scriptNode);
        }

        public void Inline(ScriptViewModel script, NodeViewModel node)
        {
            var newScript = script.Script.Clone();
            var group = new NodeGroup {Name = node.Name};
            newScript.Groups.Add(group);
            var externalPins = new List<PinWithConnection>();

            foreach (var scriptNode in newScript.Nodes)
            foreach (var pinWithConnection in scriptNode.InputPins)
                if (pinWithConnection.Connection != null && pinWithConnection.Connection.NodeId == node.Id)
                    externalPins.Add(pinWithConnection);

            var newNodes = newScript.MergeWith(_script, (float) node.Position.X, (float) node.Position.Y);
            var inputConnectors = new Dictionary<string, ScriptNode>();
            var outputConnectors = new Dictionary<string, ScriptNode>();
            foreach (var scriptNode in newNodes)
            {
                scriptNode.GroupId = group.Id;
                if (NodeTypes.IsConnectorType(scriptNode.Type))
                    if (scriptNode.InputPins[0].Connection == null)
                        inputConnectors.Add(scriptNode.Name, scriptNode);
                    else
                        outputConnectors.Add(scriptNode.Name, scriptNode);
            }

            foreach (var nodeInputPin in node.Node.InputPins)
                if (nodeInputPin.Connection != null)
                {
                    ScriptNode connector;
                    if (inputConnectors.TryGetValue(nodeInputPin.Id, out connector))
                        connector.InputPins[0].Connection = nodeInputPin.Connection;
                }

            foreach (var externalConnection in externalPins)
            {
                ScriptNode connector;
                if (outputConnectors.TryGetValue(externalConnection.Connection.PinId, out connector))
                    externalConnection.Connection = new Connection(connector.Id, connector.OutputPins[0].Id);
                else
                    externalConnection.Connection = null;
            }

            newScript.Nodes.RemoveAt(node.Node.Id);
            script.Script = newScript;
        }
    }
}
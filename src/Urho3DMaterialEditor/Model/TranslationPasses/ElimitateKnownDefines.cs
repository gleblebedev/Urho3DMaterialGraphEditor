using System.Collections.Generic;
using System.Linq;
using Toe.Scripting.Helpers;

namespace Urho3DMaterialEditor.Model.TranslationPasses
{
    public class ElimitateKnownDefines
    {
        private readonly ScriptHelper<TranslatedMaterialGraph.NodeInfo> _script;

        public ElimitateKnownDefines(ScriptHelper<TranslatedMaterialGraph.NodeInfo> script)
        {
            _script = script;
        }

        public HashSet<string> Defines { get; } = new HashSet<string>();

        public HashSet<string> Undefines { get; } = new HashSet<string>();

        public void FindDefines()
        {
            foreach (var nodeHelper in _script.Nodes)
            {
                if (nodeHelper.Type == NodeTypes.Define)
                    if (!string.IsNullOrWhiteSpace(nodeHelper.Value))
                        Defines.Add(nodeHelper.Value.Trim());
                if (nodeHelper.Type == NodeTypes.Undefine)
                    if (!string.IsNullOrWhiteSpace(nodeHelper.Value))
                        Undefines.Add(nodeHelper.Value.Trim());
            }
        }

        public void Apply()
        {
            foreach (var nodeHelper in _script.Nodes.ToArray())
                if (NodeTypes.IsIfDefType(nodeHelper.Type))
                    if (Defines.Contains(nodeHelper.Value))
                    {
                        var oldLinks = nodeHelper.InputPins[0].Links.ToList();
                        foreach (var linkHelper in nodeHelper.OutputPins.First().Links)
                        foreach (var oldLink in oldLinks)
                            _script.Link(oldLink.From, linkHelper.To);
                        _script.Nodes.Remove(nodeHelper);
                    }
                    else if (Undefines.Contains(nodeHelper.Value))
                    {
                        var oldLinks = nodeHelper.InputPins[1].Links.ToList();
                        foreach (var linkHelper in nodeHelper.OutputPins.First().Links)
                        foreach (var oldLink in oldLinks)
                            _script.Link(oldLink.From, linkHelper.To);
                        _script.Nodes.Remove(nodeHelper);
                    }
        }
    }
}
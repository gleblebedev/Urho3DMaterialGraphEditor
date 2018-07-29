using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Toe.Scripting.Helpers;
using ScriptHelper = Toe.Scripting.Helpers.ScriptHelper<Urho3DMaterialEditor.Model.TranslatedMaterialGraph.NodeInfo>;
using NodeHelper = Toe.Scripting.Helpers.NodeHelper<Urho3DMaterialEditor.Model.TranslatedMaterialGraph.NodeInfo>;
using PinHelper = Toe.Scripting.Helpers.PinHelper<Urho3DMaterialEditor.Model.TranslatedMaterialGraph.NodeInfo>;


namespace Urho3DMaterialEditor.Model.TranslationPasses
{
    public class ElimitateDuplicates
    {
        private readonly ScriptHelper _script;

        public ElimitateDuplicates(ScriptHelper script)
        {
            _script = script;
        }


        public void Apply()
        {
            bool hasChanges;
            do
            {
                hasChanges = false;
                foreach (var nodeGroup in _script.Nodes.ToLookup(_ => new NodeKey(_)))
                {
                    var itemsInGroup = nodeGroup.ToList();
                    if (itemsInGroup.Count > 1)
                        foreach (var groupWithPotentialySameInputs in nodeGroup.ToLookup(_ =>
                            GetPinsAndLinksHash(_.InputPins)))
                            if (Apply(groupWithPotentialySameInputs.ToList()))
                                hasChanges = true;
                }
            } while (hasChanges);
        }

        private bool Apply(IList<NodeHelper> nodeGroup)
        {
            if (nodeGroup.Count <= 1)
                return false;

            var removedDups = false;
            for (var i = 0; i < nodeGroup.Count; ++i)
            {
                var masterCopy = nodeGroup[i];
                for (var j = nodeGroup.Count - 1; j > i; --j)
                {
                    var dup = nodeGroup[j];
                    if (ComparePinLists(dup.InputPins, masterCopy.InputPins))
                    {
                        foreach (var pair in masterCopy.OutputPins.Zip(dup.OutputPins, (m, d) => new {To = m, From = d})
                        ) _script.CopyConnections(pair.From, pair.To);
                        nodeGroup.RemoveAt(j);
                        _script.Nodes.Remove(dup);
                        removedDups = true;
                    }
                }
            }

            return removedDups;
        }


        private bool ComparePinLists(PinList<TranslatedMaterialGraph.NodeInfo> list1,
            PinList<TranslatedMaterialGraph.NodeInfo> list2)
        {
            if (list1.Count != list2.Count)
                return false;
            foreach (var compareRes in list1.Zip(list2, (a, b) => CompareConnectedPins(a, b)))
                if (!compareRes)
                    return false;
            return true;
        }

        private bool CompareConnectedPins(PinHelper pin1, PinHelper pin2)
        {
            if (pin1.Id != pin2.Id)
                return false;
            if (pin1.Type != pin2.Type)
                return false;
            if (pin1.Links.Count != pin2.Links.Count)
                return false;
            foreach (var compareRes in pin1.ConnectedPins.Zip(pin2.ConnectedPins, (a, b) => ReferenceEquals(a, b)))
                if (!compareRes)
                    return false;
            return true;
        }

        private int GetPinsAndLinksHash(PinList<TranslatedMaterialGraph.NodeInfo> pins)
        {
            var hashCode = -1248223815;
            foreach (var pin in pins)
            {
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(pin.Id);
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(pin.Type);
                foreach (var link in pin.Links.OrderBy(_ => RuntimeHelpers.GetHashCode(_)))
                    hashCode = hashCode * -1521134295 + RuntimeHelpers.GetHashCode(link.From);
            }

            return hashCode;
        }

        private struct NodeKey
        {
            public NodeKey(NodeHelper _)
            {
                Type = _.Type;
                Name = _.Name;
                Value = _.Value;
                if (_.OutputPins.Count > 0)
                {
                    OutputId = _.OutputPins[0].Id;
                    OutputType = _.OutputPins[0].Type;
                }
                else
                {
                    OutputId = null;
                    OutputType = null;
                }
            }

            public string Type;
            public string Name;
            public string Value;
            public string OutputId;
            public string OutputType;
        }
    }
}
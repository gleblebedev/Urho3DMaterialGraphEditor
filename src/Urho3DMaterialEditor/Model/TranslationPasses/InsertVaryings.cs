using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Toe.Scripting.Helpers;
using ScriptHelper = Toe.Scripting.Helpers.ScriptHelper<Urho3DMaterialEditor.Model.TranslatedMaterialGraph.NodeInfo>;
using NodeHelper = Toe.Scripting.Helpers.NodeHelper<Urho3DMaterialEditor.Model.TranslatedMaterialGraph.NodeInfo>;
using PinHelper = Toe.Scripting.Helpers.PinHelper<Urho3DMaterialEditor.Model.TranslatedMaterialGraph.NodeInfo>;

namespace Urho3DMaterialEditor.Model.TranslationPasses
{
    public class InsertVaryings
    {
        private readonly ScriptHelper _script;
        private double _threshold = 3.5;
        int _varyingIndex = 1;

        public InsertVaryings(ScriptHelper script)
        {
            _script = script;
        }

        public void Apply()
        {
            List<int> invalidNodes = new List<int>();
            foreach (var scriptNode in _script.Nodes)
            {
                if (scriptNode.Extra.RequiredInPixelShader && scriptNode.Extra.UsedInVertexShader)
                {
                    invalidNodes.Add(scriptNode.Id);
                }
            }

            if (invalidNodes.Count > 0)
            {
                throw new MaterialCompilationException("This node should be exclusive to pixel shader but used in vertex shader", invalidNodes.Distinct().ToArray());
            }
            foreach (var scriptNode in _script.Nodes.ToArray())
            {
                if (scriptNode.Extra.RequiredInPixelShader)
                {
                    //if (scriptNode.Id == 13 && scriptNode.Name == "float/float")
                    //{
                    //    ++_counter;
                    //    if (_counter == 8)
                    //        Debug.WriteLine("iteration "+ _counter);
                    //}
                    MoveInputNodesToPixelShader(scriptNode);
                    foreach (var a in _script.Nodes)
                    {
                        foreach (var pin in a.InputPins)
                        {
                            if (pin.Links.Count > 1)
                            {
                                throw new MaterialCompilationException("Pin has more than 1 input", scriptNode.Id);
                            }
                        }
                    }
                }
            }
        }

        private int _counter;
        private void MoveInputNodesToPixelShader(NodeHelper scriptNode)
        {
            foreach (var pin in scriptNode.InputPins)
            {
                if (pin.Links.Count > 0)
                {
                    if (pin.Links.Count != 1)
                        throw new MaterialCompilationException("Input should have exactly one input", scriptNode.Id);

                    var from = pin.Links[0].From;
                    var to = pin.Links[0].To;
                    var dataSource = from.Node;
                    var extra = dataSource.Extra;
                    if (!extra.RequiredInPixelShader)
                    {
                        if (extra.EstimatedCost <= _threshold)
                        {
                            NodeHelper altNode = MoveToPixelShader(dataSource);
                            if (altNode != dataSource)
                            {
                                _script.Link(altNode.OutputPins[from.Id],
                                    scriptNode.InputPins[to.Id]);
                                _script.RemoveLink(pin.Links[0]);
                            }
                        }
                        else
                        {
                            var name = "Varying"+_varyingIndex;
                            ++_varyingIndex;
                            var setVarying = new NodeHelper
                            {
                                Type = NodeTypes.Special.SetVarying,
                                Name = "set " + name,
                                Value = name,
                                Id = dataSource.Id,
                                InputPins = {new PinHelper("", from.Type)}
                            };
                            setVarying.Extra = dataSource.Extra.Clone();
                            if (setVarying.Extra.Define != null)
                                setVarying.Extra.Define.IsIfDef = false;
                            setVarying.Extra.UsedInPixelShader = false;
                            var getVarying = new NodeHelper
                            {
                                Type = NodeTypes.Special.GetVarying,
                                Name = "get " + name,
                                Value = name,
                                Id = dataSource.Id,
                                OutputPins = {new PinHelper("", from.Type)}
                            };
                            getVarying.Extra = dataSource.Extra.Clone();
                            if (getVarying.Extra.Define != null)
                                getVarying.Extra.Define.IsIfDef = false;
                            getVarying.Extra.RequiredInPixelShader = true;
                            getVarying.Extra.UsedInVertexShader = false;
                            getVarying.Extra.EstimatedCost = 0;
                            _script.Add(setVarying);
                            _script.Add(getVarying);
                            _script.LinkData(dataSource.OutputPins[from.Id], setVarying);

                            var sourcePin = dataSource.OutputPins[from.Id];
                            foreach (var link in sourcePin.Links.ToArray())
                            {
                                if (link.To.Node.Extra.RequiredInPixelShader)
                                {
                                    _script.LinkData(getVarying, link.To);
                                    _script.RemoveLink(link);
                                }
                            }
                        }
                        if (pin.Links.Count != 1)
                            throw new MaterialCompilationException("Input should have exactly one input", scriptNode.Id);
                    }
                }
            }
        }

        private NodeHelper MoveToPixelShader(NodeHelper scriptNode)
        {
            if (scriptNode.Extra.PixelShaderCopy != null)
            {
                return scriptNode.Extra.PixelShaderCopy;
            }

            if (scriptNode.Name == "MatDiffColor")
            {
                scriptNode.Name = scriptNode.Name;
            }

            NodeHelper clone;
            //if (scriptNode.Extra.UsedInVertexShader)
            {
                clone = scriptNode.Clone();
                _script.Add(clone);
                clone.Extra = scriptNode.Extra.Clone();
                scriptNode.Extra.PixelShaderCopy = clone;
                clone.Extra.PixelShaderCopy = clone;
                scriptNode.Extra.PixelShaderCopy = clone;
                scriptNode.Extra.UsedInPixelShader = false; //TODO: It may be used more than once
                clone.Extra.UsedInVertexShader = false;
            }
            //else
            //{
            //    clone = scriptNode;

            //}
            clone.Extra.RequiredInPixelShader = true;

            foreach (var pin in scriptNode.InputPins)
            {
                if (pin.Links.Count > 0)
                {
                    if (pin.Links.Count != 1)
                        throw new MaterialCompilationException("Input should have exactly one input", scriptNode.Id);

                    var dataSource = pin.Links[0].From.Node;
                    NodeHelper altNode = MoveToPixelShader(dataSource);
                    if (altNode != dataSource)
                    {
                        _script.Link(altNode.OutputPins[pin.Links[0].From.Id], clone.InputPins[pin.Links[0].To.Id]);
                        if (clone == scriptNode)
                        {
                            _script.RemoveLink(pin.Links[0]);
                        }
                    }

                }
            }
            return clone;
        }


        private void InsertVaryings2()
        {
            //TODO: Fix it!
            //var varyingIndex = 1;
            //var nodeListCopy = Script.Nodes.ToList();
            //for (var index = 0; index < nodeListCopy.Count; index++)
            //{
            //    var scriptNode = nodeListCopy[index];
            //    if (scriptNode.Extra.Attribution == NodeAttribution.VertexShader)
            //    {
            //        NodeHelper setVarying = null;
            //        NodeHelper getVarying = null;

            //        var isConnectedToPixelShader =
            //            scriptNode.OutputPins.ConnectedPins.Any(_ =>
            //                _.Node.Extra.Attribution == NodeAttribution.PixelShader);
            //        if (!isConnectedToPixelShader)
            //            continue;

            //        var canBeInPixelShader = scriptNode.Extra.EstimatedCost.HasValue && (scriptNode.Extra.EstimatedCost.Value <= 2.5f);// CanBeInPixelShader(scriptNode);
            //        var isConnectedToVertexShader =
            //            scriptNode.OutputPins.ConnectedPins.Any(_ =>
            //                _.Node.Extra.Attribution == NodeAttribution.VertexShader);

            //        if (canBeInPixelShader)
            //            if (!isConnectedToVertexShader)
            //            {
            //                scriptNode.Extra.Attribution = NodeAttribution.PixelShader;
            //                continue;
            //            }
            //            else
            //            {
            //                var c = scriptNode.CloneWithConnections();
            //                c.Extra = new NodeInfo {Attribution = NodeAttribution.PixelShader};
            //                nodeListCopy.Add(c);
            //                foreach (var link in scriptNode.OutputPins.Links.ToList())
            //                    if (link.To.Node.Extra.Attribution == NodeAttribution.PixelShader)
            //                        Script.RemoveLink(link);
            //                foreach (var link in c.OutputPins.Links.ToList())
            //                    if (link.To.Node.Extra.Attribution == NodeAttribution.VertexShader)
            //                        Script.RemoveLink(link);
            //            }

            //        foreach (var link in scriptNode.OutputPins.Links.ToArray())
            //        {
            //            var to = link.To;
            //            if (to.Node.Extra?.Attribution == NodeAttribution.PixelShader)
            //            {
            //                var from = link.From;
            //                Script.RemoveLink(link);
            //                if (setVarying == null)
            //                {
            //                    var name = "Varying" + varyingIndex;
            //                    ++varyingIndex;
            //                    setVarying = new NodeHelper
            //                    {
            //                        Type = NodeTypes.Special.SetVarying,
            //                        Name = "set " + name,
            //                        Value = name,
            //                        InputPins = {new PinHelper("", from.Type)}
            //                    };
            //                    getVarying = new NodeHelper
            //                    {
            //                        Type = NodeTypes.Special.GetVarying,
            //                        Name = "get " + name,
            //                        Value = name,
            //                        OutputPins = {new PinHelper("", from.Type)}
            //                    };
            //                    Script.Add(setVarying);
            //                    Script.Add(getVarying);
            //                }

            //                Script.Link(from, setVarying.InputPins[0]);
            //                Script.Link(getVarying.OutputPins[0], to);
            //            }
            //        }
            //    }
            //}
        }
    }
}
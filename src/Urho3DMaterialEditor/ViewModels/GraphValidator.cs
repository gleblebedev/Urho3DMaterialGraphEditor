using System;
using System.Collections.Generic;
using System.Linq;
using Toe.Scripting.WPF.ViewModels;
using Urho3DMaterialEditor.Model;
using Urho3DMaterialEditor.Model.Templates;

namespace Urho3DMaterialEditor.ViewModels
{
    public class GraphValidator
    {
        private readonly Lazy<MainViewModel> _mainVm;
        private readonly MaterialNodeRegistry _nodeRegistry;

        public GraphValidator(Lazy<MainViewModel> mainVM, MaterialNodeRegistry nodeRegistry)
        {
            _mainVm = mainVM;
            _nodeRegistry = nodeRegistry;
        }

        public MaterialCompilationException Validate(ScriptViewModel script)
        {
            var _samplers = new Dictionary<string, NodeViewModel>();
            var _defines = new Dictionary<string, NodeViewModel>();
            var _undefines = new Dictionary<string, NodeViewModel>();
            var validPinTypes = new HashSet<string>(PinTypes.SamplerTypes.Concat(PinTypes.DataTypes));
            foreach (var scriptNode in script.Nodes)
            {
                if (NodeTypes.IsIfDefType(scriptNode.Type) || scriptNode.Type == NodeTypes.Define ||
                    scriptNode.Type == NodeTypes.Undefine)
                {
                    if (string.IsNullOrWhiteSpace(scriptNode.Value))
                        return new MaterialCompilationException("Value is not set", scriptNode.Id);

                    if (scriptNode.Value.Any(_ => char.IsWhiteSpace(_)))
                        return new MaterialCompilationException("Value contains whitespaces", scriptNode.Id);
                }

                if (scriptNode.Type == NodeTypes.Define)
                {
                    NodeViewModel conflictingDefine;
                    if (_undefines.TryGetValue(scriptNode.Value, out conflictingDefine))
                        return new MaterialCompilationException("Conflicting define found", conflictingDefine.Id,
                            scriptNode.Id);

                    _defines[scriptNode.Value] = scriptNode;
                }

                if (scriptNode.Type == NodeTypes.Undefine)
                {
                    NodeViewModel conflictingDefine;
                    if (_defines.TryGetValue(scriptNode.Value, out conflictingDefine))
                        return new MaterialCompilationException("Conflicting define found", conflictingDefine.Id,
                            scriptNode.Id);

                    _undefines[scriptNode.Value] = scriptNode;
                }

                if (NodeTypes.IsSampler(scriptNode.Type))
                {
                    var unitName = MaterialTemplate.GetTextureUnitName(scriptNode.Node.Name) ?? scriptNode.Node.Name;
                    NodeViewModel oldNode;
                    if (_samplers.TryGetValue(unitName, out oldNode))
                        return new MaterialCompilationException(
                            "Duplicate texture unit name. For this node Urho3D uses \"" + unitName +
                            "\" texture unit name.", oldNode.Id, scriptNode.Id);

                    _samplers.Add(unitName, scriptNode);
                }

                foreach (var pinViewModel in scriptNode.InputPins.Concat(scriptNode.OutputPins))
                    if (!validPinTypes.Contains(pinViewModel.Type))
                        return new MaterialCompilationException(
                            "Pin " + pinViewModel.Id + " has unknown type " + pinViewModel.Type, scriptNode.Id);

                if (scriptNode.Node.Type != NodeTypes.Function)
                {
                    var factories = _nodeRegistry.ResolveFactories(scriptNode.Node.Type).ToList();
                    if (factories.Count == 0)
                        return new MaterialCompilationException("Unknown node type " + scriptNode.Node.Type,
                            scriptNode.Id);

                    if (factories.Count == 1)
                    {
                        var n = factories[0].Build();
                        if (n.InputPins.Count != scriptNode.InputPins.Count)
                            return new MaterialCompilationException("Input pin doesn't match", scriptNode.Id);

                        if (n.OutputPins.Count != scriptNode.OutputPins.Count)
                            return new MaterialCompilationException("Output pin doesn't match", scriptNode.Id);

                        foreach (var pinPair in n.InputPins.Zip(scriptNode.InputPins,
                            (nn, ss) => new {Expected = nn, Actial = ss}))
                        {
                            var actualPin = pinPair.Actial.Pin;
                            var expectedPin = pinPair.Expected;
                            if (expectedPin.Id != actualPin.Id)
                                return new MaterialCompilationException(
                                    "Expected pin " + expectedPin.Id + " but found pin " + actualPin.Id, scriptNode.Id);

                            if (expectedPin.Type != actualPin.Type)
                                return new MaterialCompilationException(
                                    "Expected pin " + expectedPin.Id + " of type " + expectedPin.Type +
                                    " but found pin of type " + actualPin.Type, scriptNode.Id);
                        }

                        foreach (var pinPair in n.OutputPins.Zip(scriptNode.OutputPins,
                            (nn, ss) => new {Expected = nn, Actial = ss}))
                        {
                            var actualPin = pinPair.Actial.Pin;
                            var expectedPin = pinPair.Expected;
                            if (expectedPin.Id != actualPin.Id)
                                return new MaterialCompilationException(
                                    "Expected pin " + expectedPin.Id + " but found pin " + actualPin.Id, scriptNode.Id);

                            if (expectedPin.Type != actualPin.Type)
                                return new MaterialCompilationException(
                                    "Expected pin " + expectedPin.Id + " of type " + expectedPin.Type +
                                    " but found pin of type " + actualPin.Type, scriptNode.Id);
                        }
                    }
                }
            }

            return null;
        }
    }
}
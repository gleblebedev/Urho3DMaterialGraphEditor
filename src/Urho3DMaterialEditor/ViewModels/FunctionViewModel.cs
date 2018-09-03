using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using ReactiveUI;
using Toe.Scripting;
using Toe.Scripting.WPF.ViewModels;

namespace Urho3DMaterialEditor.ViewModels
{
    public class FunctionViewModel : NodeViewModel, IDisposable
    {
        private readonly IDisposable _subscription;
        private string _value;
        public string funcHeader;

        public FunctionViewModel(ScriptViewModel script, ScriptNode node) : base(script, node)
        {
            _value = node.Value;
            _subscription = this.WhenAnyValue(_ => _.EditableValue).Throttle(TimeSpan.FromSeconds(0.25))
                .ObserveOnDispatcher()
                .Subscribe(_ => BuildNodeValue());
            CanRename = true;
        }

        internal void AddPin(string val) {
            var np = new PinWithConnection(val,val);
            Node.InputPins.Add(np);
            InputPins.Add(new InputPinViewModel(this, np));
            CreateFuncHead();
        }

        public void CreateFuncHead() {
            funcHeader = "(";
            int nn = 1;
            foreach (var item in Node.InputPins) {
                funcHeader += item.Type + " var" + nn.ToString()+", ";
                nn++;
            }
            funcHeader = funcHeader.Substring(0, funcHeader.Length - 2)+")";
        }

        internal void OutPin(string val) {
            var np = new PinWithConnection(val, val);
            Node.OutputPins.Clear();
            Node.OutputPins.Add(np);
            OutputPins.Clear();
            OutputPins.Add(new OutputPinViewModel(this, np));
        }

        public string EditableValue
        {
            get => _value;
            set => RaiseAndSetIfChanged(ref _value, value);
        }

        internal void ClearInPins() {
            Node.InputPins.Clear();
            InputPins.Clear();
        }

        public void Dispose()
        {
            _subscription.Dispose();
        }

        private void BuildNodeValue()
        {
            Value = _value;
        }
    }
}
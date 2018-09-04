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
        public string funcHeader, funcFoot, functionTXT;

        public string FuncHeader {
            get => funcHeader;
            set => RaiseAndSetIfChanged(ref funcHeader, value);
        }

        public FunctionViewModel(ScriptViewModel script, ScriptNode node) : base(script, node)
        {
            _value = node.Value;
            _subscription = this.WhenAnyValue(_ => _.EditableValue).Throttle(TimeSpan.FromSeconds(0.25))
                .ObserveOnDispatcher()
                .Subscribe(_ => BuildNodeValue());
            CanRename = true;

            CreateFuncTXT();
        }

        internal void AddPin(string val) {
            var np = new PinWithConnection(val,val);
            Node.InputPins.Add(np);
            InputPins.Add(new InputPinViewModel(this, np));
            CreateFuncTXT();
        }

        public void CreateFuncTXT() {
            var outP = Node.OutputPins[0]?.Type;
            FuncHeader = outP + " " + Name + " (";
            int nn = 1;
            foreach (var item in Node.InputPins) {
                FuncHeader += item.Type + " var" + nn.ToString()+", ";
                nn++;
            }
            FuncHeader = FuncHeader.Substring(0, FuncHeader.Length - 2)+")";

            functionTXT = FuncHeader + "\n{\n" + (Value??"") + "\n" + "return "+ funcFoot+ ";\n}";
        }

        internal void OutPin(string val)
        {
            Node.OutputPins[0].Type = val;
            OutputPins.Clear();
            OutputPins.Add(new OutputPinViewModel(this, Node.OutputPins[0]));

            CreateFuncTXT();
        }

        public string EditableValue
        {
            get => _value;
            set => 
                RaiseAndSetIfChanged(ref _value, value);
        }

        internal void ClearInPins() {
            Node.InputPins.Clear();
            InputPins.Clear();

            FuncHeader = "";
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
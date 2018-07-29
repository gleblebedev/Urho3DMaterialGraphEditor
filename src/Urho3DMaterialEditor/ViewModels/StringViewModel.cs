using System;
using System.Reactive.Linq;
using ReactiveUI;
using Toe.Scripting;
using Toe.Scripting.WPF.ViewModels;

namespace Urho3DMaterialEditor.ViewModels
{
    public class StringViewModel : NodeViewModel, IDisposable
    {
        private readonly IDisposable _subscription;
        private string _value;

        public StringViewModel(ScriptViewModel script, ScriptNode node) : base(script, node)
        {
            _value = node.Value;
            _subscription = this.WhenAnyValue(_ => _.EditableValue).Throttle(TimeSpan.FromSeconds(0.25))
                .ObserveOnDispatcher()
                .Subscribe(_ => BuildNodeValue());
        }

        public string EditableValue
        {
            get => _value;
            set => RaiseAndSetIfChanged(ref _value, value);
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
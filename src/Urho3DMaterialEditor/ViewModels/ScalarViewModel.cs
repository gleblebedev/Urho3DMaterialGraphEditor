using System;
using System.Reactive.Linq;
using ReactiveUI;
using Toe.Scripting;
using Toe.Scripting.WPF.ViewModels;
using Urho3DMaterialEditor.Model;

namespace Urho3DMaterialEditor.ViewModels
{
    public class ScalarViewModel : NodeViewModel, IDisposable
    {
        private readonly IDisposable _subscription;
        private string _x = "0.0";

        public ScalarViewModel(ScriptViewModel script, ScriptNode node) : base(script, node)
        {
            CanRename = NodeTypes.IsParameter(node.Type);

            _x = node.Value;
            _subscription = this.WhenAnyValue(_ => _.X).Throttle(TimeSpan.FromSeconds(0.25)).ObserveOnDispatcher()
                .Subscribe(_ => BuildNodeValue());
        }


        public string X
        {
            get => _x;
            set => RaiseAndSetIfChanged(ref _x, value);
        }

        public void Dispose()
        {
            _subscription.Dispose();
        }

        private void BuildNodeValue()
        {
            Value = _x;
        }
    }
}
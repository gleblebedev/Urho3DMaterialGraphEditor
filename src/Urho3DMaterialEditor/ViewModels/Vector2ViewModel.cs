using System;
using System.Reactive.Linq;
using ReactiveUI;
using Toe.Scripting;
using Toe.Scripting.WPF.ViewModels;
using Urho3DMaterialEditor.Model;

namespace Urho3DMaterialEditor.ViewModels
{
    public class Vector2ViewModel : NodeViewModel, IDisposable
    {
        private readonly IDisposable _subscription;
        private string _x = "0.0";
        private string _y = "0.0";

        public Vector2ViewModel(ScriptViewModel script, ScriptNode node) : base(script, node)
        {
            CanRename = NodeTypes.IsParameter(node.Type);

            var components = (node.Value ?? "").Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            if (components.Length > 0)
            {
                _x = components[0];
                if (components.Length > 1) _y = components[1];
            }

            _subscription = this.WhenAnyValue(_ => _.X, _ => _.Y).Throttle(TimeSpan.FromSeconds(0.25))
                .ObserveOnDispatcher()
                .Subscribe(_ => BuildNodeValue());
        }

        public string X
        {
            get => _x;
            set => RaiseAndSetIfChanged(ref _x, value);
        }

        public string Y
        {
            get => _y;
            set => RaiseAndSetIfChanged(ref _y, value);
        }

        public void Dispose()
        {
            _subscription.Dispose();
        }

        private void BuildNodeValue()
        {
            Value = string.Format("{0} {1}", X, Y);
        }
    }
}
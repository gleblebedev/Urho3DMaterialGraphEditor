using System;
using System.Reactive.Linq;
using ReactiveUI;
using Toe.Scripting;
using Toe.Scripting.WPF.ViewModels;
using Urho3DMaterialEditor.Model;

namespace Urho3DMaterialEditor.ViewModels
{
    public class Mat2ViewModel : NodeViewModel, IDisposable
    {
        private readonly string[] _m = new string[9];
        private readonly IDisposable _subscription;

        public Mat2ViewModel(ScriptViewModel script, ScriptNode node) : base(script, node)
        {
            CanRename = NodeTypes.IsParameter(node.Type);

            var components = (node.Value ?? "").Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            for (var index = 0; index < _m.Length; index++) _m[index] = "0.0";

            for (var index = 0; index < components.Length; index++) _m[index] = components[index];

            _subscription = Observable.CombineLatest(
                    this.ObservableForProperty(_ => _.M00, false, false),
                    this.ObservableForProperty(_ => _.M01, false, false),
                    this.ObservableForProperty(_ => _.M10, false, false),
                    this.ObservableForProperty(_ => _.M11, false, false)
                )
                .Throttle(TimeSpan.FromSeconds(0.25))
                .ObserveOnDispatcher()
                .Subscribe(_ => BuildNodeValue());
        }


        public string M00
        {
            get => _m[0];
            set => RaiseAndSetIfChanged(ref _m[0], value);
        }

        public string M01
        {
            get => _m[1];
            set => RaiseAndSetIfChanged(ref _m[1], value);
        }

        public string M10
        {
            get => _m[2];
            set => RaiseAndSetIfChanged(ref _m[2], value);
        }

        public string M11
        {
            get => _m[3];
            set => RaiseAndSetIfChanged(ref _m[3], value);
        }

        public void Dispose()
        {
            _subscription?.Dispose();
        }


        private void BuildNodeValue()
        {
            Value = string.Join(" ", _m);
        }
    }
}
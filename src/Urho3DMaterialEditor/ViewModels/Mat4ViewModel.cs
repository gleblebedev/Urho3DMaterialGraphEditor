using System;
using System.Reactive.Linq;
using ReactiveUI;
using Toe.Scripting;
using Toe.Scripting.WPF.ViewModels;
using Urho3DMaterialEditor.Model;

namespace Urho3DMaterialEditor.ViewModels
{
    public class Mat4ViewModel : NodeViewModel, IDisposable
    {
        private readonly string[] _m = new string[16];
        private readonly IDisposable _subscription;

        public Mat4ViewModel(ScriptViewModel script, ScriptNode node) : base(script, node)
        {
            CanRename = NodeTypes.IsParameter(node.Type);

            var components = (node.Value ?? "").Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            for (var index = 0; index < _m.Length; index++) _m[index] = "0.0";

            for (var index = 0; index < components.Length; index++) _m[index] = components[index];

            _subscription = Observable.CombineLatest(
                    this.ObservableForProperty(_ => _.M00, false, false),
                    this.ObservableForProperty(_ => _.M01, false, false),
                    this.ObservableForProperty(_ => _.M02, false, false),
                    this.ObservableForProperty(_ => _.M03, false, false),
                    this.ObservableForProperty(_ => _.M10, false, false),
                    this.ObservableForProperty(_ => _.M11, false, false),
                    this.ObservableForProperty(_ => _.M12, false, false),
                    this.ObservableForProperty(_ => _.M13, false, false),
                    this.ObservableForProperty(_ => _.M20, false, false),
                    this.ObservableForProperty(_ => _.M21, false, false),
                    this.ObservableForProperty(_ => _.M22, false, false),
                    this.ObservableForProperty(_ => _.M23, false, false),
                    this.ObservableForProperty(_ => _.M30, false, false),
                    this.ObservableForProperty(_ => _.M31, false, false),
                    this.ObservableForProperty(_ => _.M32, false, false),
                    this.ObservableForProperty(_ => _.M33, false, false)
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

        public string M02
        {
            get => _m[2];
            set => RaiseAndSetIfChanged(ref _m[2], value);
        }

        public string M03
        {
            get => _m[3];
            set => RaiseAndSetIfChanged(ref _m[3], value);
        }

        public string M10
        {
            get => _m[4];
            set => RaiseAndSetIfChanged(ref _m[4], value);
        }

        public string M11
        {
            get => _m[5];
            set => RaiseAndSetIfChanged(ref _m[5], value);
        }

        public string M12
        {
            get => _m[6];
            set => RaiseAndSetIfChanged(ref _m[6], value);
        }

        public string M13
        {
            get => _m[7];
            set => RaiseAndSetIfChanged(ref _m[7], value);
        }

        public string M20
        {
            get => _m[8];
            set => RaiseAndSetIfChanged(ref _m[8], value);
        }

        public string M21
        {
            get => _m[9];
            set => RaiseAndSetIfChanged(ref _m[9], value);
        }

        public string M22
        {
            get => _m[10];
            set => RaiseAndSetIfChanged(ref _m[10], value);
        }

        public string M23
        {
            get => _m[11];
            set => RaiseAndSetIfChanged(ref _m[11], value);
        }

        public string M30
        {
            get => _m[12];
            set => RaiseAndSetIfChanged(ref _m[12], value);
        }

        public string M31
        {
            get => _m[13];
            set => RaiseAndSetIfChanged(ref _m[13], value);
        }

        public string M32
        {
            get => _m[14];
            set => RaiseAndSetIfChanged(ref _m[14], value);
        }

        public string M33
        {
            get => _m[15];
            set => RaiseAndSetIfChanged(ref _m[15], value);
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
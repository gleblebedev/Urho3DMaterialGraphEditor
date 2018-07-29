using System;
using System.Globalization;
using System.Reactive.Linq;
using ReactiveUI;
using Toe.Scripting;
using Toe.Scripting.WPF.ViewModels;

namespace Urho3DMaterialEditor.ViewModels
{
    public class ColorViewModel : NodeViewModel, IDisposable
    {
        public static IFormatProvider FormatProvider = CultureInfo.InvariantCulture;
        private readonly IDisposable _subscription;
        private float _a;
        private float _b;
        private float _g;
        private float _r;

        public ColorViewModel(ScriptViewModel script, ScriptNode node) : base(script, node)
        {
            var components = (node.Value ?? "").Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            if (components.Length > 0)
            {
                if (!float.TryParse(components[0], NumberStyles.Any, FormatProvider, out _r))
                    _r = 0.0f;
                if (components.Length > 1)
                {
                    if (!float.TryParse(components[1], NumberStyles.Any, FormatProvider, out _g))
                        _g = 0.0f;
                    if (components.Length > 2)
                    {
                        if (!float.TryParse(components[2], NumberStyles.Any, FormatProvider, out _b))
                            _b = 0.0f;
                        if (components.Length > 3)
                            if (!float.TryParse(components[3], NumberStyles.Any, FormatProvider, out _a))
                                _a = 1.0f;
                    }
                }
            }

            _subscription = this.WhenAnyValue(_ => _.R, _ => _.G, _ => _.B, _ => _.A)
                .Throttle(TimeSpan.FromSeconds(0.25))
                .ObserveOnDispatcher().Subscribe(_ => BuildNodeValue());
        }

        public float R
        {
            get => _r;
            set
            {
                if (RaiseAndSetIfChanged(ref _r, value)) BuildNodeValue();
            }
        }

        public float G
        {
            get => _g;
            set
            {
                if (RaiseAndSetIfChanged(ref _g, value)) BuildNodeValue();
            }
        }

        public float B
        {
            get => _b;
            set
            {
                if (RaiseAndSetIfChanged(ref _b, value)) BuildNodeValue();
            }
        }

        public float A
        {
            get => _a;
            set
            {
                if (RaiseAndSetIfChanged(ref _a, value)) BuildNodeValue();
            }
        }

        public void Dispose()
        {
            _subscription.Dispose();
        }

        private void BuildNodeValue()
        {
            Value = string.Format(FormatProvider, "{0:G} {1:G} {2:G} {3:G}", R.ToString("0.000"), G.ToString("0.000"),
                B.ToString("0.000"), A.ToString("0.000")).Replace(',', '.');
        }
    }
}
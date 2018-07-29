using System;
using System.Linq;
using Toe.Scripting;
using Toe.Scripting.Helpers;

namespace Urho3DMaterialEditor.Model
{
    public class MaterialCompilationException : Exception
    {
        public MaterialCompilationException(int nodeId, string pinId, Exception innerException) : base(
            "Failed to compile material", innerException)
        {
            Pins = new[] {new Connection(nodeId, pinId)};
        }

        public MaterialCompilationException(PinHelper<TranslatedMaterialGraph.NodeInfo> pin, Exception innerException) :
            base(
                "Failed to compile material", innerException)
        {
            Pins = new[] {new Connection(pin.Node?.Id ?? 0, pin.Id)};
        }

        public MaterialCompilationException(NodeAndPin nodeAndPin) : base("Failed to compile material")
        {
            Pins = new[] {new Connection(nodeAndPin.Node?.Id ?? 0, nodeAndPin.Pin?.Id)};
        }

        public MaterialCompilationException(string message, params int[] nodeId) : base(
            message ?? "Failed to compile material")
        {
            Pins = nodeId.Select(_ => new Connection(_, "")).ToArray();
        }

        public Connection[] Pins { get; }
    }
}
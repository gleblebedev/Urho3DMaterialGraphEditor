using Toe.Scripting;

namespace Urho3DMaterialEditor.Model
{
    public class ZoneDataNodeFactory : FunctionNodeFactory
    {
        public const string AmbientStartColor = "AmbientStartColor";
        public const string AmbientEndColor = "AmbientEndColor";
        public const string Zone = "Zone";

        public ZoneDataNodeFactory() : base(NodeTypes.ZoneData, "Zone", new[] {MaterialNodeRegistry.Categories.Data},
            new Pin[] { },
            new[]
            {
                new Pin(AmbientStartColor, PinTypes.Vec4),
                new Pin(AmbientEndColor, PinTypes.Vec4),
                new Pin(Zone, PinTypes.Mat4x3)
            })
        {
        }
    }
}
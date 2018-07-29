using Toe.Scripting;

namespace Urho3DMaterialEditor.Model
{
    public class ObjectDataNodeFactory : FunctionNodeFactory
    {
        public const string Model = "Model";
        public const string Skin = "Skin";
        public const string Normal = "Normal";

        public ObjectDataNodeFactory() : base(NodeTypes.ObjectData, "Object",
            new[] {MaterialNodeRegistry.Categories.Data},
            new Pin[] { },
            new[]
            {
                new Pin(Model, PinTypes.Mat4x3),
                new Pin(Skin, PinTypes.Skin),
                new Pin(Normal, PinTypes.Mat3)
            })
        {
        }
    }
}
using Toe.Scripting;

namespace Urho3DMaterialEditor.Model
{
    public class VertexDataNodeFactory : FunctionNodeFactory
    {
        public const string Pos = "Pos";
        public const string Normal = "Normal";
        public const string Color = "Color";
        public const string TexCoord = "TexCoord";
        public const string TexCoord1 = "TexCoord1";
        public const string Tangent = "Tangent";
        public const string BlendWeights = "BlendWeights";
        public const string BlendIndices = "BlendIndices";
        public const string CubeTexCoord = "CubeTexCoord";
        public const string CubeTexCoord1 = "CubeTexCoord1";

        public VertexDataNodeFactory() : base(NodeTypes.VertexData, "Vertex",
            new[] {MaterialNodeRegistry.Categories.Data},
            new Pin[] { },
            new[]
            {
                new Pin(Pos, PinTypes.Vec4),
                new Pin(Normal, PinTypes.Vec3),
                new Pin(Color, PinTypes.Vec4),
                new Pin(TexCoord, PinTypes.Vec2),
                new Pin(TexCoord1, PinTypes.Vec2),
                new Pin(Tangent, PinTypes.Vec4),
                new Pin(BlendWeights, PinTypes.Vec4),
                new Pin(BlendIndices, PinTypes.Ivec4),
                new Pin(CubeTexCoord, PinTypes.Vec3),
                new Pin(CubeTexCoord1, PinTypes.Vec4)
            })
        {
        }
    }
}
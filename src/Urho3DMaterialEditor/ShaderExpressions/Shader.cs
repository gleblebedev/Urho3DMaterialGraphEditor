using Urho;
using Urho3DMaterialEditor.Model;

namespace Urho3DMaterialEditor.ShaderExpressions
{
    public static class Shader
    {
        [NodeType(NodeTypes.AddFloatFloat)]
        public static float Add(float x, float y)
        {
            return x + y;
        }

        [NodeType(NodeTypes.AddVec2Vec2)]
        public static Vector2 Add(Vector2 x, Vector2 y)
        {
            return x + y;
        }

        [NodeType(NodeTypes.AddVec3Vec3)]
        public static Vector3 Add(Vector3 x, Vector3 y)
        {
            return x + y;
        }

        [NodeType(NodeTypes.AddVec4Vec4)]
        public static Vector4 Add(Vector4 x, Vector4 y)
        {
            return x + y;
        }

        [NodeType(NodeTypes.SubtractFloatFloat)]
        public static float Subtract(float x, float y)
        {
            return x - y;
        }

        [NodeType(NodeTypes.SubtractVec2Vec2)]
        public static Vector2 Subtract(Vector2 x, Vector2 y)
        {
            return x - y;
        }

        [NodeType(NodeTypes.SubtractVec3Vec3)]
        public static Vector3 Subtract(Vector3 x, Vector3 y)
        {
            return x - y;
        }

        [NodeType(NodeTypes.SubtractVec4Vec4)]
        public static Vector4 Subtract(Vector4 x, Vector4 y)
        {
            return x - y;
        }
    }
}
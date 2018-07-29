using System.Collections.Generic;

namespace Urho3DMaterialEditor.Model
{
    public static class PinTypes
    {
        public const string Sampler2D = "sampler2D";

        //public const string Sampler3D = "sampler3D";
        public const string SamplerCube = "samplerCube";
        public const string Vec4 = "vec4";
        public const string Vec3 = "vec3";
        public const string Vec2 = "vec2";
        public const string Float = "float";
        public const string Bool = "bool";
        public const string Bvec4 = "bvec4";
        public const string Bvec3 = "bvec3";
        public const string Bvec2 = "bvec2";
        public const string Int = "int";
        public const string Ivec4 = "ivec4";
        public const string Ivec3 = "ivec3";
        public const string Ivec2 = "ivec2";
        public const string Mat2 = "mat2";
        public const string Mat3 = "mat3";
        public const string Mat4 = "mat4";
        public const string Mat4x3 = "mat4x3";
        public const string Skin = "skinMatrices";
        public const string LightMatrices = "lightMatrices";
        public const string VertexLights = "vertexLights";

        public static IEnumerable<string> SamplerTypes
        {
            get
            {
                yield return Sampler2D;
                //yield return Sampler3D;
                yield return SamplerCube;
            }
        }

        public static IEnumerable<string> DataTypesExceptArrays
        {
            get
            {
                yield return Float;
                yield return Vec2;
                yield return Vec3;
                yield return Vec4;
                yield return Bool;
                yield return Bvec2;
                yield return Bvec3;
                yield return Bvec4;
                yield return Int;
                yield return Ivec2;
                yield return Ivec3;
                yield return Ivec4;
                yield return Mat2;
                yield return Mat3;
                yield return Mat4;
                yield return Mat4x3;
            }
        }

        public static IEnumerable<string> DataTypes
        {
            get
            {
                foreach (var type in DataTypesExceptArrays) yield return type;
                yield return Skin;
                yield return LightMatrices;
                yield return VertexLights;
            }
        }

        public static class Special
        {
            public const string Color = "color";
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Urho3DMaterialEditor.Model
{
    public static class NodeTypes
    {
        public const string IfDefPrefix = "ifdef";
        public const string Define = "define";
        public const string Undefine = "undefine";

        public const string Sampler2D = "sampler2D";
        public const string Sampler3D = "sampler3D";
        public const string SamplerCube = "SamplerCube";

        public const string Discard = "discard";
        public const string AmbientColor = "ambientColor";
        public const string RefractionColor = "refractionColor";
        public const string Opacity = "opacity";
        public const string LightColor = "lightColor";
        public const string DeferredOutput = "deferredOutput";
        public const string PositionOutput = "positionOutput";
        public const string VertexData = "vertexData";
        public const string ObjectData = "objectData";
        public const string LightData = "lightData";
        public const string CameraData = "cameraData";
        public const string ZoneData = "zoneData";
        public const string FragCoord = "fragCoord";
        public const string FrontFacing = "frontFacing";
        public const string Cull = "cull";
        public const string ShadowCull = "shadowCull";
        public const string Fill = "fill";

        public const string SampleShadow = "sampleShadow(shadowMap,vec4)";
        public const string SampleVSMShadow = "sampleVSMShadow(shadowMap,vec2)";

        public const string RadiansFloat = "radians(float)"; //float radians(float degrees)
        public const string RadiansVec2 = "radians(vec2)"; //vec2 radians(vec2 degrees)
        public const string RadiansVec3 = "radians(vec3)"; //vec3 radians(vec3 degrees)
        public const string RadiansVec4 = "radians(vec4)"; //vec4 radians(vec4 degrees)
        public const string DegreesFloat = "degrees(float)"; //float degrees(float radians)
        public const string DegreesVec2 = "degrees(vec2)"; //vec2 degrees(vec2 radians)
        public const string DegreesVec3 = "degrees(vec3)"; //vec3 degrees(vec3 radians)
        public const string DegreesVec4 = "degrees(vec4)"; //vec4 degrees(vec4 radians)
        public const string SinFloat = "sin(float)"; //float sin(float angle)
        public const string SinVec2 = "sin(vec2)"; //vec2 sin(vec2 angle)
        public const string SinVec3 = "sin(vec3)"; //vec3 sin(vec3 angle)
        public const string SinVec4 = "sin(vec4)"; //vec4 sin(vec4 angle)
        public const string CosFloat = "cos(float)"; //float cos(float angle)
        public const string CosVec2 = "cos(vec2)"; //vec2 cos(vec2 angle)
        public const string CosVec3 = "cos(vec3)"; //vec3 cos(vec3 angle)
        public const string CosVec4 = "cos(vec4)"; //vec4 cos(vec4 angle)
        public const string TanFloat = "tan(float)"; //float tan(float angle)
        public const string TanVec2 = "tan(vec2)"; //vec2 tan(vec2 angle)
        public const string TanVec3 = "tan(vec3)"; //vec3 tan(vec3 angle)
        public const string TanVec4 = "tan(vec4)"; //vec4 tan(vec4 angle)
        public const string AsinFloat = "asin(float)"; //float asin(float x)
        public const string AsinVec2 = "asin(vec2)"; //vec2 asin(vec2 x)
        public const string AsinVec3 = "asin(vec3)"; //vec3 asin(vec3 x)
        public const string AsinVec4 = "asin(vec4)"; //vec4 asin(vec4 x)
        public const string AcosFloat = "acos(float)"; //float acos(float x)
        public const string AcosVec2 = "acos(vec2)"; //vec2 acos(vec2 x)
        public const string AcosVec3 = "acos(vec3)"; //vec3 acos(vec3 x)
        public const string AcosVec4 = "acos(vec4)"; //vec4 acos(vec4 x)
        public const string AtanFloat = "atan(float)"; //float atan(float y_over_x)
        public const string AtanVec2 = "atan(vec2)"; //vec2 atan(vec2 y_over_x)
        public const string AtanVec3 = "atan(vec3)"; //vec3 atan(vec3 y_over_x)
        public const string AtanVec4 = "atan(vec4)"; //vec4 atan(vec4 y_over_x)
        public const string AtanFloatFloat = "atan(float,float)"; //float atan(float y, float x)
        public const string AtanVec2Vec2 = "atan(vec2,vec2)"; //vec2 atan(vec2 y, vec2 x)
        public const string AtanVec3Vec3 = "atan(vec3,vec3)"; //vec3 atan(vec3 y, vec3 x)
        public const string AtanVec4Vec4 = "atan(vec4,vec4)"; //vec4 atan(vec4 y, vec4 x)
        public const string PowFloatFloat = "pow(float,float)"; //float pow(float x, float y)
        public const string PowVec2Vec2 = "pow(vec2,vec2)"; //vec2 pow(vec2 x, vec2 y)
        public const string PowVec3Vec3 = "pow(vec3,vec3)"; //vec3 pow(vec3 x, vec3 y)
        public const string PowVec4Vec4 = "pow(vec4,vec4)"; //vec4 pow(vec4 x, vec4 y)
        public const string ExpFloat = "exp(float)"; //float exp(float x)
        public const string ExpVec2 = "exp(vec2)"; //vec2 exp(vec2 x)
        public const string ExpVec3 = "exp(vec3)"; //vec3 exp(vec3 x)
        public const string ExpVec4 = "exp(vec4)"; //vec4 exp(vec4 x)
        public const string LogFloat = "log(float)"; //float log(float x)
        public const string LogVec2 = "log(vec2)"; //vec2 log(vec2 x)
        public const string LogVec3 = "log(vec3)"; //vec3 log(vec3 x)
        public const string LogVec4 = "log(vec4)"; //vec4 log(vec4 x)
        public const string Exp2Float = "exp2(float)"; //float exp2(float x)
        public const string Exp2Vec2 = "exp2(vec2)"; //vec2 exp2(vec2 x)
        public const string Exp2Vec3 = "exp2(vec3)"; //vec3 exp2(vec3 x)
        public const string Exp2Vec4 = "exp2(vec4)"; //vec4 exp2(vec4 x)
        public const string Log2Float = "log2(float)"; //float log2(float x)
        public const string Log2Vec2 = "log2(vec2)"; //vec2 log2(vec2 x)
        public const string Log2Vec3 = "log2(vec3)"; //vec3 log2(vec3 x)
        public const string Log2Vec4 = "log2(vec4)"; //vec4 log2(vec4 x)
        public const string SqrtFloat = "sqrt(float)"; //float sqrt(float x)
        public const string SqrtVec2 = "sqrt(vec2)"; //vec2 sqrt(vec2 x)
        public const string SqrtVec3 = "sqrt(vec3)"; //vec3 sqrt(vec3 x)
        public const string SqrtVec4 = "sqrt(vec4)"; //vec4 sqrt(vec4 x)
        public const string InversesqrtFloat = "inversesqrt(float)"; //float inversesqrt(float x)
        public const string InversesqrtVec2 = "inversesqrt(vec2)"; //vec2 inversesqrt(vec2 x)
        public const string InversesqrtVec3 = "inversesqrt(vec3)"; //vec3 inversesqrt(vec3 x)
        public const string InversesqrtVec4 = "inversesqrt(vec4)"; //vec4 inversesqrt(vec4 x)
        public const string AbsFloat = "abs(float)"; //float abs(float x)
        public const string AbsVec2 = "abs(vec2)"; //vec2 abs(vec2 x)
        public const string AbsVec3 = "abs(vec3)"; //vec3 abs(vec3 x)
        public const string AbsVec4 = "abs(vec4)"; //vec4 abs(vec4 x)
        public const string SignFloat = "sign(float)"; //float sign(float x)
        public const string SignVec2 = "sign(vec2)"; //vec2 sign(vec2 x)
        public const string SignVec3 = "sign(vec3)"; //vec3 sign(vec3 x)
        public const string SignVec4 = "sign(vec4)"; //vec4 sign(vec4 x)
        public const string FloorFloat = "floor(float)"; //float floor(float x)
        public const string FloorVec2 = "floor(vec2)"; //vec2 floor(vec2 x)
        public const string FloorVec3 = "floor(vec3)"; //vec3 floor(vec3 x)
        public const string FloorVec4 = "floor(vec4)"; //vec4 floor(vec4 x)
        public const string CeilFloat = "ceil(float)"; //float ceil(float x)
        public const string CeilVec2 = "ceil(vec2)"; //vec2 ceil(vec2 x)
        public const string CeilVec3 = "ceil(vec3)"; //vec3 ceil(vec3 x)
        public const string CeilVec4 = "ceil(vec4)"; //vec4 ceil(vec4 x)
        public const string FractFloat = "fract(float)"; //float fract(float x)
        public const string FractVec2 = "fract(vec2)"; //vec2 fract(vec2 x)
        public const string FractVec3 = "fract(vec3)"; //vec3 fract(vec3 x)
        public const string FractVec4 = "fract(vec4)"; //vec4 fract(vec4 x)
        public const string ModFloatFloat = "mod(float,float)"; //float mod(float x, float y)
        public const string ModVec2Vec2 = "mod(vec2,vec2)"; //vec2 mod(vec2 x, vec2 y)
        public const string ModVec3Vec3 = "mod(vec3,vec3)"; //vec3 mod(vec3 x, vec3 y)
        public const string ModVec4Vec4 = "mod(vec4,vec4)"; //vec4 mod(vec4 x, vec4 y)
        public const string ModVec2Float = "mod(vec2,float)"; //vec2 mod(vec2 x, float y)
        public const string ModVec3Float = "mod(vec3,float)"; //vec3 mod(vec3 x, float y)
        public const string ModVec4Float = "mod(vec4,float)"; //vec4 mod(vec4 x, float y)
        public const string MinFloatFloat = "min(float,float)"; //float min(float x, float y)
        public const string MinVec2Vec2 = "min(vec2,vec2)"; //vec2 min(vec2 x, vec2 y)
        public const string MinVec3Vec3 = "min(vec3,vec3)"; //vec3 min(vec3 x, vec3 y)
        public const string MinVec4Vec4 = "min(vec4,vec4)"; //vec4 min(vec4 x, vec4 y)
        public const string MinVec2Float = "min(vec2,float)"; //vec2 min(vec2 x, float y)
        public const string MinVec3Float = "min(vec3,float)"; //vec3 min(vec3 x, float y)
        public const string MinVec4Float = "min(vec4,float)"; //vec4 min(vec4 x, float y)
        public const string MaxFloatFloat = "max(float,float)"; //float max(float x, float y)
        public const string MaxVec2Vec2 = "max(vec2,vec2)"; //vec2 max(vec2 x, vec2 y)
        public const string MaxVec3Vec3 = "max(vec3,vec3)"; //vec3 max(vec3 x, vec3 y)
        public const string MaxVec4Vec4 = "max(vec4,vec4)"; //vec4 max(vec4 x, vec4 y)
        public const string MaxVec2Float = "max(vec2,float)"; //vec2 max(vec2 x, float y)
        public const string MaxVec3Float = "max(vec3,float)"; //vec3 max(vec3 x, float y)
        public const string MaxVec4Float = "max(vec4,float)"; //vec4 max(vec4 x, float y)

        public const string
            ClampFloatFloatFloat = "clamp(float,float,float)"; //float clamp(float x, float minVal, float maxVal)

        public const string ClampVec2Vec2Vec2 = "clamp(vec2,vec2,vec2)"; //vec2 clamp(vec2 x, vec2 minVal, vec2 maxVal)
        public const string ClampVec3Vec3Vec3 = "clamp(vec3,vec3,vec3)"; //vec3 clamp(vec3 x, vec3 minVal, vec3 maxVal)
        public const string ClampVec4Vec4Vec4 = "clamp(vec4,vec4,vec4)"; //vec4 clamp(vec4 x, vec4 minVal, vec4 maxVal)

        public const string
            ClampVec2FloatFloat = "clamp(vec2,float,float)"; //vec2 clamp(vec2 x, float minVal, float maxVal)

        public const string
            ClampVec3FloatFloat = "clamp(vec3,float,float)"; //vec3 clamp(vec3 x, float minVal, float maxVal)

        public const string
            ClampVec4FlfloatFloat = "clamp(vec4,flfloat,float)"; //vec4 clamp(vec4 x, flfloat minVal, float maxVal)

        public const string MixFloatFloatFloat = "mix(float,float,float)"; //float mix(float x, float y, float a)
        public const string MixVec2Vec2Vec2 = "mix(vec2,vec2,vec2)"; //vec2 mix(vec2 x, vec2 y, vec2 a)
        public const string MixVec3Vec3Vec3 = "mix(vec3,vec3,vec3)"; //vec3 mix(vec3 x, vec3 y, vec3 a)
        public const string MixVec4Vec4Vec4 = "mix(vec4,vec4,vec4)"; //vec4 mix(vec4 x, vec4 y, vec4 a)
        public const string MixVec2Vec2Float = "mix(vec2,vec2,float)"; //vec2 mix(vec2 x, vec2 y, float a)
        public const string MixVec3Vec3Float = "mix(vec3,vec3,float)"; //vec3 mix(vec3 x, vec3 y, float a)
        public const string MixVec4Vec4Float = "mix(vec4,vec4,float)"; //vec4 mix(vec4 x, vec4 y, float a)
        public const string StepFloatFloat = "step(float,float)"; //float step(float edge, float x)
        public const string StepVec2Vec2 = "step(vec2,vec2)"; //vec2 step(vec2 edge, vec2 x)
        public const string StepVec3Vec3 = "step(vec3,vec3)"; //vec3 step(vec3 edge, vec3 x)
        public const string StepVec4Vec4 = "step(vec4,vec4)"; //vec4 step(vec4 edge, vec4 x)
        public const string StepFloatVec2 = "step(float,vec2)"; //vec2 step(float edge, vec2 x)
        public const string StepFloatVec3 = "step(float,vec3)"; //vec3 step(float edge, vec3 x)
        public const string StepFloatVec4 = "step(float,vec4)"; //vec4 step(float edge, vec4 x)

        public const string
            SmoothstepFloatFloatFloat =
                "smoothstep(float,float,float)"; //float smoothstep(float edge0, float edge1, float x)

        public const string
            SmoothstepVec2Vec2Vec2 = "smoothstep(vec2,vec2,vec2)"; //vec2 smoothstep(vec2 edge0, vec2 edge1, vec2 x)

        public const string
            SmoothstepVec3Vec3Vec3 = "smoothstep(vec3,vec3,vec3)"; //vec3 smoothstep(vec3 edge0, vec3 edge1, vec3 x)

        public const string
            SmoothstepVec4Vec4Vec4 = "smoothstep(vec4,vec4,vec4)"; //vec4 smoothstep(vec4 edge0, vec4 edge1, vec4 x)

        public const string
            SmoothstepFloatFloatVec2 =
                "smoothstep(float,float,vec2)"; //vec2 smoothstep(float edge0, float edge1, vec2 x)

        public const string
            SmoothstepFloatFloatVec3 =
                "smoothstep(float,float,vec3)"; //vec3 smoothstep(float edge0, float edge1, vec3 x)

        public const string
            SmoothstepFloatFloatVec4 =
                "smoothstep(float,float,vec4)"; //vec4 smoothstep(float edge0, float edge1, vec4 x)

        public const string GetWorldPos = "getWorldPos";
        public const string GetClipPosVec3 = "getClipPos(vec3)";

        public const string LengthFloat = "length(float)"; //float length(float x)
        public const string LengthVec2 = "length(vec2)"; //float length(vec2 x)
        public const string LengthVec3 = "length(vec3)"; //float length(vec3 x)
        public const string LengthVec4 = "length(vec4)"; //float length(vec4 x)
        public const string DistanceFloatFloat = "distance(float,float)"; //float distance(float p0, float p1)
        public const string DistanceVec2Vec2 = "distance(vec2,vec2)"; //float distance(vec2 p0, vec2 p1)
        public const string DistanceVec3Vec3 = "distance(vec3,vec3)"; //float distance(vec3 p0, vec3 p1)
        public const string DistanceVec4Vec4 = "distance(vec4,vec4)"; //float distance(vec4 p0, vec4 p1)
        public const string DotFloatFloat = "dot(float,float)"; //float dot(float x, float y)
        public const string DotVec2Vec2 = "dot(vec2,vec2)"; //float dot(vec2 x, vec2 y)
        public const string DotVec3Vec3 = "dot(vec3,vec3)"; //float dot(vec3 x, vec3 y)
        public const string DotVec4Vec4 = "dot(vec4,vec4)"; //float dot(vec4 x, vec4 y)
        public const string MultiplyFloatFloat = "float*float";
        public const string MultiplyVec2Vec2 = "vec2*vec2";
        public const string MultiplyVec2Mat2 = "vec2*mat2";
        public const string MultiplyVec3Vec3 = "vec3*vec3";
        public const string MultiplyVec4Vec4 = "vec4*vec4";
        public const string MultiplyVec4Mat4 = "vec4*mat4";
        public const string MultiplyVec4Mat4x3 = "vec4*mat4x3";
        public const string MultiplyMat4x3Float = "mat4x3*float";
        public const string MultiplyMat4Float = "mat4*float";
        public const string MultiplyVec3Mat3 = "vec3*mat3";
        public const string MultiplyMat3Vec3 = "mat3*vec3";
        public const string MultiplyVec2Float = "vec2*float";
        public const string MultiplyVec3Float = "vec3*float";
        public const string MultiplyVec4Float = "vec4*float";
        public const string DivideFloatFloat = "float/float";
        public const string DivideVec2Vec2 = "vec2/vec2";
        public const string DivideVec3Vec3 = "vec3/vec3";
        public const string DivideVec4Vec4 = "vec4/vec4";
        public const string DivideVec2Float = "vec2/float";
        public const string DivideVec3Float = "vec3/float";
        public const string DivideVec4Float = "vec4/float";
        public const string AddFloatFloat = "float+float";
        public const string AddVec2Vec2 = "vec2+vec2";
        public const string AddVec3Vec3 = "vec3+vec3";
        public const string AddVec4Vec4 = "vec4+vec4";
        public const string AddMat4Mat4 = "mat4+mat4";
        public const string AddMat4x3Mat4x3 = "mat4x3+mat4x3";
        public const string SubtractFloatFloat = "float-float";
        public const string SubtractVec2Vec2 = "vec2-vec2";
        public const string SubtractVec3Vec3 = "vec3-vec3";
        public const string SubtractVec4Vec4 = "vec4-vec4";
        public const string MinusFloat = "-float";
        public const string MinusVec2 = "-vec2";
        public const string MinusVec3 = "-vec3";
        public const string MinusVec4 = "-vec4";
        public const string SaturateFloat = "saturate(float)";
        public const string SaturateVec2 = "saturate(vec2)";
        public const string SaturateVec3 = "saturate(vec3)";
        public const string SaturateVec4 = "saturate(vec4)";
        public const string CrossVec3Vec3 = "cross(vec3,vec3)"; //vec3 cross(vec3 x, vec3 y)
        public const string NormalizeFloat = "normalize(float)"; //float normalize(float x)
        public const string NormalizeVec2 = "normalize(vec2)"; //vec2 normalize(vec2 x)
        public const string NormalizeVec3 = "normalize(vec3)"; //vec3 normalize(vec3 x)
        public const string NormalizeVec4 = "normalize(vec4)"; //vec4 normalize(vec4 x)

        public const string
            FaceforwardFloatFloatFloat =
                "faceforward(float,float,float)"; //float faceforward(float N, float I, float Nref)

        public const string
            FaceforwardVec2Vec2Vec2 = "faceforward(vec2,vec2,vec2)"; //vec2 faceforward(vec2 N, vec2 I, vec2 Nref)

        public const string
            FaceforwardVec3Vec3Vec3 = "faceforward(vec3,vec3,vec3)"; //vec3 faceforward(vec3 N, vec3 I, vec3 Nref)

        public const string
            FaceforwardVec4Vec4Vec4 = "faceforward(vec4,vec4,vec4)"; //vec4 faceforward(vec4 N, vec4 I, vec4 Nref)

        public const string ReflectFloatFloat = "reflect(float,float)"; //float reflect(float I, float N)
        public const string ReflectVec2Vec2 = "reflect(vec2,vec2)"; //vec2 reflect(vec2 I, vec2 N)
        public const string ReflectVec3Vec3 = "reflect(vec3,vec3)"; //vec3 reflect(vec3 I, vec3 N)
        public const string ReflectVec4Vec4 = "reflect(vec4,vec4)"; //vec4 reflect(vec4 I, vec4 N)

        public const string
            RefractFloatFloatFloat = "refract(float,float,float)"; //float refract(float I, float N, float eta)

        public const string RefractVec2Vec2Float = "refract(vec2,vec2,float)"; //vec2 refract(vec2 I, vec2 N, float eta)
        public const string RefractVec3Vec3Float = "refract(vec3,vec3,float)"; //vec3 refract(vec3 I, vec3 N, float eta)
        public const string RefractVec4Vec4Float = "refract(vec4,vec4,float)"; //vec4 refract(vec4 I, vec4 N, float eta)
        public const string MatrixCompMultMat2Mat2 = "matrixCompMult(mat2,mat2)"; //mat2 matrixCompMult(mat2 x, mat2 y)
        public const string MatrixCompMultMat3Mat3 = "matrixCompMult(mat3,mat3)"; //mat3 matrixCompMult(mat3 x, mat3 y)
        public const string MatrixCompMultMat4Mat4 = "matrixCompMult(mat4,mat4)"; //mat4 matrixCompMult(mat4 x, mat4 y)
        public const string AndAlso = "andAlso";
        public const string OrElse = "orElse";
        public const string LessThanFloatFloat = "lessThan(float,float)";
        public const string GreaterThanFloatFloat = "greaterThan(float,float)";
        public const string LessThanVec2Vec2 = "lessThan(vec2,vec2)"; //bvec2 lessThan(vec2 x, vec2 y)
        public const string LessThanVec3Vec3 = "lessThan(vec3,vec3)"; //bvec3 lessThan(vec3 x, vec3 y)
        public const string LessThanVec4Vec4 = "lessThan(vec4,vec4)"; //bvec4 lessThan(vec4 x, vec4 y)
        public const string LessThanIvec2Ivec2 = "lessThan(ivec2,ivec2)"; //bvec2 lessThan(ivec2 x, ivec2 y)
        public const string LessThanIvec3Ivec3 = "lessThan(ivec3,ivec3)"; //bvec3 lessThan(ivec3 x, ivec3 y)
        public const string LessThanIvec4Ivec4 = "lessThan(ivec4,ivec4)"; //bvec4 lessThan(ivec4 x, ivec4 y)

        public const string LessThanEqualFloatFloat = "lessThanEqual(float,float)";
        public const string LessThanEqualVec2Vec2 = "lessThanEqual(vec2,vec2)"; //bvec2 lessThanEqual(vec2 x, vec2 y)
        public const string LessThanEqualVec3Vec3 = "lessThanEqual(vec3,vec3)"; //bvec3 lessThanEqual(vec3 x, vec3 y)
        public const string LessThanEqualVec4Vec4 = "lessThanEqual(vec4,vec4)"; //bvec4 lessThanEqual(vec4 x, vec4 y)

        public const string
            LessThanEqualIvec2Ivec2 = "lessThanEqual(ivec2,ivec2)"; //bvec2 lessThanEqual(ivec2 x, ivec2 y)

        public const string
            LessThanEqualIvec3Ivec3 = "lessThanEqual(ivec3,ivec3)"; //bvec3 lessThanEqual(ivec3 x, ivec3 y)

        public const string
            LessThanEqualIvec4Ivec4 = "lessThanEqual(ivec4,ivec4)"; //bvec4 lessThanEqual(ivec4 x, ivec4 y)

        public const string GreaterThanEqualFloatFloat = "greaterThanEqual(float,float)";
        public const string GreaterThanVec2Vec2 = "greaterThan(vec2,vec2)"; //bvec2 greaterThan(vec2 x, vec2 y)
        public const string GreaterThanVec3Vec3 = "greaterThan(vec3,vec3)"; //bvec3 greaterThan(vec3 x, vec3 y)
        public const string GreaterThanVec4Vec4 = "greaterThan(vec4,vec4)"; //bvec4 greaterThan(vec4 x, vec4 y)
        public const string GreaterThanIvec2Ivec2 = "greaterThan(ivec2,ivec2)"; //bvec2 greaterThan(ivec2 x, ivec2 y)
        public const string GreaterThanIvec3Ivec3 = "greaterThan(ivec3,ivec3)"; //bvec3 greaterThan(ivec3 x, ivec3 y)
        public const string GreaterThanIvec4Ivec4 = "greaterThan(ivec4,ivec4)"; //bvec4 greaterThan(ivec4 x, ivec4 y)

        public const string
            GreaterThanEqualVec2Vec2 = "greaterThanEqual(vec2,vec2)"; //bvec2 greaterThanEqual(vec2 x, vec2 y)

        public const string
            GreaterThanEqualVec3Vec3 = "greaterThanEqual(vec3,vec3)"; //bvec3 greaterThanEqual(vec3 x, vec3 y)

        public const string
            GreaterThanEqualVec4Vec4 = "greaterThanEqual(vec4,vec4)"; //bvec4 greaterThanEqual(vec4 x, vec4 y)

        public const string
            GreaterThanEqualIvec2Ivec2 = "greaterThanEqual(ivec2,ivec2)"; //bvec2 greaterThanEqual(ivec2 x, ivec2 y)

        public const string
            GreaterThanEqualIvec3Ivec3 = "greaterThanEqual(ivec3,ivec3)"; //bvec3 greaterThanEqual(ivec3 x, ivec3 y)

        public const string
            GreaterThanEqualIvec4Ivec4 = "greaterThanEqual(ivec4,ivec4)"; //bvec4 greaterThanEqual(ivec4 x, ivec4 y)

        public const string EqualFloatFloat = "equal(float,float)";
        public const string EqualVec2Vec2 = "equal(vec2,vec2)"; //bvec2 equal(vec2 x, vec2 y)
        public const string EqualVec3Vec3 = "equal(vec3,vec3)"; //bvec3 equal(vec3 x, vec3 y)
        public const string EqualVec4Vec4 = "equal(vec4,vec4)"; //bvec4 equal(vec4 x, vec4 y)
        public const string EqualIvec2Ivec2 = "equal(ivec2,ivec2)"; //bvec2 equal(ivec2 x, ivec2 y)
        public const string EqualIvec3Ivec3 = "equal(ivec3,ivec3)"; //bvec3 equal(ivec3 x, ivec3 y)
        public const string EqualIvec4Ivec4 = "equal(ivec4,ivec4)"; //bvec4 equal(ivec4 x, ivec4 y)

        public const string NotEqualFloatFloat = "notEqual(float,float)";
        public const string NotEqualVec2Vec2 = "notEqual(vec2,vec2)"; //bvec2 notEqual(vec2 x, vec2 y)
        public const string NotEqualVec3Vec3 = "notEqual(vec3,vec3)"; //bvec3 notEqual(vec3 x, vec3 y)
        public const string NotEqualVec4Vec4 = "notEqual(vec4,vec4)"; //bvec4 notEqual(vec4 x, vec4 y)
        public const string NotEqualIvec2Ivec2 = "notEqual(ivec2,ivec2)"; //bvec2 notEqual(ivec2 x, ivec2 y)
        public const string NotEqualIvec3Ivec3 = "notEqual(ivec3,ivec3)"; //bvec3 notEqual(ivec3 x, ivec3 y)
        public const string NotEqualIvec4Ivec4 = "notEqual(ivec4,ivec4)"; //bvec4 notEqual(ivec4 x, ivec4 y)

        public const string AnyBvec2 = "any(bvec2)"; //bool any(bvec2 x)
        public const string AnyBvec3 = "any(bvec3)"; //bool any(bvec3 x)
        public const string AnyBvec4 = "any(bvec4)"; //bool any(bvec4 x)
        public const string AllBvec2 = "all(bvec2)"; //bool all(bvec2 x)
        public const string AllBvec3 = "all(bvec3)"; //bool all(bvec3 x)
        public const string AllBvec4 = "all(bvec4)"; //bool all(bvec4 x)
        public const string NotBool = "not(bool)";
        public const string NotBvec2 = "not(bvec2)"; //bvec2 not(bvec2 x)
        public const string NotBvec3 = "not(bvec3)"; //bvec3 not(bvec3 x)
        public const string NotBvec4 = "not(bvec4)"; //bvec4 not(bvec4 x)

        public const string
            Texture2DSampler2DVec2 = "texture2D(sampler2D,vec2)"; //vec4 texture2D(sampler2D sampler, vec2 coord)

        public const string
            Texture2DSampler2DVec2Float =
                "texture2D(sampler2D,vec2,float)"; //vec4 texture2D(sampler2D sampler, vec2 coord, float bias)

        public const string
            TextureCubeSamplerCubeVec3 =
                "textureCube(samplerCube,vec3)"; //vec4 textureCube(samplerCube sampler, vec3 coord)

        public const string
            TextureCubeSamplerCubeVec3Float =
                "textureCube(samplerCube,vec3,float)"; //vec4 textureCube(samplerCube sampler, vec3 coord, float bias)

        public const string PerPixelFloat = "perPixelFloat"; //
        public const string PerPixelVec2 = "perPixelVec2"; //
        public const string PerPixelVec3 = "perPixelVec3"; //
        public const string PerPixelVec4 = "perPixelVec4"; //

        public const string BreakVec2 = "breakVec2"; //
        public const string BreakVec3 = "breakVec3"; //
        public const string BreakVec4 = "breakVec4"; //
        public const string BreakVec4ToVec3AndFloat = "breakVec4toVec3Float"; //
        public const string BreakVec3ToVec2AndFloat = "breakVec3toVec2Float"; //
        public const string BreakVec4ToVec2AndVec2 = "breakVec4toVec2Vec2"; //
        public const string BreakVec4ToVec2AndFloats = "breakVec4toVec2FloatFloat"; //
        public const string MakeVec2 = "makeVec2"; //
        public const string MakeVec3 = "makeVec3"; //
        public const string MakeVec4 = "makeVec4"; //
        public const string MakeMat2 = "makeMat2"; //
        public const string MakeVec4FromVec3 = "makeVec4fromVec3"; //
        public const string MakeVec4FromVec3AndFloat = "makeVec4fromVec3Float"; //
        public const string MakeVec3FromVec2AndFloat = "makeVec3fromVec2Float"; //
        public const string MakeVec4FromVec2AndVec2 = "makeVec4fromVec2Vec2"; //
        public const string MakeMat3FromMat4 = "mat3(mat4)"; //
        public const string MakeMat3FromVec3Vec3Vec3 = "mat3(vec3,vec3,vec3)"; //
        public const string MakeMat3FromMat4x3 = "mat3(mat4x3)"; //
        public const string MakeMat4x3FromVec4Vec4Vec4 = "mat4x3(vec4,vec4,vec4)"; //

        public const string TernaryFloat = "select(float)"; //
        public const string TernaryVec2 = "select(vec2)"; //
        public const string TernaryVec3 = "select(vec3)"; //
        public const string TernaryVec4 = "select(vec4)"; //

        public const string SkinMatrixIndex = PinTypes.Skin + "[int]"; //
        public const string VertexLightsIndex = PinTypes.VertexLights + "[int]"; //
        public const string LightMatricesIndex = PinTypes.LightMatrices + "[int]"; //

        public const string ConnectorPrefix = "connect"; //
        public const string AttributePrefix = "attribute"; //
        public const string UniformPrefix = "uniform"; //
        public const string ParameterPrefix = "parameter"; //

        internal static Dictionary<string, string> _connectors;
        internal static Dictionary<string, string> _ifdefs;
        internal static Dictionary<string, string> _attributes;
        internal static Dictionary<string, string> _uniforms;
        internal static Dictionary<string, string> _parameters;
        internal static Dictionary<string, string> _constants;

        static NodeTypes()
        {
            _connectors = PinTypes.DataTypes.Concat(PinTypes.SamplerTypes)
                .Select(_ => Tuple.Create(_, MakeType(ConnectorPrefix, _))).ToDictionary(_ => _.Item2, _ => _.Item1);
            _ifdefs = PinTypes.DataTypes.Select(_ => Tuple.Create(_, MakeType(IfDefPrefix, _)))
                .ToDictionary(_ => _.Item2, _ => _.Item1);
            _attributes = PinTypes.DataTypesExceptArrays.Select(_ => Tuple.Create(_, MakeType(AttributePrefix, _)))
                .ToDictionary(_ => _.Item2, _ => _.Item1);
            _uniforms = PinTypes.DataTypes.Select(_ => Tuple.Create(_, MakeType(UniformPrefix, _)))
                .ToDictionary(_ => _.Item2, _ => _.Item1);
            _parameters = PinTypes.DataTypesExceptArrays.Concat(PinTypes.Special.Color)
                .Select(_ => Tuple.Create(_, MakeType(ParameterPrefix, _))).ToDictionary(_ => _.Item2, _ => _.Item1);
            _constants = PinTypes.DataTypesExceptArrays.Concat(PinTypes.Special.Color).Select(_ => Tuple.Create(_, _))
                .ToDictionary(_ => _.Item2, _ => _.Item1);
        }

        private static IEnumerable<string> Concat(this IEnumerable<string> source, string val)
        {
            foreach (var entry in source) yield return entry;
            yield return val;
        }

        public static bool IsSampler(string type)
        {
            switch (type)
            {
                case Sampler2D:
                case SamplerCube:
                    return true;
            }

            return false;
        }

        public static bool IsUniform(string type)
        {
            if (_uniforms.ContainsKey(type))
                return true;
            switch (type)
            {
                case LightData:
                case ObjectData:
                case ZoneData:
                case CameraData:
                    return true;
            }

            return false;
        }

        public static bool IsParameter(string type)
        {
            return _parameters.ContainsKey(type);
        }

        public static bool IsAttribute(string type)
        {
            if (_attributes.ContainsKey(type))
                return true;
            switch (type)
            {
                case VertexData:
                    //public const string ColorAttribute = "сolorAttribute";
                    //public const string Vector4Attribute = "vector4Attribute";
                    //public const string Vector3Attribute = "vector3Attribute";
                    //public const string Vector2Attribute = "vector2Attribute";
                    //public const string FloatAttribute = "floatAttribute";
                    return true;
            }

            return false;
        }

        public static bool IsConstant(string type)
        {
            return _constants.ContainsKey(type);
        }

        public static bool IsOutput(string nodeType)
        {
            switch (nodeType)
            {
                case AmbientColor:
                case DeferredOutput:
                case LightColor:
                case PositionOutput:
                case Discard:
                    return true;
            }

            return false;
        }

        public static bool IsConnectorType(string nodeType)
        {
            return _connectors.ContainsKey(nodeType);
        }

        public static bool IsIfDefType(string nodeType)
        {
            return _ifdefs.ContainsKey(nodeType);
        }

        public static bool IsFinalColor(string nodeType)
        {
            switch (nodeType)
            {
                case Special.FinalColor:
                case Special.FragData0:
                case Special.FragData1:
                case Special.FragData2:
                case Special.FragData3:
                case Special.ShadowMapOutput:
                    return true;
            }

            return false;
        }

        public static string MakeType(string prefix, string type)
        {
            var sb = new StringBuilder();
            sb.Append(prefix);
            sb.Append("(");
            sb.Append(type);
            sb.Append(")");
            return sb.ToString();
        }

        public static class SubCategories
        {
            public const string Logic = "Logic";
            public const string Arithmetic = "Arithmetic";
            public const string Trigonometry = "Trigonometry";
        }

        public static class Categories
        {
            public const string Preprocessor = "Preprocessor";
            public const string Functions = "Functions";
            public const string Connectors = "Connectors";
            public const string Data = "Data";
        }

        public static class Special
        {
            public const string SetVarying = "setVarying";
            public const string GetVarying = "getVarying";
            public const string Variable = "var";
            public const string ShadowMapOutput = "shadowMapOutput";
            public const string FinalColor = "finalColor";
            public const string FragData0 = "finalData[0]";
            public const string FragData1 = "finalData[1]";
            public const string FragData2 = "finalData[2]";
            public const string FragData3 = "finalData[3]";

            public const string Default = "default";
        }
    }
}
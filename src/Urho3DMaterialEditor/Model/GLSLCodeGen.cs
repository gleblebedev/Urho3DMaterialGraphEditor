using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReactiveUI;
using Toe.Scripting.Helpers;
using Urho3DMaterialEditor.Model.Templates;

namespace Urho3DMaterialEditor.Model
{
    public class GLSLCodeGen : ICodeGen
    {
        public const string GetNormalMatrix = "GetNormalMatrix";
        private readonly IT4Writer _writer;

        public string _getNormalMatrix = @"
mat3 GetNormalMatrix(mat4 modelMatrix)
{
    return mat3(modelMatrix[0].xyz, modelMatrix[1].xyz, modelMatrix[2].xyz);
}
";

        private int _ifDefCount;
        public Dictionary<NodeHelper<TranslatedMaterialGraph.NodeInfo>, string> _variables;

        public GLSLCodeGen(IT4Writer writer)
        {
            _writer = writer;
            _variables = new Dictionary<NodeHelper<TranslatedMaterialGraph.NodeInfo>, string>();
        }

        public IEnumerable<RequiredFunction> GetRequiredFunctions(RequiredFunction requiredFunction)
        {
            yield break;
        }

        public IEnumerable<RequiredFunction> GetRequiredFunctions(NodeHelper<TranslatedMaterialGraph.NodeInfo> node)
        {
            switch (node.Type)
            {
                case NodeTypes.MakeMat3FromMat4x3:
                case NodeTypes.MakeMat3FromMat4:
                    yield return new RequiredFunction(GetNormalMatrix);
                    break;
            }
        }

        public IEnumerable<NodeHelper<TranslatedMaterialGraph.NodeInfo>> GetRequiredUniforms(
            RequiredFunction requiredFunction)
        {
            switch (requiredFunction.Name)
            {
            }

            yield break;
        }

        public IEnumerable<NodeHelper<TranslatedMaterialGraph.NodeInfo>> GetRequiredUniforms(
            NodeHelper<TranslatedMaterialGraph.NodeInfo> node)
        {
            switch (node.Type)
            {
                case NodeTypes.PositionOutput:
                    yield return new NodeHelper<TranslatedMaterialGraph.NodeInfo>
                    {
                        Name = "cClipPlane",
                        Type = PinTypes.Vec4
                    };
                    break;
            }
        }

        public string GetFunction(RequiredFunction function)
        {
            switch (function.Name)
            {
                case GetNormalMatrix:
                    return _getNormalMatrix;
                default:
                    throw new NotImplementedException();
            }
        }

        public string GenerateCode(PinHelper<TranslatedMaterialGraph.NodeInfo> pin)
        {
            if (pin.Links.Count == 0)
                return GenerateDefaultValue(pin.Type);
            return GenerateCode(pin.Links[0].From.Node);
        }

        private string GenerateDefaultValue(string pinType)
        {
            switch (pinType)
            {
                case PinTypes.Ivec4:
                    return "ivec4(0,0,0,0)";
                case PinTypes.Ivec3:
                    return "ivec3(0,0,0)";
                case PinTypes.Ivec2:
                    return "ivec2(0,0)";
                case PinTypes.Int:
                    return "(0)";
                case PinTypes.Mat2:
                    return "mat2(1.0,0.0, 0.0,1.0)";
                case PinTypes.Mat3:
                    return "mat3(1.0,0.0,0.0, 0.0,1.0,0.0, 0.0,0.0,1.0)";
                case PinTypes.Mat4x3:
                case PinTypes.Mat4:
                    return "mat4(1.0,0.0,0.0,0.0, 0.0,1.0,0.0,0.0, 0.0,0.0,1.0,0.0, 0.0,0.0,0.0,1.0)";
                case PinTypes.Vec4:
                    return "vec4(0.0,0.0,0.0,0.0)";
                case PinTypes.Vec3:
                    return "vec3(0.0,0.0,0.0)";
                case PinTypes.Vec2:
                    return "vec2(0.0,0.0)";
                case PinTypes.Float:
                    return "(0.0)";
                case PinTypes.Bvec4:
                    return "bvec4(false,false,false,false)";
                case PinTypes.Bvec3:
                    return "bvec3(false,false,false)";
                case PinTypes.Bvec2:
                    return "bvec2(false,false)";
                case PinTypes.Bool:
                    return "false";
                case PinTypes.VertexLights:
                    return "vec4[12]";
                case PinTypes.LightMatrices:
                    return "mat4[4]";
            }

            throw new NotImplementedException("Can't generate a default value for type " + pinType);
        }

        public string GenerateIfDef(NodeHelper<TranslatedMaterialGraph.NodeInfo> node)
        {
            ++_ifDefCount;
            var ifDefName = "ifdef" + _ifDefCount;
            var a = GenerateCode(node.InputPins[0]);
            var b = GenerateCode(node.InputPins[1]);
            var pinTypeName = node.OutputPins[0].Type;
            var actualType = GetType(pinTypeName);

            {
                _writer.WriteLine(node.Extra.Define.IfDefExpression,
                    "// based on node " + node.Name + " " + node.Value + " (type:" + node.Type + ", id:" + node.Id +
                    ")");
                _writer.WriteLine(node.Extra.Define.IfDefExpression, actualType + " " + ifDefName + " = " + a + ";");
            }
            {
                _writer.WriteLine(node.Extra.Define.IfNotDefExpression,
                    "// based on node " + node.Name + " " + node.Value + " (type:" + node.Type + ", id:" + node.Id +
                    ")");
                _writer.WriteLine(node.Extra.Define.IfNotDefExpression, actualType + " " + ifDefName + " = " + b + ";");
            }
            return ifDefName;
        }

        public string GenerateVarCode(NodeHelper<TranslatedMaterialGraph.NodeInfo> node)
        {
            string name;
            if (_variables.TryGetValue(node, out name))
                return name;
            name = node.Name;
            _variables.Add(node, name);
            var pinTypeName = node.InputPins[0].Type;
            var actualType = GetType(pinTypeName);

            if (node.InputPins[0].Links.Count != 1)
                throw new MaterialCompilationException("Var node should have exactly one input node connected",
                    node.Id);
            var connectedNode = node.InputPins[0].ConnectedPins.First().Node;
            {
                var arg = GenerateCode(node.InputPins[0]);
                var def = node.Extra.Define;
                _writer.WriteLine(def.IsAlways ? null : def.Expression,
                    "// based on node " + connectedNode.Name + " " + node.Value + " (type:" + connectedNode.Type +
                    ", id:" + connectedNode.Id + "), cost estimation: "+node.Extra.EstimatedCost);
                _writer.WriteLine(def.IsAlways ? null : def.Expression,
                    actualType + " " + node.Name + " = " + arg + ";");
            }
            return name;
        }

        public string GenerateCode(NodeHelper<TranslatedMaterialGraph.NodeInfo> node)
        {
            if (node.Type == NodeTypes.Special.Variable) return GenerateVarCode(node);
            if (node.Extra.Define.IsIfDef) return GenerateIfDef(node);
            //if (node.Type == NodeTypes.Texture2DSampler2DVec2 
            //    || node.Type == NodeTypes.Texture2DSampler2DVec2Float
            //    || node.Type == NodeTypes.TextureCubeSamplerCubeVec3
            //    || node.Type == NodeTypes.TextureCubeSamplerCubeVec3Float
            //    )
            //{
            //    if (node.InputPins["sampler"].Links.Count == 0)
            //    {
            //        return "vec4(0.0,0.0,0.0,0.0)";
            //    }
            //}
            var args = node.InputPins.Select(_ => GenerateCode(_)).ToList();
            if (NodeTypes.IsConnectorType(node.Type)) return args[0];
            if (NodeTypes.IsIfDefType(node.Type))
                throw new InvalidOperationException("ifdef should be handeled at var code path");

            if (node.Type == NodeTypes.VertexData) return "i" + node.OutputPins[0].Id;

            if (NodeTypes.IsAttribute(node.Type))
            {
                if (node.Name == "BlendIndices")
                    return "ivec4(i" + node.Name + ")";
                return "i" + node.Name;
            }

            if (NodeTypes.IsUniform(node.Type)) return "c" + node.Name;
            if (NodeTypes.IsParameter(node.Type)) return "c" + node.Name;

            string constType;
            if (NodeTypes._constants.TryGetValue(node.Type, out constType))
                return GetConstantValue(node.Value, constType);
            switch (node.Type)
            {
                case NodeTypes.Special.Default:
                    return GenerateDefaultValue(node.OutputPins[0].Type);
                case NodeTypes.Special.ShadowMapOutput:
                    _writer.WriteLine(null, "    #ifdef VSM_SHADOW");
                    _writer.WriteLine(null, "        float depth = " + args[0] + ".x / " + args[0] + ".y * 0.5 + 0.5;");
                    _writer.WriteLine(null, "        gl_FragColor = vec4(depth, depth * depth, 1.0, 1.0);");
                    _writer.WriteLine(null, "    #else");
                    _writer.WriteLine(null, "        gl_FragColor = vec4(1.0);");
                    _writer.WriteLine(null, "    #endif");
                    return null;
                case NodeTypes.Special.FinalColor:
                    _writer.WriteLine(null, "gl_FragColor = " + args[0] + ";");
                    return null;
                case NodeTypes.Special.FragData0:
                    _writer.WriteLine(null, "gl_FragData[0] = " + args[0] + ";");
                    return null;
                case NodeTypes.Special.FragData1:
                    _writer.WriteLine(null, "gl_FragData[1] = " + args[0] + ";");
                    return null;
                case NodeTypes.Special.FragData2:
                    _writer.WriteLine(null, "gl_FragData[2] = " + args[0] + ";");
                    return null;
                case NodeTypes.Special.FragData3:
                    _writer.WriteLine(null, "gl_FragData[3] = " + args[0] + ";");
                    return null;
                case NodeTypes.FragCoord:
                    return "gl_FragCoord";
                case NodeTypes.FrontFacing:
                    return "gl_FrontFacing";
                case NodeTypes.LightColor:
                case NodeTypes.Opacity:
                case NodeTypes.AmbientColor:
                case NodeTypes.RefractionColor:
                case NodeTypes.PerPixelFloat:
                case NodeTypes.PerPixelVec2:
                case NodeTypes.PerPixelVec3:
                case NodeTypes.PerPixelVec4:
                    return args[0];
                case NodeTypes.ObjectData:
                case NodeTypes.LightData:
                case NodeTypes.CameraData:
                case NodeTypes.ZoneData:
                    throw new InvalidOperationException(
                        "This code shouldn't be executed. All uniforms must be split into individual nodes on preprocessing.");

                case NodeTypes.Special.SetVarying:
                {
                    var arg = args[0];
                    var actualType = GetType(node.InputPins[0].Type);
                    var varyingType = GetVaryingType(node.InputPins[0].Type);
                    if (actualType != varyingType)
                        arg = varyingType + "(" + arg + ")";
                    var connectedNode = node.InputPins[0].ConnectedPins.FirstOrDefault()?.Node;
                    if (connectedNode != null)
                    {
                        _writer.WriteLine(null,
                            "// based on node " + connectedNode.Name + " " + connectedNode.Value + " (type:" + connectedNode.Type +
                            ", id:" + connectedNode.Id + "), cost estimation: " + connectedNode.Extra.EstimatedCost);
                    }

                    _writer.WriteLine(node.Extra.Define.Expression , "v" + node.Value + " = " + arg + ";");
                }
                    return null;
                case NodeTypes.Discard:
                    _writer.WriteLine(null, "if (" + args[0] + ") discard;");
                    return null;
                case NodeTypes.Special.GetVarying:
                {
                    var arg = "v" + node.Value;
                    var actualType = GetType(node.OutputPins[0].Type);
                    var varyingType = GetVaryingType(node.OutputPins[0].Type);
                    if (actualType != varyingType)
                        arg = actualType + "(" + arg + ")";
                    return arg;
                }
                case NodeTypes.Sampler2D:
                case NodeTypes.Sampler3D:
                case NodeTypes.SamplerCube:
                    return GetSamplerName(node);
                //_writer.WriteLine("uniform "+node.OutputPins.First().Type+" s" + GetSamplerName(node) + ";");
                case NodeTypes.BreakVec4ToVec3AndFloat:
                case NodeTypes.BreakVec3ToVec2AndFloat:
                case NodeTypes.BreakVec4ToVec2AndVec2:
                case NodeTypes.BreakVec4ToVec2AndFloats:
                case NodeTypes.BreakVec2:
                case NodeTypes.BreakVec3:
                case NodeTypes.BreakVec4:
                    if (node.OutputPins.Count != 1)
                        throw new InvalidOperationException(node + " expected to have a single output.");
                    return args[0] + "." + node.OutputPins[0].Id.ToLower();
                case NodeTypes.MultiplyFloatFloat:
                case NodeTypes.MultiplyVec2Float:
                case NodeTypes.MultiplyVec3Float:
                case NodeTypes.MultiplyVec4Float:
                case NodeTypes.MultiplyVec2Vec2:
                case NodeTypes.MultiplyVec2Mat2:
                case NodeTypes.MultiplyVec3Vec3:
                case NodeTypes.MultiplyVec4Vec4:
                case NodeTypes.MultiplyVec4Mat4:
                case NodeTypes.MultiplyVec3Mat3:
                case NodeTypes.MultiplyMat3Vec3:
                case NodeTypes.MultiplyMat4x3Float:
                case NodeTypes.MultiplyMat4Float:
                    return GenBinaryOperator("*", args);
                case NodeTypes.MultiplyVec4Mat4x3:
                    return "(" + GenBinaryOperator("*", args) + ").xyz";
                //case NodeTypes.MultiplyVec3Mat4:
                //case NodeTypes.MultiplyVec3Mat4x3:
                //    return GenBinaryOperator("*", new []{ "vec4("+args[0]+", 1.0)", args[1] });
                case NodeTypes.AddFloatFloat:
                case NodeTypes.AddVec2Vec2:
                case NodeTypes.AddVec3Vec3:
                case NodeTypes.AddVec4Vec4:
                case NodeTypes.AddMat4Mat4:
                case NodeTypes.AddMat4x3Mat4x3:
                    return GenBinaryOperator("+", args);
                case NodeTypes.SubtractFloatFloat:
                case NodeTypes.SubtractVec2Vec2:
                case NodeTypes.SubtractVec3Vec3:
                case NodeTypes.SubtractVec4Vec4:
                    return GenBinaryOperator("-", args);
                case NodeTypes.MinusFloat:
                case NodeTypes.MinusVec2:
                case NodeTypes.MinusVec3:
                case NodeTypes.MinusVec4:
                    return GenUnaryOperator("-", args);
                case NodeTypes.SaturateFloat:
                    return "clamp(" + args[0] + ", 0.0, 1.0)";
                case NodeTypes.SaturateVec2:
                    return "clamp(" + args[0] + ", vec2(0.0, 0.0), vec2(1.0, 1.0))";
                case NodeTypes.SaturateVec3:
                    return "clamp(" + args[0] + ", vec3(0.0, 0.0, 0.0), vec3(1.0, 1.0, 1.0))";
                case NodeTypes.SaturateVec4:
                    return "clamp(" + args[0] + ", vec4(0.0, 0.0, 0.0, 0.0), vec4(1.0, 1.0, 1.0, 1.0))";
                case NodeTypes.DivideFloatFloat:
                case NodeTypes.DivideVec2Float:
                case NodeTypes.DivideVec3Float:
                case NodeTypes.DivideVec4Float:
                case NodeTypes.DivideVec2Vec2:
                case NodeTypes.DivideVec3Vec3:
                case NodeTypes.DivideVec4Vec4:
                    return GenBinaryOperator("/", args);
                case NodeTypes.TernaryFloat:
                case NodeTypes.TernaryVec2:
                case NodeTypes.TernaryVec3:
                case NodeTypes.TernaryVec4:
                    return "((" + args[0] + ")?(" + args[1] + "):(" + args[2] + "))";
                case NodeTypes.MakeVec4FromVec3:
                    return node.OutputPins[0].Type + "(" + string.Join(", ", args) + ", 1.0)";
                case NodeTypes.MakeMat4x3FromVec4Vec4Vec4:
                    return "mat4(" + string.Join(", ", args) + ", vec4(0.0, 0.0, 0.0, 1.0))";
                case NodeTypes.MakeVec4FromVec3AndFloat:
                case NodeTypes.MakeVec4FromVec2AndVec2:
                case NodeTypes.MakeVec3FromVec2AndFloat:
                case NodeTypes.MakeVec2:
                case NodeTypes.MakeVec3:
                case NodeTypes.MakeVec4:
                case NodeTypes.MakeMat2:
                case NodeTypes.MakeMat3FromVec3Vec3Vec3:
                    return node.OutputPins[0].Type + "(" + string.Join(", ", args) + ")";
                case NodeTypes.MakeMat3FromMat4:
                case NodeTypes.MakeMat3FromMat4x3:
                    return "GetNormalMatrix(" + string.Join(", ", args) + ")";
                case NodeTypes.SampleVSMShadow:
                    return "texture2D(" + string.Join(", ", args) + ").rg";
                case NodeTypes.SkinMatrixIndex:
                    return "mat4(" + args[0] + "[" + args[1] + "*3], " +
                           args[0] + "[" + args[1] + "*3+1]," +
                           args[0] + "[" + args[1] + "*3+2], vec4(0.0, 0.0, 0.0, 1.0))";
                case NodeTypes.LightMatricesIndex:
                case NodeTypes.VertexLightsIndex:
                    return args[0] + "[" + args[1] + "]";
                case NodeTypes.PositionOutput:
                    return args[0];
                case NodeTypes.LessThanFloatFloat:
                    return GenBinaryOperator("<", args);
                case NodeTypes.NotBool:
                    return "!("+args[0]+")";
                case NodeTypes.EqualFloatFloat:
                    return GenBinaryOperator("==", args);
                case NodeTypes.NotEqualFloatFloat:
                    return GenBinaryOperator("!=", args);
                case NodeTypes.GreaterThanFloatFloat:
                    return GenBinaryOperator(">", args);
                case NodeTypes.LessThanEqualFloatFloat:
                    return GenBinaryOperator("<=", args);
                case NodeTypes.GreaterThanEqualFloatFloat:
                    return GenBinaryOperator(">=", args);
                case NodeTypes.AndAlso:
                    return GenBinaryOperator("&&", args);
                case NodeTypes.OrElse:
                    return GenBinaryOperator("||", args);
                default:
                    var bracketIndex = node.Type.IndexOf("(");
                    if (bracketIndex > 0)
                        return node.Type.Substring(0, bracketIndex) + "(" + string.Join(", ", args) + ")";
                    throw new MaterialCompilationException("Node " + node.Type + " isn't implemented yet.", node.Id);
            }
        }

        private string GetConstantValue(string value, string pinType)
        {
            if (pinType == PinTypes.Special.Color)
                pinType = PinTypes.Vec4;
            switch (pinType)
            {
                case PinTypes.Float:
                case PinTypes.Bool:
                case PinTypes.Int:
                    return value;
                case PinTypes.Ivec2:
                case PinTypes.Ivec3:
                case PinTypes.Ivec4:
                case PinTypes.Bvec2:
                case PinTypes.Bvec3:
                case PinTypes.Bvec4:
                case PinTypes.Vec2:
                case PinTypes.Vec3:
                case PinTypes.Vec4:
                    return pinType + "(" + string.Join(", ",
                               (value ?? "").Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries)) + ")";
                case PinTypes.Mat2:
                    return FormatMat(value, 2);
                case PinTypes.Mat3:
                    return FormatMat(value, 3);
                case PinTypes.Mat4:
                    return FormatMat(value, 4);
            }

            throw new NotImplementedException();
        }

        private string FormatMat(string nodeValue, int dimSize)
        {
            var vals = (nodeValue ?? "").Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            var sb = new StringBuilder();
            sb.Append("mat4(");
            var sep = "";
            for (var c = 0; c < dimSize; ++c)
            for (var r = 0; r < dimSize; ++r)
            {
                sb.Append(sep);
                sep = ", ";
                var index = c + r * dimSize;
                if (index >= vals.Length)
                    sb.Append("0.0");
                else
                    sb.Append(vals[index]);
            }

            sb.Append(")");
            return sb.ToString();
        }

        private string GenBinaryOperator(string p0, IList<string> args)
        {
            return "(" + args[0] + " " + p0 + " " + args[1] + ")";
        }

        private string GenUnaryOperator(string p0, IList<string> args)
        {
            return "(" + p0 + " " + args[0] + ")";
        }

        public static string GetType(NodeHelper<TranslatedMaterialGraph.NodeInfo> node)
        {
            return GetType(node.OutputPins.First());
        }

        public static string GetType(PinHelper<TranslatedMaterialGraph.NodeInfo> pin)
        {
            return GetType(pin.Type);
        }

        public static string GetType(string pinTypeName)
        {
            switch (pinTypeName)
            {
                case PinTypes.Sampler2D:
                    return "sampler2D";
                //case PinTypes.Sampler3D:
                //    return "sampler3D";
                case PinTypes.SamplerCube:
                    return "samplerCube";
                case PinTypes.VertexLights:
                    return "vec4";
                case PinTypes.LightMatrices:
                    return "mat4";
                case PinTypes.Mat4x3:
                    return "mat4";
                case PinTypes.Special.Color:
                    return "vec4";
                case PinTypes.Skin:
                    return "vec4";
            }

            return pinTypeName;
        }

        public static string GetVaryingType(string pinTypeName)
        {
            switch (pinTypeName)
            {
                case PinTypes.Bool:
                    return PinTypes.Float;
                case PinTypes.Bvec2:
                    return PinTypes.Vec2;
                case PinTypes.Bvec3:
                    return PinTypes.Vec3;
                case PinTypes.Bvec4:
                    return PinTypes.Vec4;
                case PinTypes.Ivec2:
                    return PinTypes.Vec2;
                case PinTypes.Ivec3:
                    return PinTypes.Vec3;
                case PinTypes.Ivec4:
                    return PinTypes.Vec4;
            }

            return GetType(pinTypeName);
        }

        public static object GetArraySize(string type)
        {
            switch (type)
            {
                case PinTypes.Skin:
                    return "[MAXBONES*3]";
                case PinTypes.VertexLights:
                    return "[12]";
                case PinTypes.LightMatrices:
                    return "[4]";
            }

            return "";
        }

        public static string GetSamplerName(NodeHelper<TranslatedMaterialGraph.NodeInfo> node)
        {
            switch (node.Name)
            {
                case SamplerNodeFactory.Diffuse:
                    return "sDiffMap";
                case SamplerNodeFactory.DiffuseCubeMap:
                    return "sDiffCubeMap";
                case SamplerNodeFactory.Normal:
                    return "sNormalMap";
                case SamplerNodeFactory.Specular:
                    return "sSpecMap";
                case SamplerNodeFactory.Emissive:
                    return "sEmissiveMap";
                case SamplerNodeFactory.Environment:
                    return "sEnvMap";
                case SamplerNodeFactory.EnvironmentCubeMap:
                    return "sEnvCubeMap";
                case SamplerNodeFactory.Screen:
                    return "sEnvMap";
                case SamplerNodeFactory.LightRampMap:
                    return "sLightRampMap";
                case SamplerNodeFactory.ShadowMap:
                    return "sShadowMap";
                case SamplerNodeFactory.FaceSelectCubeMap:
                    return "sFaceSelectCubeMap";
                case SamplerNodeFactory.IndirectionCubeMap:
                    return "sIndirectionCubeMap";
                case SamplerNodeFactory.ZoneCubeMap:
                    return "sZoneCubeMap";
                /*
uniform sampler2D sLightRampMap;
uniform sampler2D sLightSpotMap;
uniform samplerCube sLightCubeMap;
#ifndef GL_ES
    uniform sampler3D sVolumeMap;
    uniform sampler2D sAlbedoBuffer;
    uniform sampler2D sNormalBuffer;
    uniform sampler2D sDepthBuffer;
    uniform sampler2D sLightBuffer;
    #ifdef VSM_SHADOW
        uniform sampler2D sShadowMap;
    #else
        uniform sampler2DShadow sShadowMap;
    #endif
    uniform samplerCube sFaceSelectCubeMap;
    uniform samplerCube sIndirectionCubeMap;
    uniform samplerCube sZoneCubeMap;
    uniform sampler3D sZoneVolumeMap;
#else
    uniform highp sampler2D sShadowMap;

                 */
                default:
                    throw new NotImplementedException(node.ToString());
            }
        }
    }
}
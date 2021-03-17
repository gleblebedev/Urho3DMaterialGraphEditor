using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Toe.Scripting;

namespace Urho3DMaterialEditor.Model
{
    public class MaterialNodeRegistry : AbstractNodeRegistry
    {
        public static readonly MaterialNodeRegistry Instance = new MaterialNodeRegistry();
        private readonly ILookup<string, INodeFactory> _factoryByType;
        private readonly List<InlineFunctionNodeFactory> _inlineFunctions = new List<InlineFunctionNodeFactory>();

        public MaterialNodeRegistry()
        {
            Add(new SamplerNodeFactory(SamplerNodeFactory.Diffuse, NodeTypes.Sampler2D));
            Add(new SamplerNodeFactory(SamplerNodeFactory.DiffuseCubeMap, NodeTypes.SamplerCube));
            Add(new SamplerNodeFactory(SamplerNodeFactory.Normal, NodeTypes.Sampler2D));
            Add(new SamplerNodeFactory(SamplerNodeFactory.Specular, NodeTypes.Sampler2D));
            Add(new SamplerNodeFactory(SamplerNodeFactory.Emissive, NodeTypes.Sampler2D));
            Add(new SamplerNodeFactory(SamplerNodeFactory.Environment, NodeTypes.Sampler2D));
            Add(new SamplerNodeFactory(SamplerNodeFactory.EnvironmentCubeMap, NodeTypes.SamplerCube));
            Add(new SamplerNodeFactory(SamplerNodeFactory.Screen, NodeTypes.Sampler2D));

#if DEBUG
            Add(new SamplerNodeFactory(SamplerNodeFactory.ShadowMap, NodeTypes.Sampler2D));
            Add(new SamplerNodeFactory(SamplerNodeFactory.LightRampMap, NodeTypes.Sampler2D));
            Add(new SamplerNodeFactory(SamplerNodeFactory.FaceSelectCubeMap, NodeTypes.SamplerCube));
            Add(new SamplerNodeFactory(SamplerNodeFactory.IndirectionCubeMap, NodeTypes.SamplerCube));
#endif
            Add(new SamplerNodeFactory(SamplerNodeFactory.ZoneCubeMap, NodeTypes.SamplerCube));


            Add(new ParameterNodeFactory(PinTypes.Vec4, NodeCategory.Parameter, "UOffset", "1.0 0.0 0.0 1.0"));
            Add(new ParameterNodeFactory(PinTypes.Vec4, NodeCategory.Parameter, "VOffset", "0.0 1.0 0.0 1.0"));

            Add(new ConstantNodeFactory(PinTypes.Special.Color, NodeCategory.Value, PinTypes.Special.Color));
            Add(new ConstantNodeFactory(PinTypes.Float, NodeCategory.Value, "float"));
            Add(new ConstantNodeFactory(PinTypes.Vec4, NodeCategory.Value, "vec4"));
            Add(new ConstantNodeFactory(PinTypes.Vec3, NodeCategory.Value, "vec3"));
            Add(new ConstantNodeFactory(PinTypes.Vec2, NodeCategory.Value, "vec2"));

            Add(new ConstantNodeFactory(PinTypes.Bool, NodeCategory.Value, "bool"));
            Add(new ConstantNodeFactory(PinTypes.Bvec4, NodeCategory.Value, "bvec4"));
            Add(new ConstantNodeFactory(PinTypes.Bvec3, NodeCategory.Value, "bvec3"));
            Add(new ConstantNodeFactory(PinTypes.Bvec2, NodeCategory.Value, "bvec2"));

            Add(new ConstantNodeFactory(PinTypes.Int, NodeCategory.Value, "int"));
            Add(new ConstantNodeFactory(PinTypes.Ivec4, NodeCategory.Value, "ivec4"));
            Add(new ConstantNodeFactory(PinTypes.Ivec3, NodeCategory.Value, "ivec3"));
            Add(new ConstantNodeFactory(PinTypes.Ivec2, NodeCategory.Value, "ivec2"));

            Add(new ConstantNodeFactory(PinTypes.Mat4, NodeCategory.Value, "mat4"));
            Add(new ConstantNodeFactory(PinTypes.Mat3, NodeCategory.Value, "mat3"));
            Add(new ConstantNodeFactory(PinTypes.Mat2, NodeCategory.Value, "mat2"));

            Add(new ParameterNodeFactory(PinTypes.Special.Color, NodeCategory.Parameter, "MatDiffColor"));
            Add(new ParameterNodeFactory(PinTypes.Special.Color, NodeCategory.Parameter, "MatEmissiveColor"));
            Add(new ParameterNodeFactory(PinTypes.Special.Color, NodeCategory.Parameter, "MatEnvMapColor"));
            Add(new ParameterNodeFactory(PinTypes.Special.Color, NodeCategory.Parameter, Parameters.MatSpecColor));
            foreach (var parameter in NodeTypes._parameters)
                Add(new ParameterNodeFactory(parameter.Value, NodeCategory.Parameter, parameter.Value + "Parameter"));
            Add(new ParameterNodeFactory(PinTypes.Float, NodeCategory.Parameter, "Roughness"));
            Add(new ParameterNodeFactory(PinTypes.Float, NodeCategory.Parameter, "Metallic"));

            Add(new UniformNodeFactory(PinTypes.Float, NodeCategory.Parameter, "DeltaTimePS"));
            Add(new UniformNodeFactory(PinTypes.Float, NodeCategory.Parameter, "ElapsedTimePS"));
            Add(new UniformNodeFactory(PinTypes.Float, NodeCategory.Parameter, "ElapsedTime"));
            Add(new BuildInVariableNodeFactory(PinTypes.Vec4, NodeCategory.Parameter, NodeTypes.FragCoord));
            Add(new BuildInVariableNodeFactory(PinTypes.Bool, NodeCategory.Parameter, NodeTypes.FrontFacing));

#if DEBUG
            var debugVisibility = NodeFactoryVisibility.Visible;
#else
            var debugVisibility = NodeFactoryVisibility.Hidden;
#endif
            foreach (var parameter in NodeTypes._uniforms)
                Add(new UniformNodeFactory(parameter.Value, NodeCategory.Parameter, parameter.Value + "Uniform", debugVisibility));


            Add(new OutputNodeFactory(NodeTypes.AmbientColor, "ambientColor", PinTypes.Vec4));
            Add(new OutputNodeFactory(NodeTypes.RefractionColor, "refractionColor", PinTypes.Vec4));
            Add(new OutputNodeFactory(NodeTypes.Opacity, "opacity", PinTypes.Float));
            Add(new OutputNodeFactory(NodeTypes.LightColor, "lightColor", PinTypes.Vec4));
            Add(new OutputNodeFactory(NodeTypes.PositionOutput, "positionOutput", PinTypes.Vec4));
            Add(new OutputNodeFactory(NodeTypes.Discard, "discard", PinTypes.Bool));

            Add(new VertexDataNodeFactory());
            Add(new ObjectDataNodeFactory());
            Add(new LightDataNodeFactory());
            Add(new CameraDataNodeFactory());
            Add(new ZoneDataNodeFactory());

            foreach (var connector in NodeTypes._connectors)
                Add(new ConnectorNodeFactory(connector.Key, "Connect " + connector.Value, connector.Value));
            foreach (var attribute in NodeTypes._attributes)
                Add(new AttributeNodeFactory(attribute.Key, attribute.Value + "Attribute", attribute.Value, debugVisibility));
            Add(new OutputNodeFactory(NodeTypes.Special.FinalColor, "finalColor", PinTypes.Vec4, debugVisibility));
            Add(new OutputNodeFactory(NodeTypes.Special.FragData0, "fragData[0]", PinTypes.Vec4, debugVisibility));
            Add(new OutputNodeFactory(NodeTypes.Special.FragData1, "fragData[1]", PinTypes.Vec4, debugVisibility));
            Add(new OutputNodeFactory(NodeTypes.Special.FragData2, "fragData[2]", PinTypes.Vec4, debugVisibility));
            Add(new OutputNodeFactory(NodeTypes.Special.FragData3, "fragData[3]", PinTypes.Vec4, debugVisibility));

            Add(new MarkerNodeFactory(NodeTypes.Cull, NodeTypes.Cull, Urho.CullMode.Ccw.ToString()));
            Add(new MarkerNodeFactory(NodeTypes.ShadowCull, NodeTypes.ShadowCull, Urho.CullMode.Ccw.ToString()));
            Add(new MarkerNodeFactory(NodeTypes.Fill, NodeTypes.Fill, Urho.FillMode.Solid.ToString()));
            Add(new DefineNodeFactory(NodeTypes.Define, NodeTypes.Define));
            Add(new DefineNodeFactory(NodeTypes.Undefine, NodeTypes.Undefine));
            foreach (var connector in NodeTypes._ifdefs)
                Add(new IfDefNodeFactory(connector.Key, "ifdef(" + connector.Value + ")", connector.Value));
            Add(FunctionNodeFactory.Function(NodeTypes.SampleShadow, "sampleShadow", "light",
                new[]
                {
                    new Pin("shadowMap", PinTypes.Sampler2D),
                    new Pin("shadowPos", PinTypes.Vec4)
                },
                new[] {new Pin("", PinTypes.Float)}));
            Add(FunctionNodeFactory.Function(NodeTypes.SampleVSMShadow, "sampleVSMShadow", "light",
                new[]
                {
                    new Pin("shadowMap", PinTypes.Sampler2D),
                    new Pin("shadowPos", PinTypes.Vec2)
                },
                new[] {new Pin("", PinTypes.Vec2)}));

            Add(FunctionNodeFactory.Function(NodeTypes.PerPixelFloat, "per pixel float", "per pixel",
                new[] {new Pin("x", PinTypes.Float)}, PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.PerPixelVec2, "per pixel vec2", "per pixel",
                new[] {new Pin("x", PinTypes.Vec2)}, PinTypes.Vec2));
            Add(FunctionNodeFactory.Function(NodeTypes.PerPixelVec3, "per pixel vec3", "per pixel",
                new[] {new Pin("x", PinTypes.Vec3)}, PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.PerPixelVec4, "per pixel vec4", "per pixel",
                new[] {new Pin("x", PinTypes.Vec4)}, PinTypes.Vec4));

            Add(FunctionNodeFactory.Function(NodeTypes.TernaryFloat, NodeTypes.TernaryFloat,
                NodeTypes.SubCategories.Logic, "select",
                new[]
                {
                    new Pin("condition", PinTypes.Bool), new Pin("true", PinTypes.Float),
                    new Pin("false", PinTypes.Float)
                }, PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.TernaryVec2, NodeTypes.TernaryVec2,
                NodeTypes.SubCategories.Logic, "select",
                new[]
                {
                    new Pin("condition", PinTypes.Bool), new Pin("true", PinTypes.Vec2), new Pin("false", PinTypes.Vec2)
                },
                PinTypes.Vec2));
            Add(FunctionNodeFactory.Function(NodeTypes.TernaryVec3, NodeTypes.TernaryVec3,
                NodeTypes.SubCategories.Logic, "select",
                new[]
                {
                    new Pin("condition", PinTypes.Bool), new Pin("true", PinTypes.Vec3), new Pin("false", PinTypes.Vec3)
                },
                PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.TernaryVec4, NodeTypes.TernaryVec4,
                NodeTypes.SubCategories.Logic, "select",
                new[]
                {
                    new Pin("condition", PinTypes.Bool), new Pin("true", PinTypes.Vec4), new Pin("false", PinTypes.Vec4)
                },
                PinTypes.Vec4));

            Add(FunctionNodeFactory.Function(NodeTypes.SkinMatrixIndex, NodeTypes.SkinMatrixIndex, "make/break",
                new[]
                {
                    new Pin(PinIds.Value, PinTypes.Skin),
                    new Pin(PinIds.Index, PinTypes.Int)
                },
                new[]
                {
                    new Pin("", PinTypes.Mat4x3)
                }));
            Add(FunctionNodeFactory.Function(NodeTypes.VertexLightsIndex, NodeTypes.VertexLightsIndex, "make/break",
                new[]
                {
                    new Pin(PinIds.Value, PinTypes.VertexLights),
                    new Pin(PinIds.Index, PinTypes.Int)
                },
                new[]
                {
                    new Pin("", PinTypes.Vec4)
                }));
            Add(FunctionNodeFactory.Function(NodeTypes.LightMatricesIndex, NodeTypes.LightMatricesIndex, "make/break",
                new[]
                {
                    new Pin(PinIds.Value, PinTypes.LightMatrices),
                    new Pin(PinIds.Index, PinTypes.Int)
                },
                new[]
                {
                    new Pin("", PinTypes.Mat4)
                }));

            Add(FunctionNodeFactory.Function(NodeTypes.BreakVec2, "break vec2", "make/break",
                new[] {new Pin(PinIds.Value, PinTypes.Vec2)},
                new[]
                {
                    new Pin("X", PinTypes.Float),
                    new Pin("Y", PinTypes.Float)
                }));
            Add(FunctionNodeFactory.Function(NodeTypes.BreakVec3, "break vec3", "make/break",
                new[] {new Pin(PinIds.Value, PinTypes.Vec3)},
                new[]
                {
                    new Pin("X", PinTypes.Float),
                    new Pin("Y", PinTypes.Float),
                    new Pin("Z", PinTypes.Float)
                }));
            Add(FunctionNodeFactory.Function(NodeTypes.BreakVec4, "break vec4", "make/break",
                new[] {new Pin(PinIds.Value, PinTypes.Vec4)},
                new[]
                {
                    new Pin("X", PinTypes.Float),
                    new Pin("Y", PinTypes.Float),
                    new Pin("Z", PinTypes.Float),
                    new Pin("W", PinTypes.Float)
                }));
            Add(FunctionNodeFactory.Function(NodeTypes.BreakVec4, "break ivec4", "make/break",
                new[] {new Pin(PinIds.Value, PinTypes.Ivec4)},
                new[]
                {
                    new Pin("X", PinTypes.Int),
                    new Pin("Y", PinTypes.Int),
                    new Pin("Z", PinTypes.Int),
                    new Pin("W", PinTypes.Int)
                }));
            Add(FunctionNodeFactory.Function(NodeTypes.BreakVec3ToVec2AndFloat, "break vec3 to vec2, float",
                "make/break",
                new[] {new Pin(PinIds.Value, PinTypes.Vec3)},
                new[]
                {
                    new Pin("XY", PinTypes.Vec2),
                    new Pin("Z", PinTypes.Float)
                }));
            Add(FunctionNodeFactory.Function(NodeTypes.BreakVec4ToVec3AndFloat, "break vec4 to vec3, float",
                "make/break",
                new[] {new Pin(PinIds.Value, PinTypes.Vec4)},
                new[]
                {
                    new Pin("XYZ", PinTypes.Vec3),
                    new Pin("W", PinTypes.Float)
                }));
            Add(FunctionNodeFactory.Function(NodeTypes.BreakVec4ToVec2AndVec2, "break vec4 to vec2, vec2", "make/break",
                new[] {new Pin(PinIds.Value, PinTypes.Vec4)},
                new[]
                {
                    new Pin("XY", PinTypes.Vec2),
                    new Pin("ZW", PinTypes.Vec2)
                }));
            Add(FunctionNodeFactory.Function(NodeTypes.BreakVec4ToVec2AndFloats, "break vec4 to vec2, float, float", "make/break",
                new[] { new Pin(PinIds.Value, PinTypes.Vec4) },
                new[]
                {
                    new Pin("XY", PinTypes.Vec2),
                    new Pin("Z", PinTypes.Float),
                    new Pin("W", PinTypes.Float),
                }));
    
            Add(FunctionNodeFactory.Function(NodeTypes.MakeVec2, "vec2(float,float)", "make/break",
                new[]
                {
                    new Pin("X", PinTypes.Float),
                    new Pin("Y", PinTypes.Float)
                },
                PinTypes.Vec2));
            Add(FunctionNodeFactory.Function(NodeTypes.MakeVec3, "vec3(float,float,float)", "make/break",
                new[]
                {
                    new Pin("X", PinTypes.Float),
                    new Pin("Y", PinTypes.Float),
                    new Pin("Z", PinTypes.Float)
                },
                PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.MakeVec4, "vec4(float,float,float,float)", "make/break",
                new[]
                {
                    new Pin("X", PinTypes.Float),
                    new Pin("Y", PinTypes.Float),
                    new Pin("Z", PinTypes.Float),
                    new Pin("W", PinTypes.Float)
                }, PinTypes.Vec4));
            Add(FunctionNodeFactory.Function(NodeTypes.MakeMat2, "mat2(float,float,float,float)", "make/break",
                new[]
                {
                    new Pin("m00", PinTypes.Float),
                    new Pin("m01", PinTypes.Float),
                    new Pin("m10", PinTypes.Float),
                    new Pin("m11", PinTypes.Float)
                }, PinTypes.Mat2));
            Add(FunctionNodeFactory.Function(NodeTypes.MakeVec4FromVec3AndFloat, "vec4(vec3,float)", "make/break",
                new[]
                {
                    new Pin("XYZ", PinTypes.Vec3),
                    new Pin("W", PinTypes.Float)
                },
                PinTypes.Vec4));
            Add(FunctionNodeFactory.Function(NodeTypes.MakeVec3FromVec2AndFloat, "vec3(vec2,float)", "make/break",
                new[]
                {
                    new Pin("XY", PinTypes.Vec2),
                    new Pin("Z", PinTypes.Float)
                },
                PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.MakeVec4FromVec2AndVec2, "vec4(vec2,vec2)", "make/break",
                new[]
                {
                    new Pin("XY", PinTypes.Vec2),
                    new Pin("ZW", PinTypes.Vec2)
                },
                PinTypes.Vec4));
            Add(FunctionNodeFactory.Function(NodeTypes.MakeMat3FromMat4x3, "mat3(mat4x3)", "make/break",
                new[] {new Pin("mat4x3", PinTypes.Mat4x3)},
                new[] {new Pin("", PinTypes.Mat3)}));
            Add(FunctionNodeFactory.Function(NodeTypes.MakeMat3FromMat4, "mat3(mat4)", "make/break",
                new[] {new Pin("mat4", PinTypes.Mat4)},
                new[] {new Pin("", PinTypes.Mat3)}));
            Add(FunctionNodeFactory.Function(NodeTypes.MakeMat3FromVec3Vec3Vec3, NodeTypes.MakeMat3FromVec3Vec3Vec3,
                "make/break",
                new[] {new Pin("[0]", PinTypes.Vec3), new Pin("[1]", PinTypes.Vec3), new Pin("[2]", PinTypes.Vec3)},
                new[] {new Pin("", PinTypes.Mat3)}));
            Add(FunctionNodeFactory.Function(NodeTypes.MakeMat4x3FromVec4Vec4Vec4, "mat4x3(vec4,vec4,vec4)",
                "make/break",
                new[]
                {
                    new Pin("[0]", PinTypes.Vec4),
                    new Pin("[1]", PinTypes.Vec4),
                    new Pin("[2]", PinTypes.Vec4)
                },
                PinTypes.Mat4x3));

            Add(FunctionNodeFactory.Function(NodeTypes.RadiansFloat, NodeTypes.RadiansFloat,
                NodeTypes.SubCategories.Trigonometry, "radians",
                new[] {new Pin("degrees", PinTypes.Float)}, PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.RadiansVec2, NodeTypes.RadiansVec2,
                NodeTypes.SubCategories.Trigonometry, "radians",
                new[] {new Pin("degrees", PinTypes.Vec2)}, PinTypes.Vec2));
            Add(FunctionNodeFactory.Function(NodeTypes.RadiansVec3, NodeTypes.RadiansVec3,
                NodeTypes.SubCategories.Trigonometry, "radians",
                new[] {new Pin("degrees", PinTypes.Vec3)}, PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.RadiansVec4, NodeTypes.RadiansVec4,
                NodeTypes.SubCategories.Trigonometry, "radians",
                new[] {new Pin("degrees", PinTypes.Vec4)}, PinTypes.Vec4));
            Add(FunctionNodeFactory.Function(NodeTypes.DegreesFloat, NodeTypes.DegreesFloat,
                NodeTypes.SubCategories.Trigonometry, "degrees",
                new[] {new Pin("radians", PinTypes.Float)}, PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.DegreesVec2, NodeTypes.DegreesVec2,
                NodeTypes.SubCategories.Trigonometry, "degrees",
                new[] {new Pin("radians", PinTypes.Vec2)}, PinTypes.Vec2));
            Add(FunctionNodeFactory.Function(NodeTypes.DegreesVec3, NodeTypes.DegreesVec3,
                NodeTypes.SubCategories.Trigonometry, "degrees",
                new[] {new Pin("radians", PinTypes.Vec3)}, PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.DegreesVec4, NodeTypes.DegreesVec4,
                NodeTypes.SubCategories.Trigonometry, "degrees",
                new[] {new Pin("radians", PinTypes.Vec4)}, PinTypes.Vec4));
            Add(FunctionNodeFactory.Function(NodeTypes.SinFloat, NodeTypes.SinFloat,
                NodeTypes.SubCategories.Trigonometry, "sin",
                new[] {new Pin(PinIds.Angle, PinTypes.Float)}, PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.SinVec2, NodeTypes.SinVec2, NodeTypes.SubCategories.Trigonometry,
                "sin",
                new[] {new Pin(PinIds.Angle, PinTypes.Vec2)}, PinTypes.Vec2));
            Add(FunctionNodeFactory.Function(NodeTypes.SinVec3, NodeTypes.SinVec3, NodeTypes.SubCategories.Trigonometry,
                "sin",
                new[] {new Pin(PinIds.Angle, PinTypes.Vec3)}, PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.SinVec4, NodeTypes.SinVec4, NodeTypes.SubCategories.Trigonometry,
                "sin",
                new[] {new Pin(PinIds.Angle, PinTypes.Vec4)}, PinTypes.Vec4));
            Add(FunctionNodeFactory.Function(NodeTypes.CosFloat, NodeTypes.CosFloat,
                NodeTypes.SubCategories.Trigonometry, "cos",
                new[] {new Pin(PinIds.Angle, PinTypes.Float)}, PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.CosVec2, NodeTypes.CosVec2, NodeTypes.SubCategories.Trigonometry,
                "cos",
                new[] {new Pin(PinIds.Angle, PinTypes.Vec2)}, PinTypes.Vec2));
            Add(FunctionNodeFactory.Function(NodeTypes.CosVec3, NodeTypes.CosVec3, NodeTypes.SubCategories.Trigonometry,
                "cos",
                new[] {new Pin(PinIds.Angle, PinTypes.Vec3)}, PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.CosVec4, NodeTypes.CosVec4, NodeTypes.SubCategories.Trigonometry,
                "cos",
                new[] {new Pin(PinIds.Angle, PinTypes.Vec4)}, PinTypes.Vec4));
            Add(FunctionNodeFactory.Function(NodeTypes.TanFloat, NodeTypes.TanFloat,
                NodeTypes.SubCategories.Trigonometry, "tan",
                new[] {new Pin(PinIds.Angle, PinTypes.Float)}, PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.TanVec2, NodeTypes.TanVec2, NodeTypes.SubCategories.Trigonometry,
                "tan",
                new[] {new Pin(PinIds.Angle, PinTypes.Vec2)}, PinTypes.Vec2));
            Add(FunctionNodeFactory.Function(NodeTypes.TanVec3, NodeTypes.TanVec3, NodeTypes.SubCategories.Trigonometry,
                "tan",
                new[] {new Pin(PinIds.Angle, PinTypes.Vec3)}, PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.TanVec4, NodeTypes.TanVec4, NodeTypes.SubCategories.Trigonometry,
                "tan",
                new[] {new Pin(PinIds.Angle, PinTypes.Vec4)}, PinTypes.Vec4));
            Add(FunctionNodeFactory.Function(NodeTypes.AsinFloat, NodeTypes.AsinFloat,
                NodeTypes.SubCategories.Trigonometry, "asin",
                new[] {new Pin(PinIds.X, PinTypes.Float)}, PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.AsinVec2, NodeTypes.AsinVec2,
                NodeTypes.SubCategories.Trigonometry, "asin",
                new[] {new Pin(PinIds.X, PinTypes.Vec2)}, PinTypes.Vec2));
            Add(FunctionNodeFactory.Function(NodeTypes.AsinVec3, NodeTypes.AsinVec3,
                NodeTypes.SubCategories.Trigonometry, "asin",
                new[] {new Pin(PinIds.X, PinTypes.Vec3)}, PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.AsinVec4, NodeTypes.AsinVec4,
                NodeTypes.SubCategories.Trigonometry, "asin",
                new[] {new Pin(PinIds.X, PinTypes.Vec4)}, PinTypes.Vec4));
            Add(FunctionNodeFactory.Function(NodeTypes.AcosFloat, NodeTypes.AcosFloat,
                NodeTypes.SubCategories.Trigonometry, "acos",
                new[] {new Pin(PinIds.X, PinTypes.Float)}, PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.AcosVec2, NodeTypes.AcosVec2,
                NodeTypes.SubCategories.Trigonometry, "acos",
                new[] {new Pin(PinIds.X, PinTypes.Vec2)}, PinTypes.Vec2));
            Add(FunctionNodeFactory.Function(NodeTypes.AcosVec3, NodeTypes.AcosVec3,
                NodeTypes.SubCategories.Trigonometry, "acos",
                new[] {new Pin(PinIds.X, PinTypes.Vec3)}, PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.AcosVec4, NodeTypes.AcosVec4,
                NodeTypes.SubCategories.Trigonometry, "acos",
                new[] {new Pin(PinIds.X, PinTypes.Vec4)}, PinTypes.Vec4));
            Add(FunctionNodeFactory.Function(NodeTypes.AtanFloat, NodeTypes.AtanFloat,
                NodeTypes.SubCategories.Trigonometry, "atan",
                new[] {new Pin("y_over_x", PinTypes.Float)}, PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.AtanVec2, NodeTypes.AtanVec2,
                NodeTypes.SubCategories.Trigonometry, "atan",
                new[] {new Pin("y_over_x", PinTypes.Vec2)}, PinTypes.Vec2));
            Add(FunctionNodeFactory.Function(NodeTypes.AtanVec3, NodeTypes.AtanVec3,
                NodeTypes.SubCategories.Trigonometry, "atan",
                new[] {new Pin("y_over_x", PinTypes.Vec3)}, PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.AtanVec4, NodeTypes.AtanVec4,
                NodeTypes.SubCategories.Trigonometry, "atan",
                new[] {new Pin("y_over_x", PinTypes.Vec4)}, PinTypes.Vec4));
            Add(FunctionNodeFactory.Function(NodeTypes.AtanFloatFloat, NodeTypes.AtanFloatFloat,
                NodeTypes.SubCategories.Trigonometry, "atan",
                new[] {new Pin(PinIds.Y, PinTypes.Float), new Pin("x", PinTypes.Float)},
                PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.AtanVec2Vec2, NodeTypes.AtanVec2Vec2,
                NodeTypes.SubCategories.Trigonometry, "atan",
                new[] {new Pin(PinIds.Y, PinTypes.Vec2), new Pin("x", PinTypes.Vec2)}, PinTypes.Vec2));
            Add(FunctionNodeFactory.Function(NodeTypes.AtanVec3Vec3, NodeTypes.AtanVec3Vec3,
                NodeTypes.SubCategories.Trigonometry, "atan",
                new[] {new Pin(PinIds.Y, PinTypes.Vec3), new Pin("x", PinTypes.Vec3)}, PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.AtanVec4Vec4, NodeTypes.AtanVec4Vec4,
                NodeTypes.SubCategories.Trigonometry, "atan",
                new[] {new Pin(PinIds.Y, PinTypes.Vec4), new Pin("x", PinTypes.Vec4)}, PinTypes.Vec4));
            Add(FunctionNodeFactory.Function(NodeTypes.PowFloatFloat, NodeTypes.PowFloatFloat, "pow",
                new[] {new Pin(PinIds.X, PinTypes.Float), new Pin(PinIds.Y, PinTypes.Float)},
                PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.PowVec2Vec2, NodeTypes.PowVec2Vec2, "pow",
                new[] {new Pin(PinIds.X, PinTypes.Vec2), new Pin(PinIds.Y, PinTypes.Vec2)}, PinTypes.Vec2));
            Add(FunctionNodeFactory.Function(NodeTypes.PowVec3Vec3, NodeTypes.PowVec3Vec3, "pow",
                new[] {new Pin(PinIds.X, PinTypes.Vec3), new Pin(PinIds.Y, PinTypes.Vec3)}, PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.PowVec4Vec4, NodeTypes.PowVec4Vec4, "pow",
                new[] {new Pin(PinIds.X, PinTypes.Vec4), new Pin(PinIds.Y, PinTypes.Vec4)}, PinTypes.Vec4));
            Add(FunctionNodeFactory.Function(NodeTypes.ExpFloat, NodeTypes.ExpFloat, "exp",
                new[] {new Pin(PinIds.X, PinTypes.Float)}, PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.ExpVec2, NodeTypes.ExpVec2, "exp",
                new[] {new Pin(PinIds.X, PinTypes.Vec2)}, PinTypes.Vec2));
            Add(FunctionNodeFactory.Function(NodeTypes.ExpVec3, NodeTypes.ExpVec3, "exp",
                new[] {new Pin(PinIds.X, PinTypes.Vec3)}, PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.ExpVec4, NodeTypes.ExpVec4, "exp",
                new[] {new Pin(PinIds.X, PinTypes.Vec4)}, PinTypes.Vec4));
            Add(FunctionNodeFactory.Function(NodeTypes.LogFloat, NodeTypes.LogFloat, "log",
                new[] {new Pin(PinIds.X, PinTypes.Float)}, PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.LogVec2, NodeTypes.LogVec2, "log",
                new[] {new Pin(PinIds.X, PinTypes.Vec2)}, PinTypes.Vec2));
            Add(FunctionNodeFactory.Function(NodeTypes.LogVec3, NodeTypes.LogVec3, "log",
                new[] {new Pin(PinIds.X, PinTypes.Vec3)}, PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.LogVec4, NodeTypes.LogVec4, "log",
                new[] {new Pin(PinIds.X, PinTypes.Vec4)}, PinTypes.Vec4));
            Add(FunctionNodeFactory.Function(NodeTypes.Exp2Float, NodeTypes.Exp2Float, "exp2",
                new[] {new Pin(PinIds.X, PinTypes.Float)}, PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.Exp2Vec2, NodeTypes.Exp2Vec2, "exp2",
                new[] {new Pin(PinIds.X, PinTypes.Vec2)}, PinTypes.Vec2));
            Add(FunctionNodeFactory.Function(NodeTypes.Exp2Vec3, NodeTypes.Exp2Vec3, "exp2",
                new[] {new Pin(PinIds.X, PinTypes.Vec3)}, PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.Exp2Vec4, NodeTypes.Exp2Vec4, "exp2",
                new[] {new Pin(PinIds.X, PinTypes.Vec4)}, PinTypes.Vec4));
            Add(FunctionNodeFactory.Function(NodeTypes.Log2Float, NodeTypes.Log2Float, "log2",
                new[] {new Pin(PinIds.X, PinTypes.Float)}, PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.Log2Vec2, NodeTypes.Log2Vec2, "log2",
                new[] {new Pin(PinIds.X, PinTypes.Vec2)}, PinTypes.Vec2));
            Add(FunctionNodeFactory.Function(NodeTypes.Log2Vec3, NodeTypes.Log2Vec3, "log2",
                new[] {new Pin(PinIds.X, PinTypes.Vec3)}, PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.Log2Vec4, NodeTypes.Log2Vec4, "log2",
                new[] {new Pin(PinIds.X, PinTypes.Vec4)}, PinTypes.Vec4));
            Add(FunctionNodeFactory.Function(NodeTypes.SqrtFloat, NodeTypes.SqrtFloat, "sqrt",
                new[] {new Pin(PinIds.X, PinTypes.Float)}, PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.SqrtVec2, NodeTypes.SqrtVec2, "sqrt",
                new[] {new Pin(PinIds.X, PinTypes.Vec2)}, PinTypes.Vec2));
            Add(FunctionNodeFactory.Function(NodeTypes.SqrtVec3, NodeTypes.SqrtVec3, "sqrt",
                new[] {new Pin(PinIds.X, PinTypes.Vec3)}, PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.SqrtVec4, NodeTypes.SqrtVec4, "sqrt",
                new[] {new Pin(PinIds.X, PinTypes.Vec4)}, PinTypes.Vec4));
            Add(FunctionNodeFactory.Function(NodeTypes.InversesqrtFloat, NodeTypes.InversesqrtFloat, "inversesqrt",
                new[] {new Pin(PinIds.X, PinTypes.Float)}, PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.InversesqrtVec2, NodeTypes.InversesqrtVec2, "inversesqrt",
                new[] {new Pin(PinIds.X, PinTypes.Vec2)}, PinTypes.Vec2));
            Add(FunctionNodeFactory.Function(NodeTypes.InversesqrtVec3, NodeTypes.InversesqrtVec3, "inversesqrt",
                new[] {new Pin(PinIds.X, PinTypes.Vec3)}, PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.InversesqrtVec4, NodeTypes.InversesqrtVec4, "inversesqrt",
                new[] {new Pin(PinIds.X, PinTypes.Vec4)}, PinTypes.Vec4));
            Add(FunctionNodeFactory.Function(NodeTypes.AbsFloat, NodeTypes.AbsFloat, "abs",
                new[] {new Pin(PinIds.X, PinTypes.Float)}, PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.AbsVec2, NodeTypes.AbsVec2, "abs",
                new[] {new Pin(PinIds.X, PinTypes.Vec2)}, PinTypes.Vec2));
            Add(FunctionNodeFactory.Function(NodeTypes.AbsVec3, NodeTypes.AbsVec3, "abs",
                new[] {new Pin(PinIds.X, PinTypes.Vec3)}, PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.AbsVec4, NodeTypes.AbsVec4, "abs",
                new[] {new Pin(PinIds.X, PinTypes.Vec4)}, PinTypes.Vec4));
            Add(FunctionNodeFactory.Function(NodeTypes.SignFloat, NodeTypes.SignFloat, "sign",
                new[] {new Pin(PinIds.X, PinTypes.Float)}, PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.SignVec2, NodeTypes.SignVec2, "sign",
                new[] {new Pin(PinIds.X, PinTypes.Vec2)}, PinTypes.Vec2));
            Add(FunctionNodeFactory.Function(NodeTypes.SignVec3, NodeTypes.SignVec3, "sign",
                new[] {new Pin(PinIds.X, PinTypes.Vec3)}, PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.SignVec4, NodeTypes.SignVec4, "sign",
                new[] {new Pin(PinIds.X, PinTypes.Vec4)}, PinTypes.Vec4));
            Add(FunctionNodeFactory.Function(NodeTypes.FloorFloat, NodeTypes.FloorFloat, "floor",
                new[] {new Pin(PinIds.X, PinTypes.Float)}, PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.FloorVec2, NodeTypes.FloorVec2, "floor",
                new[] {new Pin(PinIds.X, PinTypes.Vec2)}, PinTypes.Vec2));
            Add(FunctionNodeFactory.Function(NodeTypes.FloorVec3, NodeTypes.FloorVec3, "floor",
                new[] {new Pin(PinIds.X, PinTypes.Vec3)}, PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.FloorVec4, NodeTypes.FloorVec4, "floor",
                new[] {new Pin(PinIds.X, PinTypes.Vec4)}, PinTypes.Vec4));
            Add(FunctionNodeFactory.Function(NodeTypes.CeilFloat, NodeTypes.CeilFloat, "ceil",
                new[] {new Pin(PinIds.X, PinTypes.Float)}, PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.CeilVec2, NodeTypes.CeilVec2, "ceil",
                new[] {new Pin(PinIds.X, PinTypes.Vec2)}, PinTypes.Vec2));
            Add(FunctionNodeFactory.Function(NodeTypes.CeilVec3, NodeTypes.CeilVec3, "ceil",
                new[] {new Pin(PinIds.X, PinTypes.Vec3)}, PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.CeilVec4, NodeTypes.CeilVec4, "ceil",
                new[] {new Pin(PinIds.X, PinTypes.Vec4)}, PinTypes.Vec4));
            Add(FunctionNodeFactory.Function(NodeTypes.FractFloat, NodeTypes.FractFloat, "fract",
                new[] {new Pin(PinIds.X, PinTypes.Float)}, PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.FractVec2, NodeTypes.FractVec2, "fract",
                new[] {new Pin(PinIds.X, PinTypes.Vec2)}, PinTypes.Vec2));
            Add(FunctionNodeFactory.Function(NodeTypes.FractVec3, NodeTypes.FractVec3, "fract",
                new[] {new Pin(PinIds.X, PinTypes.Vec3)}, PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.FractVec4, NodeTypes.FractVec4, "fract",
                new[] {new Pin(PinIds.X, PinTypes.Vec4)}, PinTypes.Vec4));
            Add(FunctionNodeFactory.Function(NodeTypes.ModFloatFloat, NodeTypes.ModFloatFloat,
                NodeTypes.SubCategories.Arithmetic, "mod",
                new[] {new Pin(PinIds.X, PinTypes.Float), new Pin(PinIds.Y, PinTypes.Float)},
                PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.ModVec2Vec2, NodeTypes.ModVec2Vec2,
                NodeTypes.SubCategories.Arithmetic, "mod",
                new[] {new Pin(PinIds.X, PinTypes.Vec2), new Pin(PinIds.Y, PinTypes.Vec2)}, PinTypes.Vec2));
            Add(FunctionNodeFactory.Function(NodeTypes.ModVec3Vec3, NodeTypes.ModVec3Vec3,
                NodeTypes.SubCategories.Arithmetic, "mod",
                new[] {new Pin(PinIds.X, PinTypes.Vec3), new Pin(PinIds.Y, PinTypes.Vec3)}, PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.ModVec4Vec4, NodeTypes.ModVec4Vec4,
                NodeTypes.SubCategories.Arithmetic, "mod",
                new[] {new Pin(PinIds.X, PinTypes.Vec4), new Pin(PinIds.Y, PinTypes.Vec4)}, PinTypes.Vec4));
            Add(FunctionNodeFactory.Function(NodeTypes.ModVec2Float, NodeTypes.ModVec2Float,
                NodeTypes.SubCategories.Arithmetic, "mod",
                new[] {new Pin(PinIds.X, PinTypes.Vec2), new Pin(PinIds.Y, PinTypes.Float)}, PinTypes.Vec2));
            Add(FunctionNodeFactory.Function(NodeTypes.ModVec3Float, NodeTypes.ModVec3Float,
                NodeTypes.SubCategories.Arithmetic, "mod",
                new[] {new Pin(PinIds.X, PinTypes.Vec3), new Pin(PinIds.Y, PinTypes.Float)}, PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.ModVec4Float, NodeTypes.ModVec4Float,
                NodeTypes.SubCategories.Arithmetic, "mod",
                new[] {new Pin(PinIds.X, PinTypes.Vec4), new Pin(PinIds.Y, PinTypes.Float)}, PinTypes.Vec4));
            Add(FunctionNodeFactory.Function(NodeTypes.MinFloatFloat, NodeTypes.MinFloatFloat, "min",
                new[] {new Pin(PinIds.X, PinTypes.Float), new Pin(PinIds.Y, PinTypes.Float)},
                PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.MinVec2Vec2, NodeTypes.MinVec2Vec2, "min",
                new[] {new Pin(PinIds.X, PinTypes.Vec2), new Pin(PinIds.Y, PinTypes.Vec2)}, PinTypes.Vec2));
            Add(FunctionNodeFactory.Function(NodeTypes.MinVec3Vec3, NodeTypes.MinVec3Vec3, "min",
                new[] {new Pin(PinIds.X, PinTypes.Vec3), new Pin(PinIds.Y, PinTypes.Vec3)}, PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.MinVec4Vec4, NodeTypes.MinVec4Vec4, "min",
                new[] {new Pin(PinIds.X, PinTypes.Vec4), new Pin(PinIds.Y, PinTypes.Vec4)}, PinTypes.Vec4));
            Add(FunctionNodeFactory.Function(NodeTypes.MinVec2Float, NodeTypes.MinVec2Float, "min",
                new[] {new Pin(PinIds.X, PinTypes.Vec2), new Pin(PinIds.Y, PinTypes.Float)}, PinTypes.Vec2));
            Add(FunctionNodeFactory.Function(NodeTypes.MinVec3Float, NodeTypes.MinVec3Float, "min",
                new[] {new Pin(PinIds.X, PinTypes.Vec3), new Pin(PinIds.Y, PinTypes.Float)}, PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.MinVec4Float, NodeTypes.MinVec4Float, "min",
                new[] {new Pin(PinIds.X, PinTypes.Vec4), new Pin(PinIds.Y, PinTypes.Float)}, PinTypes.Vec4));
            Add(FunctionNodeFactory.Function(NodeTypes.MaxFloatFloat, NodeTypes.MaxFloatFloat, "max",
                new[] {new Pin(PinIds.X, PinTypes.Float), new Pin(PinIds.Y, PinTypes.Float)},
                PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.MaxVec2Vec2, NodeTypes.MaxVec2Vec2, "max",
                new[] {new Pin(PinIds.X, PinTypes.Vec2), new Pin(PinIds.Y, PinTypes.Vec2)}, PinTypes.Vec2));
            Add(FunctionNodeFactory.Function(NodeTypes.MaxVec3Vec3, NodeTypes.MaxVec3Vec3, "max",
                new[] {new Pin(PinIds.X, PinTypes.Vec3), new Pin(PinIds.Y, PinTypes.Vec3)}, PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.MaxVec4Vec4, NodeTypes.MaxVec4Vec4, "max",
                new[] {new Pin(PinIds.X, PinTypes.Vec4), new Pin(PinIds.Y, PinTypes.Vec4)}, PinTypes.Vec4));
            Add(FunctionNodeFactory.Function(NodeTypes.MaxVec2Float, NodeTypes.MaxVec2Float, "max",
                new[] {new Pin(PinIds.X, PinTypes.Vec2), new Pin(PinIds.Y, PinTypes.Float)}, PinTypes.Vec2));
            Add(FunctionNodeFactory.Function(NodeTypes.MaxVec3Float, NodeTypes.MaxVec3Float, "max",
                new[] {new Pin(PinIds.X, PinTypes.Vec3), new Pin(PinIds.Y, PinTypes.Float)}, PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.MaxVec4Float, NodeTypes.MaxVec4Float, "max",
                new[] {new Pin(PinIds.X, PinTypes.Vec4), new Pin(PinIds.Y, PinTypes.Float)}, PinTypes.Vec4));
            Add(FunctionNodeFactory.Function(NodeTypes.ClampFloatFloatFloat, NodeTypes.ClampFloatFloatFloat, "clamp",
                new[]
                {
                    new Pin("x", PinTypes.Float), new Pin("minVal", PinTypes.Float), new Pin("maxVal", PinTypes.Float)
                }, PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.ClampVec2Vec2Vec2, NodeTypes.ClampVec2Vec2Vec2, "clamp",
                new[]
                {
                    new Pin(PinIds.X, PinTypes.Vec2), new Pin("minVal", PinTypes.Vec2), new Pin("maxVal", PinTypes.Vec2)
                },
                PinTypes.Vec2));
            Add(FunctionNodeFactory.Function(NodeTypes.ClampVec3Vec3Vec3, NodeTypes.ClampVec3Vec3Vec3, "clamp",
                new[]
                {
                    new Pin(PinIds.X, PinTypes.Vec3), new Pin("minVal", PinTypes.Vec3), new Pin("maxVal", PinTypes.Vec3)
                },
                PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.ClampVec4Vec4Vec4, NodeTypes.ClampVec4Vec4Vec4, "clamp",
                new[]
                {
                    new Pin(PinIds.X, PinTypes.Vec4), new Pin("minVal", PinTypes.Vec4), new Pin("maxVal", PinTypes.Vec4)
                },
                PinTypes.Vec4));
            Add(FunctionNodeFactory.Function(NodeTypes.ClampVec2FloatFloat, NodeTypes.ClampVec2FloatFloat, "clamp",
                new[]
                {
                    new Pin("x", PinTypes.Vec2), new Pin("minVal", PinTypes.Float), new Pin("maxVal", PinTypes.Float)
                }, PinTypes.Vec2));
            Add(FunctionNodeFactory.Function(NodeTypes.ClampVec3FloatFloat, NodeTypes.ClampVec3FloatFloat, "clamp",
                new[]
                {
                    new Pin("x", PinTypes.Vec3), new Pin("minVal", PinTypes.Float), new Pin("maxVal", PinTypes.Float)
                }, PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.ClampVec4FlfloatFloat, NodeTypes.ClampVec4FlfloatFloat, "clamp",
                new[]
                {
                    new Pin("x", PinTypes.Vec4), new Pin("minVal", PinTypes.Float), new Pin("maxVal", PinTypes.Float)
                }, PinTypes.Vec4));
            Add(FunctionNodeFactory.Function(NodeTypes.MixFloatFloatFloat, NodeTypes.MixFloatFloatFloat, "mix",
                new[]
                {
                    new Pin(PinIds.X, PinTypes.Float), new Pin(PinIds.Y, PinTypes.Float), new Pin("a", PinTypes.Float)
                },
                PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.MixVec2Vec2Vec2, NodeTypes.MixVec2Vec2Vec2, "mix",
                new[] {new Pin(PinIds.X, PinTypes.Vec2), new Pin(PinIds.Y, PinTypes.Vec2), new Pin("a", PinTypes.Vec2)},
                PinTypes.Vec2));
            Add(FunctionNodeFactory.Function(NodeTypes.MixVec3Vec3Vec3, NodeTypes.MixVec3Vec3Vec3, "mix",
                new[] {new Pin(PinIds.X, PinTypes.Vec3), new Pin(PinIds.Y, PinTypes.Vec3), new Pin("a", PinTypes.Vec3)},
                PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.MixVec4Vec4Vec4, NodeTypes.MixVec4Vec4Vec4, "mix",
                new[] {new Pin(PinIds.X, PinTypes.Vec4), new Pin(PinIds.Y, PinTypes.Vec4), new Pin("a", PinTypes.Vec4)},
                PinTypes.Vec4));
            Add(FunctionNodeFactory.Function(NodeTypes.MixVec2Vec2Float, NodeTypes.MixVec2Vec2Float, "mix",
                new[]
                {
                    new Pin(PinIds.X, PinTypes.Vec2), new Pin(PinIds.Y, PinTypes.Vec2), new Pin("a", PinTypes.Float)
                },
                PinTypes.Vec2));
            Add(FunctionNodeFactory.Function(NodeTypes.MixVec3Vec3Float, NodeTypes.MixVec3Vec3Float, "mix",
                new[]
                {
                    new Pin(PinIds.X, PinTypes.Vec3), new Pin(PinIds.Y, PinTypes.Vec3), new Pin("a", PinTypes.Float)
                },
                PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.MixVec4Vec4Float, NodeTypes.MixVec4Vec4Float, "mix",
                new[]
                {
                    new Pin(PinIds.X, PinTypes.Vec4), new Pin(PinIds.Y, PinTypes.Vec4), new Pin("a", PinTypes.Float)
                },
                PinTypes.Vec4));
            Add(FunctionNodeFactory.Function(NodeTypes.StepFloatFloat, NodeTypes.StepFloatFloat, "step",
                new[] {new Pin("edge", PinTypes.Float), new Pin("x", PinTypes.Float)},
                PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.StepVec2Vec2, NodeTypes.StepVec2Vec2, "step",
                new[] {new Pin("edge", PinTypes.Vec2), new Pin("x", PinTypes.Vec2)},
                PinTypes.Vec2));
            Add(FunctionNodeFactory.Function(NodeTypes.StepVec3Vec3, NodeTypes.StepVec3Vec3, "step",
                new[] {new Pin("edge", PinTypes.Vec3), new Pin("x", PinTypes.Vec3)},
                PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.StepVec4Vec4, NodeTypes.StepVec4Vec4, "step",
                new[] {new Pin("edge", PinTypes.Vec4), new Pin("x", PinTypes.Vec4)},
                PinTypes.Vec4));
            Add(FunctionNodeFactory.Function(NodeTypes.StepFloatVec2, NodeTypes.StepFloatVec2, "step",
                new[] {new Pin("edge", PinTypes.Float), new Pin("x", PinTypes.Vec2)},
                PinTypes.Vec2));
            Add(FunctionNodeFactory.Function(NodeTypes.StepFloatVec3, NodeTypes.StepFloatVec3, "step",
                new[] {new Pin("edge", PinTypes.Float), new Pin("x", PinTypes.Vec3)},
                PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.StepFloatVec4, NodeTypes.StepFloatVec4, "step",
                new[] {new Pin("edge", PinTypes.Float), new Pin("x", PinTypes.Vec4)},
                PinTypes.Vec4));
            Add(FunctionNodeFactory.Function(NodeTypes.SmoothstepFloatFloatFloat, NodeTypes.SmoothstepFloatFloatFloat,
                "smoothstep",
                new[]
                {
                    new Pin("edge0", PinTypes.Float), new Pin("edge1", PinTypes.Float), new Pin("x", PinTypes.Float)
                }, PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.SmoothstepVec2Vec2Vec2, NodeTypes.SmoothstepVec2Vec2Vec2,
                "smoothstep",
                new[] {new Pin("edge0", PinTypes.Vec2), new Pin("edge1", PinTypes.Vec2), new Pin("x", PinTypes.Vec2)},
                PinTypes.Vec2));
            Add(FunctionNodeFactory.Function(NodeTypes.SmoothstepVec3Vec3Vec3, NodeTypes.SmoothstepVec3Vec3Vec3,
                "smoothstep",
                new[] {new Pin("edge0", PinTypes.Vec3), new Pin("edge1", PinTypes.Vec3), new Pin("x", PinTypes.Vec3)},
                PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.SmoothstepVec4Vec4Vec4, NodeTypes.SmoothstepVec4Vec4Vec4,
                "smoothstep",
                new[] {new Pin("edge0", PinTypes.Vec4), new Pin("edge1", PinTypes.Vec4), new Pin("x", PinTypes.Vec4)},
                PinTypes.Vec4));
            Add(FunctionNodeFactory.Function(NodeTypes.SmoothstepFloatFloatVec2, NodeTypes.SmoothstepFloatFloatVec2,
                "smoothstep",
                new[] {new Pin("edge0", PinTypes.Float), new Pin("edge1", PinTypes.Float), new Pin("x", PinTypes.Vec2)},
                PinTypes.Vec2));
            Add(FunctionNodeFactory.Function(NodeTypes.SmoothstepFloatFloatVec3, NodeTypes.SmoothstepFloatFloatVec3,
                "smoothstep",
                new[] {new Pin("edge0", PinTypes.Float), new Pin("edge1", PinTypes.Float), new Pin("x", PinTypes.Vec3)},
                PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.SmoothstepFloatFloatVec4, NodeTypes.SmoothstepFloatFloatVec4,
                "smoothstep",
                new[] {new Pin("edge0", PinTypes.Float), new Pin("edge1", PinTypes.Float), new Pin("x", PinTypes.Vec4)},
                PinTypes.Vec4));
            Add(FunctionNodeFactory.Function(NodeTypes.LengthFloat, NodeTypes.LengthFloat, "length",
                new[] {new Pin(PinIds.X, PinTypes.Float)}, PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.LengthVec2, NodeTypes.LengthVec2, "length",
                new[] {new Pin(PinIds.X, PinTypes.Vec2)}, PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.LengthVec3, NodeTypes.LengthVec3, "length",
                new[] {new Pin(PinIds.X, PinTypes.Vec3)}, PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.LengthVec4, NodeTypes.LengthVec4, "length",
                new[] {new Pin(PinIds.X, PinTypes.Vec4)}, PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.DistanceFloatFloat, NodeTypes.DistanceFloatFloat, "distance",
                new[] {new Pin("p0", PinTypes.Float), new Pin("p1", PinTypes.Float)},
                PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.DistanceVec2Vec2, NodeTypes.DistanceVec2Vec2, "distance",
                new[] {new Pin("p0", PinTypes.Vec2), new Pin("p1", PinTypes.Vec2)},
                PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.DistanceVec3Vec3, NodeTypes.DistanceVec3Vec3, "distance",
                new[] {new Pin("p0", PinTypes.Vec3), new Pin("p1", PinTypes.Vec3)},
                PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.DistanceVec4Vec4, NodeTypes.DistanceVec4Vec4, "distance",
                new[] {new Pin("p0", PinTypes.Vec4), new Pin("p1", PinTypes.Vec4)},
                PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.DotFloatFloat, NodeTypes.DotFloatFloat, "dot",
                new[] {new Pin(PinIds.X, PinTypes.Float), new Pin(PinIds.Y, PinTypes.Float)},
                PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.DotVec2Vec2, NodeTypes.DotVec2Vec2, "dot",
                new[] {new Pin(PinIds.X, PinTypes.Vec2), new Pin(PinIds.Y, PinTypes.Vec2)}, PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.DotVec3Vec3, NodeTypes.DotVec3Vec3, "dot",
                new[] {new Pin(PinIds.X, PinTypes.Vec3), new Pin(PinIds.Y, PinTypes.Vec3)}, PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.DotVec4Vec4, NodeTypes.DotVec4Vec4, "dot",
                new[] {new Pin(PinIds.X, PinTypes.Vec4), new Pin(PinIds.Y, PinTypes.Vec4)}, PinTypes.Float));

            Add(FunctionNodeFactory.Function(NodeTypes.MultiplyFloatFloat, NodeTypes.MultiplyFloatFloat,
                NodeTypes.SubCategories.Arithmetic, "multiply",
                new[] {new Pin(PinIds.X, PinTypes.Float), new Pin(PinIds.Y, PinTypes.Float)},
                PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.MultiplyVec2Vec2, NodeTypes.MultiplyVec2Vec2,
                NodeTypes.SubCategories.Arithmetic, "multiply",
                new[] {new Pin(PinIds.X, PinTypes.Vec2), new Pin(PinIds.Y, PinTypes.Vec2)}, PinTypes.Vec2));
            Add(FunctionNodeFactory.Function(NodeTypes.MultiplyVec3Vec3, NodeTypes.MultiplyVec3Vec3,
                NodeTypes.SubCategories.Arithmetic, "multiply",
                new[] {new Pin(PinIds.X, PinTypes.Vec3), new Pin(PinIds.Y, PinTypes.Vec3)}, PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.MultiplyVec4Vec4, NodeTypes.MultiplyVec4Vec4,
                NodeTypes.SubCategories.Arithmetic, "multiply",
                new[] {new Pin(PinIds.X, PinTypes.Vec4), new Pin(PinIds.Y, PinTypes.Vec4)}, PinTypes.Vec4));

            Add(FunctionNodeFactory.Function(NodeTypes.MultiplyVec4Mat4, NodeTypes.MultiplyVec4Mat4,
                NodeTypes.SubCategories.Arithmetic, "multiply",
                new[] {new Pin("x", PinTypes.Vec4), new Pin(PinIds.Y, PinTypes.Mat4)},
                new[] {new Pin("", PinTypes.Vec4)}));

            Add(FunctionNodeFactory.Function(NodeTypes.MultiplyVec2Mat2, NodeTypes.MultiplyVec2Mat2,
                NodeTypes.SubCategories.Arithmetic, "multiply",
                new[] { new Pin("x", PinTypes.Vec2), new Pin(PinIds.Y, PinTypes.Mat2) },
                new[] { new Pin("", PinTypes.Vec2) }));

            Add(FunctionNodeFactory.Function(NodeTypes.MultiplyMat4Float, NodeTypes.MultiplyMat4Float,
                NodeTypes.SubCategories.Arithmetic, "multiply",
                new[] {new Pin("x", PinTypes.Mat4), new Pin(PinIds.Y, PinTypes.Float)},
                new[] {new Pin("", PinTypes.Mat4)}));
            Add(FunctionNodeFactory.Function(NodeTypes.MultiplyMat4x3Float, NodeTypes.MultiplyMat4x3Float,
                NodeTypes.SubCategories.Arithmetic, "multiply",
                new[] {new Pin("x", PinTypes.Mat4x3), new Pin(PinIds.Y, PinTypes.Float)},
                new[] {new Pin("", PinTypes.Mat4x3)}));

            Add(FunctionNodeFactory.Function(NodeTypes.MultiplyVec4Mat4x3, NodeTypes.MultiplyVec4Mat4x3,
                NodeTypes.SubCategories.Arithmetic, "multiply",
                new[] {new Pin("x", PinTypes.Vec4), new Pin(PinIds.Y, PinTypes.Mat4x3)},
                new[] {new Pin("", PinTypes.Vec3)}));

            Add(FunctionNodeFactory.Function(NodeTypes.MultiplyVec3Mat3, NodeTypes.MultiplyVec3Mat3,
                NodeTypes.SubCategories.Arithmetic, "multiply",
                new[] {new Pin("x", PinTypes.Vec3), new Pin(PinIds.Y, PinTypes.Mat3)},
                new[] {new Pin("", PinTypes.Vec3)}));
            Add(FunctionNodeFactory.Function(NodeTypes.MultiplyMat3Vec3, NodeTypes.MultiplyMat3Vec3,
                NodeTypes.SubCategories.Arithmetic, "multiply",
                new[] {new Pin("x", PinTypes.Mat3), new Pin(PinIds.Y, PinTypes.Vec3)},
                new[] {new Pin("", PinTypes.Vec3)}));
            Add(FunctionNodeFactory.Function(NodeTypes.MultiplyVec2Float, NodeTypes.MultiplyVec2Float,
                NodeTypes.SubCategories.Arithmetic, "multiply",
                new[] {new Pin(PinIds.X, PinTypes.Vec2), new Pin(PinIds.Y, PinTypes.Float)}, PinTypes.Vec2));
            Add(FunctionNodeFactory.Function(NodeTypes.MultiplyVec3Float, NodeTypes.MultiplyVec3Float,
                NodeTypes.SubCategories.Arithmetic, "multiply",
                new[] {new Pin(PinIds.X, PinTypes.Vec3), new Pin(PinIds.Y, PinTypes.Float)}, PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.MultiplyVec4Float, NodeTypes.MultiplyVec4Float,
                NodeTypes.SubCategories.Arithmetic, "multiply",
                new[] {new Pin(PinIds.X, PinTypes.Vec4), new Pin(PinIds.Y, PinTypes.Float)}, PinTypes.Vec4));

            Add(FunctionNodeFactory.Function(NodeTypes.DivideFloatFloat, NodeTypes.DivideFloatFloat,
                NodeTypes.SubCategories.Arithmetic, "divide",
                new[] {new Pin(PinIds.X, PinTypes.Float), new Pin(PinIds.Y, PinTypes.Float)},
                PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.DivideVec2Vec2, NodeTypes.DivideVec2Vec2,
                NodeTypes.SubCategories.Arithmetic, "divide",
                new[] {new Pin(PinIds.X, PinTypes.Vec2), new Pin(PinIds.Y, PinTypes.Vec2)}, PinTypes.Vec2));
            Add(FunctionNodeFactory.Function(NodeTypes.DivideVec3Vec3, NodeTypes.DivideVec3Vec3,
                NodeTypes.SubCategories.Arithmetic, "divide",
                new[] {new Pin(PinIds.X, PinTypes.Vec3), new Pin(PinIds.Y, PinTypes.Vec3)}, PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.DivideVec4Vec4, NodeTypes.DivideVec4Vec4,
                NodeTypes.SubCategories.Arithmetic, "divide",
                new[] {new Pin(PinIds.X, PinTypes.Vec4), new Pin(PinIds.Y, PinTypes.Vec4)}, PinTypes.Vec4));
            Add(FunctionNodeFactory.Function(NodeTypes.DivideVec2Float, NodeTypes.DivideVec2Float,
                NodeTypes.SubCategories.Arithmetic, "divide",
                new[] {new Pin(PinIds.X, PinTypes.Vec2), new Pin(PinIds.Y, PinTypes.Float)}, PinTypes.Vec2));
            Add(FunctionNodeFactory.Function(NodeTypes.DivideVec3Float, NodeTypes.DivideVec3Float,
                NodeTypes.SubCategories.Arithmetic, "divide",
                new[] {new Pin(PinIds.X, PinTypes.Vec3), new Pin(PinIds.Y, PinTypes.Float)}, PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.DivideVec4Float, NodeTypes.DivideVec4Float,
                NodeTypes.SubCategories.Arithmetic, "divide",
                new[] {new Pin(PinIds.X, PinTypes.Vec4), new Pin(PinIds.Y, PinTypes.Float)}, PinTypes.Vec4));

            Add(FunctionNodeFactory.Function(NodeTypes.AddFloatFloat, NodeTypes.AddFloatFloat,
                NodeTypes.SubCategories.Arithmetic, "add",
                new[] {new Pin(PinIds.X, PinTypes.Float), new Pin(PinIds.Y, PinTypes.Float)},
                PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.AddVec2Vec2, NodeTypes.AddVec2Vec2,
                NodeTypes.SubCategories.Arithmetic, "add",
                new[] {new Pin(PinIds.X, PinTypes.Vec2), new Pin(PinIds.Y, PinTypes.Vec2)}, PinTypes.Vec2));
            Add(FunctionNodeFactory.Function(NodeTypes.AddVec3Vec3, NodeTypes.AddVec3Vec3,
                NodeTypes.SubCategories.Arithmetic, "add",
                new[] {new Pin(PinIds.X, PinTypes.Vec3), new Pin(PinIds.Y, PinTypes.Vec3)}, PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.AddVec4Vec4, NodeTypes.AddVec4Vec4,
                NodeTypes.SubCategories.Arithmetic, "add",
                new[] {new Pin(PinIds.X, PinTypes.Vec4), new Pin(PinIds.Y, PinTypes.Vec4)}, PinTypes.Vec4));
            Add(FunctionNodeFactory.Function(NodeTypes.AddMat4Mat4, NodeTypes.AddMat4Mat4,
                NodeTypes.SubCategories.Arithmetic, "add",
                new[] {new Pin(PinIds.X, PinTypes.Mat4), new Pin(PinIds.Y, PinTypes.Mat4)}, PinTypes.Mat4));
            Add(FunctionNodeFactory.Function(NodeTypes.AddMat4x3Mat4x3, NodeTypes.AddMat4x3Mat4x3,
                NodeTypes.SubCategories.Arithmetic, "add",
                new[] {new Pin(PinIds.X, PinTypes.Mat4x3), new Pin(PinIds.Y, PinTypes.Mat4x3)}, PinTypes.Mat4x3));

            Add(FunctionNodeFactory.Function(NodeTypes.SubtractFloatFloat, NodeTypes.SubtractFloatFloat,
                NodeTypes.SubCategories.Arithmetic, "subtract",
                new[] {new Pin(PinIds.X, PinTypes.Float), new Pin(PinIds.Y, PinTypes.Float)},
                PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.SubtractVec2Vec2, NodeTypes.SubtractVec2Vec2,
                NodeTypes.SubCategories.Arithmetic, "subtract",
                new[] {new Pin(PinIds.X, PinTypes.Vec2), new Pin(PinIds.Y, PinTypes.Vec2)}, PinTypes.Vec2));
            Add(FunctionNodeFactory.Function(NodeTypes.SubtractVec3Vec3, NodeTypes.SubtractVec3Vec3,
                NodeTypes.SubCategories.Arithmetic, "subtract",
                new[] {new Pin(PinIds.X, PinTypes.Vec3), new Pin(PinIds.Y, PinTypes.Vec3)}, PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.SubtractVec4Vec4, NodeTypes.SubtractVec4Vec4,
                NodeTypes.SubCategories.Arithmetic, "subtract",
                new[] {new Pin(PinIds.X, PinTypes.Vec4), new Pin(PinIds.Y, PinTypes.Vec4)}, PinTypes.Vec4));

            Add(FunctionNodeFactory.Function(NodeTypes.MinusFloat, NodeTypes.MinusFloat,
                NodeTypes.SubCategories.Arithmetic, "minus",
                new[] { new Pin(PinIds.X, PinTypes.Float) }, PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.MinusVec2, NodeTypes.MinusVec2,
                NodeTypes.SubCategories.Arithmetic, "minus",
                new[] { new Pin(PinIds.X, PinTypes.Vec2) }, PinTypes.Vec2));
            Add(FunctionNodeFactory.Function(NodeTypes.MinusVec3, NodeTypes.MinusVec3,
                NodeTypes.SubCategories.Arithmetic, "minus",
                new[] { new Pin(PinIds.X, PinTypes.Vec3) }, PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.MinusVec4, NodeTypes.MinusVec4,
                NodeTypes.SubCategories.Arithmetic, "minus",
                new[] { new Pin(PinIds.X, PinTypes.Vec4) }, PinTypes.Vec4));

            Add(FunctionNodeFactory.Function(NodeTypes.SaturateFloat, NodeTypes.SaturateFloat,
                NodeTypes.SubCategories.Arithmetic, "saturate",
                new[] { new Pin(PinIds.X, PinTypes.Float) }, PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.SaturateVec2, NodeTypes.SaturateVec2,
                NodeTypes.SubCategories.Arithmetic, "saturate",
                new[] { new Pin(PinIds.X, PinTypes.Vec2) }, PinTypes.Vec2));
            Add(FunctionNodeFactory.Function(NodeTypes.SaturateVec3, NodeTypes.SaturateVec3,
                NodeTypes.SubCategories.Arithmetic, "saturate",
                new[] { new Pin(PinIds.X, PinTypes.Vec3) }, PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.SaturateVec4, NodeTypes.SaturateVec4,
                NodeTypes.SubCategories.Arithmetic, "saturate",
                new[] { new Pin(PinIds.X, PinTypes.Vec4) }, PinTypes.Vec4));

            Add(FunctionNodeFactory.Function(NodeTypes.CrossVec3Vec3, NodeTypes.CrossVec3Vec3, "cross",
                new[] {new Pin(PinIds.X, PinTypes.Vec3), new Pin(PinIds.Y, PinTypes.Vec3)}, PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.NormalizeFloat, NodeTypes.NormalizeFloat, "normalize",
                new[] {new Pin(PinIds.X, PinTypes.Float)}, PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.NormalizeVec2, NodeTypes.NormalizeVec2, "normalize",
                new[] {new Pin(PinIds.X, PinTypes.Vec2)}, PinTypes.Vec2));
            Add(FunctionNodeFactory.Function(NodeTypes.NormalizeVec3, NodeTypes.NormalizeVec3, "normalize",
                new[] {new Pin(PinIds.X, PinTypes.Vec3)}, PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.NormalizeVec4, NodeTypes.NormalizeVec4, "normalize",
                new[] {new Pin(PinIds.X, PinTypes.Vec4)}, PinTypes.Vec4));
            Add(FunctionNodeFactory.Function(NodeTypes.FaceforwardFloatFloatFloat, NodeTypes.FaceforwardFloatFloatFloat,
                "faceforward",
                new[] {new Pin("N", PinTypes.Float), new Pin("I", PinTypes.Float), new Pin("Nref", PinTypes.Float)},
                PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.FaceforwardVec2Vec2Vec2, NodeTypes.FaceforwardVec2Vec2Vec2,
                "faceforward",
                new[] {new Pin("N", PinTypes.Vec2), new Pin("I", PinTypes.Vec2), new Pin("Nref", PinTypes.Vec2)},
                PinTypes.Vec2));
            Add(FunctionNodeFactory.Function(NodeTypes.FaceforwardVec3Vec3Vec3, NodeTypes.FaceforwardVec3Vec3Vec3,
                "faceforward",
                new[] {new Pin("N", PinTypes.Vec3), new Pin("I", PinTypes.Vec3), new Pin("Nref", PinTypes.Vec3)},
                PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.FaceforwardVec4Vec4Vec4, NodeTypes.FaceforwardVec4Vec4Vec4,
                "faceforward",
                new[] {new Pin("N", PinTypes.Vec4), new Pin("I", PinTypes.Vec4), new Pin("Nref", PinTypes.Vec4)},
                PinTypes.Vec4));
            Add(FunctionNodeFactory.Function(NodeTypes.ReflectFloatFloat, NodeTypes.ReflectFloatFloat, "reflect",
                new[] {new Pin("I", PinTypes.Float), new Pin("N", PinTypes.Float)},
                PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.ReflectVec2Vec2, NodeTypes.ReflectVec2Vec2, "reflect",
                new[] {new Pin("I", PinTypes.Vec2), new Pin("N", PinTypes.Vec2)}, PinTypes.Vec2));
            Add(FunctionNodeFactory.Function(NodeTypes.ReflectVec3Vec3, NodeTypes.ReflectVec3Vec3, "reflect",
                new[] {new Pin("I", PinTypes.Vec3), new Pin("N", PinTypes.Vec3)}, PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.ReflectVec4Vec4, NodeTypes.ReflectVec4Vec4, "reflect",
                new[] {new Pin("I", PinTypes.Vec4), new Pin("N", PinTypes.Vec4)}, PinTypes.Vec4));
            Add(FunctionNodeFactory.Function(NodeTypes.RefractFloatFloatFloat, NodeTypes.RefractFloatFloatFloat,
                "refract",
                new[] {new Pin("I", PinTypes.Float), new Pin("N", PinTypes.Float), new Pin("eta", PinTypes.Float)},
                PinTypes.Float));
            Add(FunctionNodeFactory.Function(NodeTypes.RefractVec2Vec2Float, NodeTypes.RefractVec2Vec2Float, "refract",
                new[] {new Pin("I", PinTypes.Vec2), new Pin("N", PinTypes.Vec2), new Pin("eta", PinTypes.Float)},
                PinTypes.Vec2));
            Add(FunctionNodeFactory.Function(NodeTypes.RefractVec3Vec3Float, NodeTypes.RefractVec3Vec3Float, "refract",
                new[] {new Pin("I", PinTypes.Vec3), new Pin("N", PinTypes.Vec3), new Pin("eta", PinTypes.Float)},
                PinTypes.Vec3));
            Add(FunctionNodeFactory.Function(NodeTypes.RefractVec4Vec4Float, NodeTypes.RefractVec4Vec4Float, "refract",
                new[] {new Pin("I", PinTypes.Vec4), new Pin("N", PinTypes.Vec4), new Pin("eta", PinTypes.Float)},
                PinTypes.Vec4));
            Add(FunctionNodeFactory.Function(NodeTypes.MatrixCompMultMat2Mat2, NodeTypes.MatrixCompMultMat2Mat2,
                "matrixCompMult", new[] {new Pin(PinIds.X, PinTypes.Mat2), new Pin(PinIds.Y, PinTypes.Mat2)},
                new[] {new Pin("", PinTypes.Mat2)}));
            Add(FunctionNodeFactory.Function(NodeTypes.MatrixCompMultMat3Mat3, NodeTypes.MatrixCompMultMat3Mat3,
                "matrixCompMult", new[] {new Pin(PinIds.X, PinTypes.Mat3), new Pin(PinIds.Y, PinTypes.Mat3)},
                new[] {new Pin("", PinTypes.Mat3)}));
            Add(FunctionNodeFactory.Function(NodeTypes.MatrixCompMultMat4Mat4, NodeTypes.MatrixCompMultMat4Mat4,
                "matrixCompMult", new[] {new Pin(PinIds.X, PinTypes.Mat4), new Pin(PinIds.Y, PinTypes.Mat4)},
                new[] {new Pin("", PinTypes.Mat4)}));

            Add(FunctionNodeFactory.Function(NodeTypes.LessThanFloatFloat, NodeTypes.LessThanFloatFloat,
                NodeTypes.SubCategories.Logic, "lessThan",
                new[] {new Pin(PinIds.X, PinTypes.Float), new Pin(PinIds.Y, PinTypes.Float)},
                new[] {new Pin("", PinTypes.Bool)}));
            Add(FunctionNodeFactory.Function(NodeTypes.LessThanVec2Vec2, NodeTypes.LessThanVec2Vec2,
                NodeTypes.SubCategories.Logic, "lessThan",
                new[] {new Pin(PinIds.X, PinTypes.Vec2), new Pin(PinIds.Y, PinTypes.Vec2)},
                new[] {new Pin("", PinTypes.Bvec2)}));
            Add(FunctionNodeFactory.Function(NodeTypes.LessThanVec3Vec3, NodeTypes.LessThanVec3Vec3,
                NodeTypes.SubCategories.Logic, "lessThan",
                new[] {new Pin(PinIds.X, PinTypes.Vec3), new Pin(PinIds.Y, PinTypes.Vec3)},
                new[] {new Pin("", PinTypes.Bvec3)}));
            Add(FunctionNodeFactory.Function(NodeTypes.LessThanVec4Vec4, NodeTypes.LessThanVec4Vec4,
                NodeTypes.SubCategories.Logic, "lessThan",
                new[] {new Pin(PinIds.X, PinTypes.Vec4), new Pin(PinIds.Y, PinTypes.Vec4)},
                new[] {new Pin("", PinTypes.Bvec4)}));
            Add(FunctionNodeFactory.Function(NodeTypes.LessThanIvec2Ivec2, NodeTypes.LessThanIvec2Ivec2,
                NodeTypes.SubCategories.Logic, "lessThan",
                new[] {new Pin(PinIds.X, PinTypes.Ivec2), new Pin(PinIds.Y, PinTypes.Ivec2)},
                new[] {new Pin("", PinTypes.Bvec2)}));
            Add(FunctionNodeFactory.Function(NodeTypes.LessThanIvec3Ivec3, NodeTypes.LessThanIvec3Ivec3,
                NodeTypes.SubCategories.Logic, "lessThan",
                new[] {new Pin(PinIds.X, PinTypes.Ivec3), new Pin(PinIds.Y, PinTypes.Ivec3)},
                new[] {new Pin("", PinTypes.Bvec3)}));
            Add(FunctionNodeFactory.Function(NodeTypes.LessThanIvec4Ivec4, NodeTypes.LessThanIvec4Ivec4,
                NodeTypes.SubCategories.Logic, "lessThan",
                new[] {new Pin(PinIds.X, PinTypes.Ivec4), new Pin(PinIds.Y, PinTypes.Ivec4)},
                new[] {new Pin("", PinTypes.Bvec4)}));

            Add(FunctionNodeFactory.Function(NodeTypes.LessThanEqualFloatFloat, NodeTypes.LessThanEqualFloatFloat,
                NodeTypes.SubCategories.Logic, "lessThanEqual",
                new[] {new Pin(PinIds.X, PinTypes.Float), new Pin(PinIds.Y, PinTypes.Float)},
                new[] {new Pin("", PinTypes.Bool)}));
            Add(FunctionNodeFactory.Function(NodeTypes.LessThanEqualVec2Vec2, NodeTypes.LessThanEqualVec2Vec2,
                NodeTypes.SubCategories.Logic, "lessThanEqual",
                new[] {new Pin(PinIds.X, PinTypes.Vec2), new Pin(PinIds.Y, PinTypes.Vec2)},
                new[] {new Pin("", PinTypes.Bvec2)}));
            Add(FunctionNodeFactory.Function(NodeTypes.LessThanEqualVec3Vec3, NodeTypes.LessThanEqualVec3Vec3,
                NodeTypes.SubCategories.Logic, "lessThanEqual",
                new[] {new Pin(PinIds.X, PinTypes.Vec3), new Pin(PinIds.Y, PinTypes.Vec3)},
                new[] {new Pin("", PinTypes.Bvec3)}));
            Add(FunctionNodeFactory.Function(NodeTypes.LessThanEqualVec4Vec4, NodeTypes.LessThanEqualVec4Vec4,
                NodeTypes.SubCategories.Logic, "lessThanEqual",
                new[] {new Pin(PinIds.X, PinTypes.Vec4), new Pin(PinIds.Y, PinTypes.Vec4)},
                new[] {new Pin("", PinTypes.Bvec4)}));
            Add(FunctionNodeFactory.Function(NodeTypes.LessThanEqualIvec2Ivec2, NodeTypes.LessThanEqualIvec2Ivec2,
                NodeTypes.SubCategories.Logic, "lessThanEqual",
                new[] {new Pin(PinIds.X, PinTypes.Ivec2), new Pin(PinIds.Y, PinTypes.Ivec2)},
                new[] {new Pin("", PinTypes.Bvec2)}));
            Add(FunctionNodeFactory.Function(NodeTypes.LessThanEqualIvec3Ivec3, NodeTypes.LessThanEqualIvec3Ivec3,
                NodeTypes.SubCategories.Logic, "lessThanEqual",
                new[] {new Pin(PinIds.X, PinTypes.Ivec3), new Pin(PinIds.Y, PinTypes.Ivec3)},
                new[] {new Pin("", PinTypes.Bvec3)}));
            Add(FunctionNodeFactory.Function(NodeTypes.LessThanEqualIvec4Ivec4, NodeTypes.LessThanEqualIvec4Ivec4,
                NodeTypes.SubCategories.Logic, "lessThanEqual",
                new[] {new Pin(PinIds.X, PinTypes.Ivec4), new Pin(PinIds.Y, PinTypes.Ivec4)},
                new[] {new Pin("", PinTypes.Bvec4)}));

            Add(FunctionNodeFactory.Function(NodeTypes.GreaterThanFloatFloat, NodeTypes.GreaterThanFloatFloat,
                NodeTypes.SubCategories.Logic, "greaterThan",
                new[] {new Pin(PinIds.X, PinTypes.Float), new Pin(PinIds.Y, PinTypes.Float)},
                new[] {new Pin("", PinTypes.Bool)}));
            Add(FunctionNodeFactory.Function(NodeTypes.GreaterThanVec2Vec2, NodeTypes.GreaterThanVec2Vec2,
                NodeTypes.SubCategories.Logic, "greaterThan",
                new[] {new Pin(PinIds.X, PinTypes.Vec2), new Pin(PinIds.Y, PinTypes.Vec2)},
                new[] {new Pin("", PinTypes.Bvec2)}));
            Add(FunctionNodeFactory.Function(NodeTypes.GreaterThanVec3Vec3, NodeTypes.GreaterThanVec3Vec3,
                NodeTypes.SubCategories.Logic, "greaterThan",
                new[] {new Pin(PinIds.X, PinTypes.Vec3), new Pin(PinIds.Y, PinTypes.Vec3)},
                new[] {new Pin("", PinTypes.Bvec3)}));
            Add(FunctionNodeFactory.Function(NodeTypes.GreaterThanVec4Vec4, NodeTypes.GreaterThanVec4Vec4,
                NodeTypes.SubCategories.Logic, "greaterThan",
                new[] {new Pin(PinIds.X, PinTypes.Vec4), new Pin(PinIds.Y, PinTypes.Vec4)},
                new[] {new Pin("", PinTypes.Bvec4)}));
            Add(FunctionNodeFactory.Function(NodeTypes.GreaterThanIvec2Ivec2, NodeTypes.GreaterThanIvec2Ivec2,
                NodeTypes.SubCategories.Logic, "greaterThan",
                new[] {new Pin(PinIds.X, PinTypes.Ivec2), new Pin(PinIds.Y, PinTypes.Ivec2)},
                new[] {new Pin("", PinTypes.Bvec2)}));
            Add(FunctionNodeFactory.Function(NodeTypes.GreaterThanIvec3Ivec3, NodeTypes.GreaterThanIvec3Ivec3,
                NodeTypes.SubCategories.Logic, "greaterThan",
                new[] {new Pin(PinIds.X, PinTypes.Ivec3), new Pin(PinIds.Y, PinTypes.Ivec3)},
                new[] {new Pin("", PinTypes.Bvec3)}));
            Add(FunctionNodeFactory.Function(NodeTypes.GreaterThanIvec4Ivec4, NodeTypes.GreaterThanIvec4Ivec4,
                NodeTypes.SubCategories.Logic, "greaterThan",
                new[] {new Pin(PinIds.X, PinTypes.Ivec4), new Pin(PinIds.Y, PinTypes.Ivec4)},
                new[] {new Pin("", PinTypes.Bvec4)}));

            Add(FunctionNodeFactory.Function(NodeTypes.GreaterThanEqualFloatFloat, NodeTypes.GreaterThanEqualFloatFloat,
                NodeTypes.SubCategories.Logic, "greaterThanEqual",
                new[] {new Pin(PinIds.X, PinTypes.Float), new Pin(PinIds.Y, PinTypes.Float)},
                new[] {new Pin("", PinTypes.Bool)}));
            Add(FunctionNodeFactory.Function(NodeTypes.GreaterThanEqualVec2Vec2, NodeTypes.GreaterThanEqualVec2Vec2,
                NodeTypes.SubCategories.Logic, "greaterThanEqual",
                new[] {new Pin(PinIds.X, PinTypes.Vec2), new Pin(PinIds.Y, PinTypes.Vec2)},
                new[] {new Pin("", PinTypes.Bvec2)}));
            Add(FunctionNodeFactory.Function(NodeTypes.GreaterThanEqualVec3Vec3, NodeTypes.GreaterThanEqualVec3Vec3,
                NodeTypes.SubCategories.Logic, "greaterThanEqual",
                new[] {new Pin(PinIds.X, PinTypes.Vec3), new Pin(PinIds.Y, PinTypes.Vec3)},
                new[] {new Pin("", PinTypes.Bvec3)}));
            Add(FunctionNodeFactory.Function(NodeTypes.GreaterThanEqualVec4Vec4, NodeTypes.GreaterThanEqualVec4Vec4,
                NodeTypes.SubCategories.Logic, "greaterThanEqual",
                new[] {new Pin(PinIds.X, PinTypes.Vec4), new Pin(PinIds.Y, PinTypes.Vec4)},
                new[] {new Pin("", PinTypes.Bvec4)}));
            Add(FunctionNodeFactory.Function(NodeTypes.GreaterThanEqualIvec2Ivec2, NodeTypes.GreaterThanEqualIvec2Ivec2,
                NodeTypes.SubCategories.Logic, "greaterThanEqual",
                new[] {new Pin(PinIds.X, PinTypes.Ivec2), new Pin(PinIds.Y, PinTypes.Ivec2)},
                new[] {new Pin("", PinTypes.Bvec2)}));
            Add(FunctionNodeFactory.Function(NodeTypes.GreaterThanEqualIvec3Ivec3, NodeTypes.GreaterThanEqualIvec3Ivec3,
                NodeTypes.SubCategories.Logic, "greaterThanEqual",
                new[] {new Pin(PinIds.X, PinTypes.Ivec3), new Pin(PinIds.Y, PinTypes.Ivec3)},
                new[] {new Pin("", PinTypes.Bvec3)}));
            Add(FunctionNodeFactory.Function(NodeTypes.GreaterThanEqualIvec4Ivec4, NodeTypes.GreaterThanEqualIvec4Ivec4,
                NodeTypes.SubCategories.Logic, "greaterThanEqual",
                new[] {new Pin(PinIds.X, PinTypes.Ivec4), new Pin(PinIds.Y, PinTypes.Ivec4)},
                new[] {new Pin("", PinTypes.Bvec4)}));

            Add(FunctionNodeFactory.Function(NodeTypes.EqualFloatFloat, NodeTypes.EqualFloatFloat,
                NodeTypes.SubCategories.Logic, "equal",
                new[] {new Pin(PinIds.X, PinTypes.Float), new Pin(PinIds.Y, PinTypes.Float)},
                new[] {new Pin("", PinTypes.Bool)}));
            Add(FunctionNodeFactory.Function(NodeTypes.EqualVec2Vec2, NodeTypes.EqualVec2Vec2,
                NodeTypes.SubCategories.Logic, "equal",
                new[] {new Pin(PinIds.X, PinTypes.Vec2), new Pin(PinIds.Y, PinTypes.Vec2)},
                new[] {new Pin("", PinTypes.Bvec2)}));
            Add(FunctionNodeFactory.Function(NodeTypes.EqualVec3Vec3, NodeTypes.EqualVec3Vec3,
                NodeTypes.SubCategories.Logic, "equal",
                new[] {new Pin(PinIds.X, PinTypes.Vec3), new Pin(PinIds.Y, PinTypes.Vec3)},
                new[] {new Pin("", PinTypes.Bvec3)}));
            Add(FunctionNodeFactory.Function(NodeTypes.EqualVec4Vec4, NodeTypes.EqualVec4Vec4,
                NodeTypes.SubCategories.Logic, "equal",
                new[] {new Pin(PinIds.X, PinTypes.Vec4), new Pin(PinIds.Y, PinTypes.Vec4)},
                new[] {new Pin("", PinTypes.Bvec4)}));
            Add(FunctionNodeFactory.Function(NodeTypes.EqualIvec2Ivec2, NodeTypes.EqualIvec2Ivec2,
                NodeTypes.SubCategories.Logic, "equal",
                new[] {new Pin(PinIds.X, PinTypes.Ivec2), new Pin(PinIds.Y, PinTypes.Ivec2)},
                new[] {new Pin("", PinTypes.Bvec2)}));
            Add(FunctionNodeFactory.Function(NodeTypes.EqualIvec3Ivec3, NodeTypes.EqualIvec3Ivec3,
                NodeTypes.SubCategories.Logic, "equal",
                new[] {new Pin(PinIds.X, PinTypes.Ivec3), new Pin(PinIds.Y, PinTypes.Ivec3)},
                new[] {new Pin("", PinTypes.Bvec3)}));
            Add(FunctionNodeFactory.Function(NodeTypes.EqualIvec4Ivec4, NodeTypes.EqualIvec4Ivec4,
                NodeTypes.SubCategories.Logic, "equal",
                new[] {new Pin(PinIds.X, PinTypes.Ivec4), new Pin(PinIds.Y, PinTypes.Ivec4)},
                new[] {new Pin("", PinTypes.Bvec4)}));

            Add(FunctionNodeFactory.Function(NodeTypes.NotEqualFloatFloat, NodeTypes.NotEqualFloatFloat,
                NodeTypes.SubCategories.Logic, "notEqual",
                new[] {new Pin(PinIds.X, PinTypes.Float), new Pin(PinIds.Y, PinTypes.Float)},
                new[] {new Pin("", PinTypes.Bool)}));
            Add(FunctionNodeFactory.Function(NodeTypes.NotEqualVec2Vec2, NodeTypes.NotEqualVec2Vec2,
                NodeTypes.SubCategories.Logic, "notEqual",
                new[] {new Pin(PinIds.X, PinTypes.Vec2), new Pin(PinIds.Y, PinTypes.Vec2)},
                new[] {new Pin("", PinTypes.Bvec2)}));
            Add(FunctionNodeFactory.Function(NodeTypes.NotEqualVec3Vec3, NodeTypes.NotEqualVec3Vec3,
                NodeTypes.SubCategories.Logic, "notEqual",
                new[] {new Pin(PinIds.X, PinTypes.Vec3), new Pin(PinIds.Y, PinTypes.Vec3)},
                new[] {new Pin("", PinTypes.Bvec3)}));
            Add(FunctionNodeFactory.Function(NodeTypes.NotEqualVec4Vec4, NodeTypes.NotEqualVec4Vec4,
                NodeTypes.SubCategories.Logic, "notEqual",
                new[] {new Pin(PinIds.X, PinTypes.Vec4), new Pin(PinIds.Y, PinTypes.Vec4)},
                new[] {new Pin("", PinTypes.Bvec4)}));
            Add(FunctionNodeFactory.Function(NodeTypes.NotEqualIvec2Ivec2, NodeTypes.NotEqualIvec2Ivec2,
                NodeTypes.SubCategories.Logic, "notEqual",
                new[] {new Pin(PinIds.X, PinTypes.Ivec2), new Pin(PinIds.Y, PinTypes.Ivec2)},
                new[] {new Pin("", PinTypes.Bvec2)}));
            Add(FunctionNodeFactory.Function(NodeTypes.NotEqualIvec3Ivec3, NodeTypes.NotEqualIvec3Ivec3,
                NodeTypes.SubCategories.Logic, "notEqual",
                new[] {new Pin(PinIds.X, PinTypes.Ivec3), new Pin(PinIds.Y, PinTypes.Ivec3)},
                new[] {new Pin("", PinTypes.Bvec3)}));
            Add(FunctionNodeFactory.Function(NodeTypes.NotEqualIvec4Ivec4, NodeTypes.NotEqualIvec4Ivec4,
                NodeTypes.SubCategories.Logic, "notEqual",
                new[] {new Pin(PinIds.X, PinTypes.Ivec4), new Pin(PinIds.Y, PinTypes.Ivec4)},
                new[] {new Pin("", PinTypes.Bvec4)}));

            Add(FunctionNodeFactory.Function(NodeTypes.AndAlso, NodeTypes.AndAlso, NodeTypes.SubCategories.Logic,
                new[] {new Pin(PinIds.X, PinTypes.Bool), new Pin(PinIds.Y, PinTypes.Bool)},
                new[] {new Pin("", PinTypes.Bool)}));
            Add(FunctionNodeFactory.Function(NodeTypes.OrElse, NodeTypes.OrElse, NodeTypes.SubCategories.Logic,
                new[] {new Pin(PinIds.X, PinTypes.Bool), new Pin(PinIds.Y, PinTypes.Bool)},
                new[] {new Pin("", PinTypes.Bool)}));

            Add(FunctionNodeFactory.Function(NodeTypes.AnyBvec2, NodeTypes.AnyBvec2, NodeTypes.SubCategories.Logic,
                "any",
                new[] {new Pin(PinIds.X, PinTypes.Bvec2)}, new[] {new Pin("", PinTypes.Bool)}));
            Add(FunctionNodeFactory.Function(NodeTypes.AnyBvec3, NodeTypes.AnyBvec3, NodeTypes.SubCategories.Logic,
                "any",
                new[] {new Pin(PinIds.X, PinTypes.Bvec3)}, new[] {new Pin("", PinTypes.Bool)}));
            Add(FunctionNodeFactory.Function(NodeTypes.AnyBvec4, NodeTypes.AnyBvec4, NodeTypes.SubCategories.Logic,
                "any",
                new[] {new Pin(PinIds.X, PinTypes.Bvec4)}, new[] {new Pin("", PinTypes.Bool)}));
            Add(FunctionNodeFactory.Function(NodeTypes.AllBvec2, NodeTypes.AllBvec2, NodeTypes.SubCategories.Logic,
                "all",
                new[] {new Pin(PinIds.X, PinTypes.Bvec2)}, new[] {new Pin("", PinTypes.Bool)}));
            Add(FunctionNodeFactory.Function(NodeTypes.AllBvec3, NodeTypes.AllBvec3, NodeTypes.SubCategories.Logic,
                "all",
                new[] {new Pin(PinIds.X, PinTypes.Bvec3)}, new[] {new Pin("", PinTypes.Bool)}));
            Add(FunctionNodeFactory.Function(NodeTypes.AllBvec4, NodeTypes.AllBvec4, NodeTypes.SubCategories.Logic,
                "all",
                new[] {new Pin(PinIds.X, PinTypes.Bvec4)}, new[] {new Pin("", PinTypes.Bool)}));
            Add(FunctionNodeFactory.Function(NodeTypes.NotBool, NodeTypes.NotBool, NodeTypes.SubCategories.Logic, "not",
                new[] {new Pin(PinIds.X, PinTypes.Bool)}, new[] {new Pin("", PinTypes.Bool)}));
            Add(FunctionNodeFactory.Function(NodeTypes.NotBvec2, NodeTypes.NotBvec2, NodeTypes.SubCategories.Logic,
                "not",
                new[] {new Pin(PinIds.X, PinTypes.Bvec2)}, new[] {new Pin("", PinTypes.Bvec2)}));
            Add(FunctionNodeFactory.Function(NodeTypes.NotBvec3, NodeTypes.NotBvec3, NodeTypes.SubCategories.Logic,
                "not",
                new[] {new Pin(PinIds.X, PinTypes.Bvec3)}, new[] {new Pin("", PinTypes.Bvec3)}));
            Add(FunctionNodeFactory.Function(NodeTypes.NotBvec4, NodeTypes.NotBvec4, NodeTypes.SubCategories.Logic,
                "not",
                new[] {new Pin(PinIds.X, PinTypes.Bvec4)}, new[] {new Pin("", PinTypes.Bvec4)}));
            Add(FunctionNodeFactory.Function(NodeTypes.Texture2DSampler2DVec2, NodeTypes.Texture2DSampler2DVec2,
                "texture2D",
                new[] {new Pin(PinIds.Sampler, PinTypes.Sampler2D), new Pin("coord", PinTypes.Vec2)},
                PinTypes.Vec4));
            Add(FunctionNodeFactory.Function(NodeTypes.Texture2DSampler2DVec2Float,
                NodeTypes.Texture2DSampler2DVec2Float,
                "texture2D",
                new[]
                {
                    new Pin(PinIds.Sampler, PinTypes.Sampler2D), new Pin("coord", PinTypes.Vec2),
                    new Pin("bias", PinTypes.Float)
                }, PinTypes.Vec4));
            Add(FunctionNodeFactory.Function(NodeTypes.TextureCubeSamplerCubeVec3, NodeTypes.TextureCubeSamplerCubeVec3,
                "textureCube", new[] {new Pin(PinIds.Sampler, PinTypes.SamplerCube), new Pin("coord", PinTypes.Vec3)},
                PinTypes.Vec4));
            Add(FunctionNodeFactory.Function(NodeTypes.TextureCubeSamplerCubeVec3Float,
                NodeTypes.TextureCubeSamplerCubeVec3Float, "textureCube",
                new[]
                {
                    new Pin(PinIds.Sampler, PinTypes.SamplerCube), new Pin("coord", PinTypes.Vec3),
                    new Pin("bias", PinTypes.Float)
                }, PinTypes.Vec4));

            var pinTypeGroups = new[]
            {
                new[] {PinTypes.Float, PinTypes.Vec2, PinTypes.Vec3, PinTypes.Vec4},
                new[] {PinTypes.Int, PinTypes.Ivec2, PinTypes.Ivec3, PinTypes.Ivec4},
                new[] {PinTypes.Bool, PinTypes.Bvec2, PinTypes.Bvec3, PinTypes.Bvec4}
            };
            for (var groupA = 0; groupA < pinTypeGroups.Length; groupA++)
            for (var groupB = 0; groupB < pinTypeGroups.Length; groupB++)
                if (groupA != groupB)
                    for (var i = 0; i < pinTypeGroups[0].Length; ++i)
                    {
                        var typeA = pinTypeGroups[groupA][i];
                        var typeB = pinTypeGroups[groupB][i];
                        var type = NodeTypes.MakeType(typeA, typeB);
                        Add(FunctionNodeFactory.Function(type, type, "make/break", new[] {new Pin(PinIds.X, typeB)},
                            typeA));
                    }

            var prefix = "Urho3DMaterialEditor.Data.GraphFunctions.";
            foreach (var resourceName in GetType().Assembly.GetManifestResourceNames())
                if (resourceName.StartsWith(prefix))
                {
                    var script = ReadScriptResource(resourceName);
                    var factory = new InlineFunctionNodeFactory(resourceName.Substring(prefix.Length), script);
                    _inlineFunctions.Add(factory);
                    Add(factory);
                }

            _factoryByType = _nodeFactories.ToLookup(_ => _.Type);
        }

        public IReadOnlyList<InlineFunctionNodeFactory> InlineFunctions => _inlineFunctions;

        public static Script ReadScriptResource(string resourceName)
        {
            using (var stream = typeof(MaterialNodeRegistry).Assembly.GetManifestResourceStream(resourceName))
            {
                using (var reader = new StreamReader(stream, new UTF8Encoding(false)))
                {
                    return JsonConvert.DeserializeObject<Script>(reader.ReadToEnd());
                }
            }
        }

        public IEnumerable<INodeFactory> ResolveFactories(string type)
        {
            return _factoryByType[type];
        }


        public INodeFactory ResolveFactory(string type)
        {
            using (var e = ResolveFactories(type).GetEnumerator())
            {
                if (!e.MoveNext())
                    throw new Exception("Factory not found for type " + type);
                var res = e.Current;
                if (e.MoveNext())
                    throw new Exception("Multiple factories defined for type " + type);
                return res;
            }
        }

        public static class Categories
        {
            public const string Functions = "Functions";
            public const string Data = "Data";
            public const string Output = "Output";
        }

        public static class Parameters
        {
            public const string MatSpecColor = "MatSpecColor";
        }
    }
}
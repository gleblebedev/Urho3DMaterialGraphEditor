using System;
using System.Linq;
using Urho3DMaterialEditor.Model.DefineTree;

namespace Urho3DMaterialEditor.Model.Templates
{
    public partial class GLSLPassTemplate : GLSLPassTemplateBase, IT4Writer
    {
        //void WriteIfDef(NodeHelper<NodeInfo> node)
        //{
        //    if (node.Extra.Define == Always.Instance)
        //        return;
        //    var res = node.Extra.Define.Optimize();
        //        if (res != null)
        //    WriteLine("#if "+ res);
        //}

        //void WriteEndIf(NodeHelper<NodeInfo> node)
        //{
        //    if (node.Extra.Define == Always.Instance)
        //        return;
        //    var res = node.Extra.Define.Optimize();
        //    if (res != null)
        //        WriteLine("#endif");
        //}

        public string _currentIfDef;
        private readonly GLSLCodeGen PixelShaderGenerator;

        private readonly GLSLCodeGen VertexShaderGenerator;

        public GLSLPassTemplate(TranslatedMaterialGraph graph)
        {
            Graph = graph;
            VertexShaderGenerator = new GLSLCodeGen(this);
            VertexShaderUniformsAndFunctions = new UniformsAndFunctions(
                graph.Script.Nodes.Where(_ => _.Extra != null && !_.Extra.RequiredInPixelShader),
                graph.VertexShaderUniforms, VertexShaderGenerator);
            PixelShaderGenerator = new GLSLCodeGen(this);
            PixelShaderUniformsAndFunctions = new UniformsAndFunctions(
                graph.Script.Nodes.Where(_ => _.Extra != null && _.Extra.RequiredInPixelShader),
                graph.PixelShaderUniforms, PixelShaderGenerator);
        }

        public UniformsAndFunctions VertexShaderUniformsAndFunctions { get; set; }
        public UniformsAndFunctions PixelShaderUniformsAndFunctions { get; set; }

        public TranslatedMaterialGraph Graph { get; }

        public void WriteLine(string ifdef, string line)
        {
            if (ifdef == BinaryTree.FalseConst)
            {
                return;
            }
            else if (ifdef == BinaryTree.TrueConst)
            {
                ifdef = null;
            }
            else if (string.IsNullOrWhiteSpace(ifdef))
            {
                ifdef = null;
            }

            if (ifdef != _currentIfDef)
            {
                if (_currentIfDef != null)
                    WriteLine("#endif");
                _currentIfDef = ifdef;
                if (_currentIfDef != null)
                    WriteLine("#if " + _currentIfDef);
            }

            WriteLine(line);
        }
    }

    public partial class GLSLTemplate : GLSLTemplateBase
    {
        public GLSLTemplate(ShaderGeneratorContext context)
        {
            Context = context;
        }

        public ShaderGeneratorContext Context { get; }

        protected string BuildPass(TranslatedMaterialGraph pass)
        {
            return new GLSLPassTemplate(pass).TransformText();
        }
    }

    public partial class HLSLPassTemplate : HLSLPassTemplateBase, IT4Writer
    {
        public string _currentIfDef;
        private HLSLCodeGen PixelShaderGenerator;

        private HLSLCodeGen VertexShaderGenerator;

        public HLSLPassTemplate(TranslatedMaterialGraph graph)
        {
            Graph = graph;
            VertexShaderGenerator = new HLSLCodeGen(this);
            PixelShaderGenerator = new HLSLCodeGen(this);
        }

        public TranslatedMaterialGraph Graph { get; }

        public void WriteLine(string ifdef, string line)
        {
            if (ifdef == BinaryTree.FalseConst)
            {
                return;
            }
            else if (ifdef == BinaryTree.TrueConst)
            {
                ifdef = null;
            }
            else if (string.IsNullOrWhiteSpace(ifdef))
            {
                ifdef = null;
            }

            if (ifdef != _currentIfDef)
            {
                if (_currentIfDef != null)
                    WriteLine("#endif");
                _currentIfDef = ifdef;
                if (_currentIfDef != null)
                    WriteLine("#if " + _currentIfDef);
            }

            WriteLine(line);
        }
    }

    public partial class HLSLTemplate : HLSLTemplateBase
    {
        private int varCounter;

        public HLSLTemplate(ShaderGeneratorContext context)
        {
            Context = context;
        }

        public ShaderGeneratorContext Context { get; }

        protected string BuildPass(TranslatedMaterialGraph pass)
        {
            return new HLSLPassTemplate(pass).TransformText();
        }
    }


    public partial class MaterialTemplate : MaterialTemplateBase
    {
        public MaterialTemplate(ShaderGeneratorContext context)
        {
            Context = context;
        }

        public ShaderGeneratorContext Context { get; }

        public static string GetTextureUnitName(string name)
        {
            switch (name)
            {
                case SamplerNodeFactory.Diffuse:
                case SamplerNodeFactory.DiffuseCubeMap:
                    return "diffuse";
                case SamplerNodeFactory.Normal:
                    return "normal";
                case SamplerNodeFactory.Specular:
                    return "specular";
                case SamplerNodeFactory.Emissive:
                    return "emissive";
                case SamplerNodeFactory.Environment:
                case SamplerNodeFactory.EnvironmentCubeMap:
                    return "environment";
                case SamplerNodeFactory.Screen:
                case SamplerNodeFactory.LightRampMap:
                case SamplerNodeFactory.ShadowMap:
                case SamplerNodeFactory.FaceSelectCubeMap:
                case SamplerNodeFactory.IndirectionCubeMap:
                case SamplerNodeFactory.ZoneCubeMap:
                    return null;
            }

            throw new NotImplementedException(name);
        }
    }

    public partial class TechniqueTemplate : TechniqueTemplateBase
    {
        public TechniqueTemplate(ShaderGeneratorContext context)
        {
            Context = context;
        }

        public ShaderGeneratorContext Context { get; }
    }
}
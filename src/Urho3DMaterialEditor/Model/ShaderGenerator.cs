using System.IO;
using Toe.Scripting;
using Toe.Scripting.Helpers;
using Urho3DMaterialEditor.Model.Templates;

namespace Urho3DMaterialEditor.Model
{
    public class ShaderGenerator
    {
        private readonly UrhoContext _context;

        //private ShaderFilesGenerator sg;
        public ShaderGenerator(UrhoContext context)
        {
            _context = context;
            // sg = null;
        }

        public PreivewContent Generate(Script script, string name, Connection pinAndNode = null)
        {
            var glslgraph = new ScriptHelper<TranslatedMaterialGraph.NodeInfo>(script);
            glslgraph.Add(new NodeHelper<TranslatedMaterialGraph.NodeInfo> {Type = NodeTypes.Undefine, Value = "DX11"});
            glslgraph.Nodes.Add(
                new NodeHelper<TranslatedMaterialGraph.NodeInfo> {Type = NodeTypes.Define, Value = "GLSL"});
            var hlslgraph = new ScriptHelper<TranslatedMaterialGraph.NodeInfo>(script);
            hlslgraph.Add(
                new NodeHelper<TranslatedMaterialGraph.NodeInfo> {Type = NodeTypes.Undefine, Value = "GL_ES"});
            hlslgraph.Add(new NodeHelper<TranslatedMaterialGraph.NodeInfo> {Type = NodeTypes.Undefine, Value = "GL3"});
            hlslgraph.Nodes.Add(
                new NodeHelper<TranslatedMaterialGraph.NodeInfo> {Type = NodeTypes.Define, Value = "HLSL"});
            var glslcontext = new ShaderGeneratorContext(glslgraph, name, pinAndNode);
            var hlslcontext = new ShaderGeneratorContext(hlslgraph, name, pinAndNode);

            var content = new PreivewContent
            {
                GLSLShader = new GLSLTemplate(glslcontext).TransformText(),
                HLSLShader = new HLSLTemplate(hlslcontext).TransformText(),
                Technique = new TechniqueTemplate(glslcontext).TransformText(),
                Material = new MaterialTemplate(glslcontext).TransformText(),
                UrhoContext = _context,
                Name = name
            };
            return content;
        }

        private Stream CreateFile(string fileName)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            return File.Open(fileName, FileMode.Create, FileAccess.Write, FileShare.Read);
        }
    }
}
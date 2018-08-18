using System.IO;

namespace Urho3DMaterialEditor.Model
{
    //public enum Quality
    //{
    //    Low = 0,
    //    Mid = 1,
    //    High = 2
    //}

    //public class TechniqueReference
    //{
    //    public Quality? Quality { get; set; }
    //    public int? LOD { get; set; }
    //}

    public class PreivewContent
    {
        public string Name { get; set; }
        public string Material { get; set; }
        public string Technique { get; set; }
        //public TechniqueReference[] TechniqueLODs { get; set; }
        //public float[] LODs { get; set; }
        public string GLSLShader { get; set; }
        public string HLSLShader { get; set; }

        public UrhoContext UrhoContext { get; set; }

        public const string Subfolder = "Graph";

        public string GetMaterialFileName()
        {
            return Path.Combine("Materials", Subfolder, Name + ".xml").Replace(Path.DirectorySeparatorChar,'/');
        }
        public string GetTechniqueFileName()
        {
            return Path.Combine("Techniques", Subfolder, Name + ".xml").Replace(Path.DirectorySeparatorChar, '/');
        }
        public string GetGLSLFileName()
        {
            return Path.Combine("Shaders", "GLSL", Subfolder, Name + ".glsl").Replace(Path.DirectorySeparatorChar, '/');
        }
        public string GetHLSLFileName()
        {
            return Path.Combine("Shaders", "HLSL", Subfolder, Name + ".hlsl").Replace(Path.DirectorySeparatorChar, '/');
        }

        public void Save()
        {
            var materialFileName = Path.Combine(UrhoContext.DataFolder, GetMaterialFileName());
            var techniqueFileName = Path.Combine(UrhoContext.DataFolder, GetTechniqueFileName());
            var glslFileName = Path.Combine(UrhoContext.DataFolder, GetGLSLFileName());
            var hlslFileName = Path.Combine(UrhoContext.DataFolder, GetHLSLFileName());

            UrhoContext.WriteAllText(glslFileName, GLSLShader);
            UrhoContext.WriteAllText(hlslFileName, HLSLShader);
            UrhoContext.WriteAllText(techniqueFileName, Technique);
            UrhoContext.WriteAllText(materialFileName, Material);
           // MainWindow.txtShadow = GLSLShader;
        }
    }
}
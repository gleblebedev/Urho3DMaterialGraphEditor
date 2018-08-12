using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json;
using Toe.Scripting;
using Toe.Scripting.WPF.ViewModels;

namespace Urho3DMaterialEditor.Model
{
    public class SaveToZip
    {
        private readonly UrhoContext _context;
        private readonly ShaderGenerator _generator;

        public SaveToZip(UrhoContext context, ShaderGenerator generator)
        {
            _context = context;
            _generator = generator;
        }

        public void SaveTo(Stream stream, string name, Script script)
        {
            using (var acrhive = new ZipArchive(stream, ZipArchiveMode.Create, false))
            {
                SaveText(UrhoContext.MaterialGraphs + "/" + name + ".json", 
                    acrhive, 
                    JsonConvert.SerializeObject(script, Formatting.Indented,
                        new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None }));

                var res = _generator.Generate(script, name);

                SaveText(res.GetGLSLFileName(), acrhive, res.GLSLShader);
                SaveText(res.GetHLSLFileName(), acrhive, res.HLSLShader);
                SaveText(res.GetTechniqueFileName(), acrhive, res.Technique);
                SaveText(res.GetMaterialFileName(), acrhive, res.Material);

                foreach (var scriptNode in script.Nodes)
                {
                    if (scriptNode.Type == NodeTypes.SamplerCube 
                        || scriptNode.Type == NodeTypes.Sampler2D 
                        || scriptNode.Type == NodeTypes.Sampler3D)
                    {
                        SaveTexture(scriptNode, acrhive);
                    }
                }
            }
        }

        private void SaveTexture(ScriptNode scriptNode, ZipArchive acrhive)
        {
            string textureName = scriptNode.Value;
            if (string.IsNullOrWhiteSpace(textureName))
                return;
            string filePath;
            if (!_context.TryGetAbsolteFileName(scriptNode.Value, out filePath))
                return;

            using (var file = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var entry = acrhive.CreateEntry(textureName);
                using (var entryStream = entry.Open())
                {
                    file.CopyTo(entryStream);
                }
            }

            if (filePath.EndsWith(".xml", StringComparison.InvariantCultureIgnoreCase))
            {
                XDocument doc = XDocument.Load(filePath);
                var texture = doc.Element("cubemap");
                if (texture != null)
                {
                    foreach (var face in texture.Elements("face"))
                    {
                        var name = face.Attribute("name")?.Value;
                        if (!string.IsNullOrWhiteSpace(name))
                        {
                            var path = Path.Combine(Path.GetDirectoryName(filePath), name);
                            string faceFileName;
                            if (_context.TryGetRelFileName(path, out faceFileName))
                            {
                                using (var file = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                                {
                                    var entry = acrhive.CreateEntry(faceFileName);
                                    using (var entryStream = entry.Open())
                                    {
                                        file.CopyTo(entryStream);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void SaveText(string name, ZipArchive acrhive, string text)
        {
            var entry = acrhive.CreateEntry(name);
            using (var entryStream = entry.Open())
            {
                using (var writer = new StreamWriter(entryStream, new UTF8Encoding(false)))
                {
                    writer.Write(text);
                }
            }
        }
    }
}

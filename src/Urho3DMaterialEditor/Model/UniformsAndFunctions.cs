using System.Collections.Generic;
using Toe.Scripting.Helpers;
using Urho3DMaterialEditor.Model.TranslationPasses;
using static Urho3DMaterialEditor.Model.TranslatedMaterialGraph;

namespace Urho3DMaterialEditor.Model
{
    public class UniformsAndFunctions
    {
        private readonly HashSet<string> _visitedFunctions;

        public UniformsAndFunctions(IEnumerable<NodeHelper<NodeInfo>> nodes, IList<NodeHelper<NodeInfo>> uniforms,
            ICodeGen codeGen)
        {
            Uniforms = new HashSet<NodeHelper<NodeInfo>>(uniforms);
            _visitedFunctions = new HashSet<string>();
            Functions = new List<RequiredFunction>();

            foreach (var scriptNode in nodes)
            {
                foreach (var uniform in codeGen.GetRequiredUniforms(scriptNode)) Uniforms.Add(uniform);
                foreach (var requiredFunction in codeGen.GetRequiredFunctions(scriptNode))
                    AddFunction(requiredFunction, codeGen);
            }

            foreach (var shaderUniform in Uniforms)
                if (shaderUniform.Extra == null)
                    shaderUniform.Extra = new NodeInfo
                    {
                        Define = new PaintDefines.Container {IsAlways = true}
                    };
        }

        public HashSet<NodeHelper<NodeInfo>> Uniforms { get; }
        public IList<RequiredFunction> Functions { get; }

        private void AddFunction(RequiredFunction requiredFunction, ICodeGen codeGen)
        {
            if (!_visitedFunctions.Add(requiredFunction.Name))
                return;
            foreach (var uniform in codeGen.GetRequiredUniforms(requiredFunction)) Uniforms.Add(uniform);
            foreach (var depFunction in codeGen.GetRequiredFunctions(requiredFunction))
                AddFunction(depFunction, codeGen);
            Functions.Add(requiredFunction);
        }
    }
}
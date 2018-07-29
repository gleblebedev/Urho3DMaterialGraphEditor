using System.Collections.Generic;
using Toe.Scripting.Helpers;
using static Urho3DMaterialEditor.Model.TranslatedMaterialGraph;

namespace Urho3DMaterialEditor.Model
{
    public interface ICodeGen
    {
        IEnumerable<RequiredFunction> GetRequiredFunctions(RequiredFunction requiredFunction);
        IEnumerable<RequiredFunction> GetRequiredFunctions(NodeHelper<NodeInfo> node);
        IEnumerable<NodeHelper<NodeInfo>> GetRequiredUniforms(RequiredFunction requiredFunction);
        IEnumerable<NodeHelper<NodeInfo>> GetRequiredUniforms(NodeHelper<NodeInfo> node);
        string GetFunction(RequiredFunction function);
    }
}
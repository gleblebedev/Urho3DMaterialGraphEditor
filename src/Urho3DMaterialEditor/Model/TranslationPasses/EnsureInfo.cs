using Toe.Scripting;
using Toe.Scripting.Helpers;

namespace Urho3DMaterialEditor.Model.TranslationPasses
{
    public class EnsureInfo
    {
        private readonly ScriptHelper<TranslatedMaterialGraph.NodeInfo> _script;

        public EnsureInfo(ScriptHelper<TranslatedMaterialGraph.NodeInfo> script)
        {
            _script = script;
        }

        public void Apply()
        {
            foreach (var node in _script.Nodes)
                if (node.Extra == null)
                    node.Extra = new TranslatedMaterialGraph.NodeInfo();
        }
    }
}
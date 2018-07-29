using System.Linq;
using ScriptHelper = Toe.Scripting.Helpers.ScriptHelper<Urho3DMaterialEditor.Model.TranslatedMaterialGraph.NodeInfo>;
using NodeHelper = Toe.Scripting.Helpers.NodeHelper<Urho3DMaterialEditor.Model.TranslatedMaterialGraph.NodeInfo>;
using PinHelper = Toe.Scripting.Helpers.PinHelper<Urho3DMaterialEditor.Model.TranslatedMaterialGraph.NodeInfo>;

namespace Urho3DMaterialEditor.Model.TranslationPasses
{
    public class InlineFunctions
    {
        private readonly ScriptHelper _script;

        public InlineFunctions(ScriptHelper script)
        {
            _script = script;
        }

        public void Apply()
        {
            var functions = MaterialNodeRegistry.Instance.InlineFunctions.ToDictionary(_ => _.Type);

            int inlineCounter;
            do
            {
                inlineCounter = 0;
                foreach (var scriptNode in _script.Nodes.ToList())
                {
                    InlineFunctionNodeFactory inlineFactory;
                    if (functions.TryGetValue(scriptNode.Type, out inlineFactory))
                    {
                        inlineFactory.Inline(_script, scriptNode);
                        ++inlineCounter;
                    }
                }
            } while (inlineCounter > 0);
        }
    }
}
using Toe.Scripting;
using Toe.Scripting.WPF.ViewModels;

namespace Urho3DMaterialEditor.ViewModels
{
    public class SpecularViewModel : ColorViewModel
    {
        public SpecularViewModel(ScriptViewModel script, ScriptNode node) : base(script, node)
        {
        }
    }
}
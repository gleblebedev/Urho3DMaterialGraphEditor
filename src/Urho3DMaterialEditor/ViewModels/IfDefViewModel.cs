using Toe.Scripting;
using Toe.Scripting.WPF.ViewModels;
using Urho3DMaterialEditor.Model;

namespace Urho3DMaterialEditor.ViewModels
{
    public class IfDefViewModel : StringViewModel
    {
        public IfDefViewModel(ScriptViewModel script, ScriptNode node) : base(script, node)
        {
        }
    }

    public class InlineableNodeViewModel : NodeViewModel
    {
        public InlineableNodeViewModel()
        {
        }

        public InlineableNodeViewModel(ScriptViewModel script, ScriptNode node) : base(script, node)
        {
            MenuItems.Add(new MenuItemViewModel {Header = "Inline", Command = InlineThis});
        }

        private void InlineThis()
        {
            var factory = MaterialNodeRegistry.Instance.ResolveFactory(Type) as InlineFunctionNodeFactory;
            if (factory != null) factory.Inline(Script, this);
        }
    }
}
using Toe.Scripting;

namespace Urho3DMaterialEditor.Model
{
    public class DefineNodeFactory : AbstractNodeFactory
    {
        public DefineNodeFactory(string type, string name) : base(type, name,
            new[] {NodeTypes.Categories.Preprocessor})
        {
        }

        public override ScriptNode Build()
        {
            var node = new ScriptNode();
            node.Type = Type;
            node.Name = Name;
            node.Category = NodeCategory.Parameter;
            return node;
        }
    }
}
using Toe.Scripting;

namespace Urho3DMaterialEditor.Model
{
    public class MarkerNodeFactory : AbstractNodeFactory
    {
        private readonly string _defaultValue;

        public MarkerNodeFactory(string type, string name, string defaultValue) : base(type, name,
            new[] { NodeTypes.Categories.Data })
        {
            _defaultValue = defaultValue ?? "";
        }

        public override ScriptNode Build()
        {
            var node = new ScriptNode();
            node.Type = Type;
            node.Name = Name;
            node.Value = _defaultValue;
            node.Category = NodeCategory.Parameter;
            return node;
        }
    }
}
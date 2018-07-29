namespace Urho3DMaterialEditor.Model.DefineTree
{
    public class Undefined : DefineTreeItem
    {
        public override DefineTreeItemType Type => DefineTreeItemType.Undefined;

        public string Value { get; set; }

        public override string ToString()
        {
            return "!defined(" + Value + ")";
        }
    }
}
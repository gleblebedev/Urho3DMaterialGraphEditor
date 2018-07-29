namespace Urho3DMaterialEditor.Model.DefineTree
{
    public class Defined : DefineTreeItem
    {
        public override DefineTreeItemType Type => DefineTreeItemType.Defined;

        public string Value { get; set; }

        public override string ToString()
        {
            return "defined(" + Value + ")";
        }
    }
}
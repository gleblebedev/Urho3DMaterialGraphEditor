namespace Urho3DMaterialEditor.Model.DefineTree
{
    public class Always : DefineTreeItem
    {
        public static readonly Always Instance = new Always();

        private Always()
        {
        }

        public override DefineTreeItemType Type => DefineTreeItemType.Always;

        public override string ToString()
        {
            return "true";
        }
    }
}
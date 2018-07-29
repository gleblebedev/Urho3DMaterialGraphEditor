namespace Urho3DMaterialEditor.Model
{
    public class RequiredFunction
    {
        public RequiredFunction(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public override int GetHashCode()
        {
            return Name != null ? Name.GetHashCode() : 0;
        }

        public override string ToString()
        {
            return Name ?? base.ToString();
        }
    }
}
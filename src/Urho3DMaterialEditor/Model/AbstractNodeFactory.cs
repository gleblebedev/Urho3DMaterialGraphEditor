using System.Collections.Generic;
using System.Linq;
using Toe.Scripting;

namespace Urho3DMaterialEditor.Model
{
    public abstract class AbstractNodeFactory : INodeFactory
    {
        public AbstractNodeFactory(string type, string name, string category) : this(type, name, new[] {category})
        {
        }

        public AbstractNodeFactory(string type, string name, string[] category)
        {
            Type = type;
            Name = name;
            Category = category;
        }

        public string Type { get; }

        public string Name { get; }


        public string[] Category { get; }

        public virtual IEnumerable<string> InputTypes => Enumerable.Empty<string>();

        public virtual IEnumerable<string> OutputTypes => Enumerable.Empty<string>();

        public virtual bool HasEnterPins => false;

        public virtual bool HasExitPins => false;
        public abstract ScriptNode Build();

        public override string ToString()
        {
            return $"Node factory \"{string.Join(".", Category.Concat(new[] {Name}))}\" of type \"{Type}\"";
        }
    }
}
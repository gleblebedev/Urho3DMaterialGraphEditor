using System;
using System.Runtime.CompilerServices;
using Toe.Scripting.Helpers;

namespace Urho3DMaterialEditor.Model.TranslationPasses
{
    public class IfDefReference : IEquatable<IfDefReference>, IComparable<IfDefReference>
    {
        public bool Defined;
        public NodeHelper<TranslatedMaterialGraph.NodeInfo> Node;

        public int NodeHashCode => RuntimeHelpers.GetHashCode(Node);

        public int CompareTo(IfDefReference other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;

            var myHash = NodeHashCode;
            var otherHash = other.NodeHashCode;
            var compare = myHash.CompareTo(otherHash);
            if (compare != 0)
                return compare;
            return Defined.CompareTo(other.Defined);
        }

        public bool Equals(IfDefReference other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Defined == other.Defined && Equals(Node, other.Node);
        }

        public override string ToString()
        {
            return (Defined ? "" : "!") + "defined(" + Node.Value + ")";
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((IfDefReference) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Defined.GetHashCode() * 397) ^ (Node != null ? Node.GetHashCode() : 0);
            }
        }

        public static bool operator ==(IfDefReference left, IfDefReference right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(IfDefReference left, IfDefReference right)
        {
            return !Equals(left, right);
        }
    }
}
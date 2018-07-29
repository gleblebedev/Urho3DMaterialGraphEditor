using System;
using System.Collections;
using System.Collections.Generic;

namespace Urho3DMaterialEditor.Model.TranslationPasses
{
    public class IfDefSet : IEquatable<IfDefSet>, IEnumerable<IfDefReference>
    {
        private readonly int _hasCode;
        private readonly IList<IfDefReference> _references;

        public IfDefSet(IfDefReference reference) : this(new[] {reference})
        {
        }

        public IfDefSet(IList<IfDefReference> references)
        {
            _references = references;
            _hasCode = 0;
            foreach (var reference in _references) _hasCode = (_hasCode * 397) ^ reference.GetHashCode();
        }

        public int Count => _references.Count;

        public IEnumerator<IfDefReference> GetEnumerator()
        {
            return _references.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Equals(IfDefSet other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (_hasCode != other._hasCode)
                return false;
            if (_references.Count != other._references.Count)
                return false;
            for (var index = 0; index < _references.Count; index++)
                if (_references[index] != other._references[index])
                    return false;
            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((IfDefSet) obj);
        }

        public override int GetHashCode()
        {
            return _hasCode;
        }

        public static bool operator ==(IfDefSet left, IfDefSet right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(IfDefSet left, IfDefSet right)
        {
            return !Equals(left, right);
        }
    }
}
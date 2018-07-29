using System;
using System.Collections.Generic;
using System.Linq;

namespace Urho3DMaterialEditor.Model.DefineTree
{
    public class BinaryTree
    {
        public const string TrueConst = "true";
        public const string FalseConst = "false";

        private readonly List<string> _keys = new List<string>();
        private readonly BinaryTreeItem _tree;

        public BinaryTree(DefineTreeItem item)
        {
            _tree = Visit(item);
        }

        public IReadOnlyList<string> Keys => _keys;

        private uint GetMask(string name)
        {
            var index = _keys.IndexOf(name);
            if (index < 0)
            {
                index = _keys.Count;
                _keys.Add(name);
            }

            return 1u << index;
        }

        private BinaryTreeItem Visit(DefineTreeItem item)
        {
            switch (item.Type)
            {
                case DefineTreeItemType.Always:
                    return new True();
                case DefineTreeItemType.Defined:
                    return new Defined(GetMask(((DefineTree.Defined) item).Value));
                case DefineTreeItemType.Undefined:
                    return new Undefined(GetMask(((DefineTree.Undefined) item).Value));
                case DefineTreeItemType.And:
                    return new And(((DefineTree.And) item).Operands.Select(_ => Visit(_)));
                case DefineTreeItemType.Or:
                    return new Or(((DefineTree.Or) item).Operands.Select(_ => Visit(_)));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override string ToString()
        {
            if (_keys.Count == 0)
                return TrueConst;
            return ToString(0, 0);
        }

        private string ToString(uint input, int index)
        {
            if (index == _keys.Count - 1)
            {
                var ndef = _tree.Evaluate(input);
                var def = _tree.Evaluate(input | (1u << index));
                if (ndef && def)
                    return TrueConst;
                if (!ndef && !def)
                    return FalseConst;
                if (def)
                    return "defined(" + _keys[index] + ")";
                return "!defined(" + _keys[index] + ")";
            }
            else
            {
                var ndef = ToString(input, index + 1);
                var def = ToString(input | (1u << index), index + 1);

                if (ndef == def)
                    return def;

                if (ndef == TrueConst)
                    ndef = "!defined(" + _keys[index] + ")";
                else if (ndef != FalseConst)
                    ndef = "(!defined(" + _keys[index] + ") && " + ndef + ")";

                if (def == TrueConst)
                    def = "defined(" + _keys[index] + ")";
                else if (def != FalseConst)
                    def = "(defined(" + _keys[index] + ") && " + def + ")";

                if (ndef == TrueConst || def == TrueConst)
                    return TrueConst;

                if (ndef == FalseConst)
                    return def;
                if (def == FalseConst)
                    return ndef;
                return "(" + def + " || " + ndef + ")";
            }
        }

        internal abstract class BinaryTreeItem
        {
            public abstract bool Evaluate(uint input);
        }

        internal class True : BinaryTreeItem
        {
            public override bool Evaluate(uint input)
            {
                return true;
            }
        }

        internal class Defined : BinaryTreeItem
        {
            private readonly uint _mask;

            public Defined(uint mask)
            {
                _mask = mask;
            }

            public override bool Evaluate(uint input)
            {
                return (_mask & input) != 0;
            }
        }

        internal class Undefined : BinaryTreeItem
        {
            private readonly uint _mask;

            public Undefined(uint mask)
            {
                _mask = mask;
            }

            public override bool Evaluate(uint input)
            {
                return (_mask & input) == 0;
            }
        }

        internal class And : BinaryTreeItem
        {
            private readonly IList<BinaryTreeItem> _items;

            public And(IEnumerable<BinaryTreeItem> items)
            {
                _items = items.ToList();
            }

            public override bool Evaluate(uint input)
            {
                return _items.All(_ => _.Evaluate(input));
            }
        }

        internal class Or : BinaryTreeItem
        {
            private readonly IList<BinaryTreeItem> _items;

            public Or(IEnumerable<BinaryTreeItem> items)
            {
                _items = items.ToList();
            }

            public override bool Evaluate(uint input)
            {
                return _items.Any(_ => _.Evaluate(input));
            }
        }
    }
}
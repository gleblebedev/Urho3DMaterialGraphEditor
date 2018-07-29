using System;
using System.Linq;
using Urho3DMaterialEditor.Model.Templates;

namespace Urho3DMaterialEditor.Model.DefineTree
{
    public abstract class DefineTreeItem
    {
        public abstract DefineTreeItemType Type { get; }

        public static DefineTreeItem Or(DefineTreeItem a, DefineTreeItem b)
        {
            if (a == null && b == null)
                return null;
            if (a == Always.Instance || b == Always.Instance)
                return Always.Instance;
            if (a == null)
                return b;
            if (b == null)
                return a;
            if (a == b)
                return a;
            if (a.Type == DefineTreeItemType.Or && b.Type == DefineTreeItemType.Or)
                return new Or {Operands = ((Or) a).Operands.Concat(((Or) b).Operands).Distinct().ToList()};
            if (a.Type == DefineTreeItemType.Or)
                return new Or {Operands = ((Or) a).Operands.Concat(new[] {b}).Distinct().ToList()};
            if (b.Type == DefineTreeItemType.Or)
                return new Or {Operands = ((Or) b).Operands.Concat(new[] {a}).Distinct().ToList()};
            return new Or {Operands = new[] {a, b}};
        }

        public static DefineTreeItem And(DefineTreeItem a, DefineTreeItem b)
        {
            if (a == null && b == null)
                return null;
            if (a == null || a == Always.Instance)
                return b;
            if (b == null || b == Always.Instance)
                return a;
            if (a == b)
                return a;
            if (a.Type == DefineTreeItemType.And && b.Type == DefineTreeItemType.And)
                return new And {Operands = ((And) a).Operands.Concat(((And) b).Operands).Distinct().ToList()};
            if (a.Type == DefineTreeItemType.And)
                return new And {Operands = ((And) a).Operands.Concat(new[] {b}).Distinct().ToList()};
            if (b.Type == DefineTreeItemType.And)
                return new And {Operands = ((And) b).Operands.Concat(new[] {a}).Distinct().ToList()};
            return new And {Operands = new[] {a, b}};
        }

        public override string ToString()
        {
            throw new InvalidOperationException();
        }

        public string Optimize()
        {
            if (Type == DefineTreeItemType.Always)
                return null;
            var binaryTree = new BinaryTree(this);
            var res = binaryTree.ToString();
            if (res == BinaryTree.TrueConst)
                return null;
            return res;
        }

        public void WriteLineIfDef(IT4Writer writer, string str)
        {
            var ifdef = Optimize();
            writer.WriteLine(ifdef, str);
        }
    }
}
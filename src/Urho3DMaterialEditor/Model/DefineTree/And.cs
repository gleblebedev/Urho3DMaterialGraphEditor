﻿using System.Collections.Generic;
using System.Linq;

namespace Urho3DMaterialEditor.Model.DefineTree
{
    public class And : DefineTreeItem
    {
        public override DefineTreeItemType Type => DefineTreeItemType.And;

        public IList<DefineTreeItem> Operands { get; set; }

        public override string ToString()
        {
            return string.Join(" && ", Operands.Select(_ => "(" + _.ToString() + ")"));
        }
    }
}
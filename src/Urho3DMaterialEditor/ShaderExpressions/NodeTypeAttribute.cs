using System;
using System.Reflection;

namespace Urho3DMaterialEditor.ShaderExpressions
{
    public class NodeTypeAttribute : Attribute
    {
        public NodeTypeAttribute(string nodeType)
        {
            NodeType = nodeType;
        }

        public string NodeType { get; }

        private static string GetNodeType(MemberInfo element)
        {
            var a = GetCustomAttribute(element, typeof(NodeTypeAttribute));
            if (a == null)
                throw new InvalidOperationException(element + " doen't have NodeType attribute defined");
            return ((NodeTypeAttribute) a).NodeType;
        }
    }
}
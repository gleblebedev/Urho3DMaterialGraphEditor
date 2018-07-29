using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Toe.Scripting;
using Urho;
using Urho3DMaterialEditor.Model;

namespace Urho3DMaterialEditor.ShaderExpressions
{
    public class ExpressionConverter
    {
        private static readonly Dictionary<Type, string> PinTypeByType = new Dictionary<Type, string>
        {
            {typeof(bool), PinTypes.Bool},
            {typeof(float), PinTypes.Float},
            {typeof(Vector2), PinTypes.Vec2},
            {typeof(Vector3), PinTypes.Vec3},
            {typeof(Vector4), PinTypes.Vec4},
            {typeof(Sampler2D), PinTypes.Sampler2D},
            //{typeof(Sampler3D), PinTypes.Sampler3D},
            {typeof(SamplerCube), PinTypes.SamplerCube}
        };

        private readonly Script _script;

        private Dictionary<Expression, NodeAndPin> _visitedNodes = new Dictionary<Expression, NodeAndPin>();

        private ExpressionConverter(Script script)
        {
            _script = script;
        }

        public static Script Convert<R>(Expression<Func<R>> expression)
        {
            return Convert((LambdaExpression) expression);
        }

        public static Script Convert<T1, R>(Expression<Func<T1, R>> expression)
        {
            return Convert((LambdaExpression) expression);
        }

        public static Script Convert<T1, T2, R>(Expression<Func<T1, T2, R>> expression)
        {
            return Convert((LambdaExpression) expression);
        }

        public static Script Convert(LambdaExpression lambda)
        {
            var script = new Script();
            var converter = new ExpressionConverter(script);
            converter.Visit(lambda);
            return script;
        }

        private void Visit(LambdaExpression lambda)
        {
            var returnType = GetPinType(lambda.ReturnType);
            foreach (var parameterExpression in lambda.Parameters)
            {
                var e = GetPinType(parameterExpression.Type);
            }

            var result = Visit(lambda.Body);
        }

        private NodeAndPin Visit(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Add:
                    return VisitAdd((BinaryExpression) expression);
                default:
                    throw new NotImplementedException(expression.NodeType + " is not implemented yet");
            }
        }

        private BinaryOp VisitBinaryExpression(BinaryExpression expression)
        {
            return new BinaryOp {Left = Visit(expression.Left), Right = Visit(expression.Right)};
        }

        private NodeAndPin VisitAdd(BinaryExpression expression)
        {
            var args = VisitBinaryExpression(expression);
            if (args.Left.Pin.Type != args.Right.Pin.Type)
                throw new NotImplementedException("" + args.Left.Pin.Type + "+" + args.Right.Pin.Type +
                                                  " is not implemented");
            string nodeType = null;
            switch (args.Left.Pin.Type)
            {
                case PinTypes.Float:
                    nodeType = NodeTypes.AddFloatFloat;
                    break;
                case PinTypes.Vec2:
                    nodeType = NodeTypes.AddVec2Vec2;
                    break;
                case PinTypes.Vec3:
                    nodeType = NodeTypes.AddVec3Vec3;
                    break;
                case PinTypes.Vec4:
                    nodeType = NodeTypes.AddVec4Vec4;
                    break;
                default:
                    throw new NotImplementedException("" + args.Left.Pin.Type + "+" + args.Right.Pin.Type +
                                                      " is not implemented");
            }

            var node = MaterialNodeRegistry.Instance.ResolveFactory(nodeType).Build();
            _script.Add(node);
            node.InputPins[0].Connection = new Connection(args.Left);
            node.InputPins[1].Connection = new Connection(args.Right);
            return new NodeAndPin(node, node.OutputPins[0]);
        }

        private string GetPinType(Type parameterExpressionType)
        {
            string type;
            if (PinTypeByType.TryGetValue(parameterExpressionType, out type)) return type;

            throw new KeyNotFoundException(parameterExpressionType + " is not supported in a shader");
        }

        private struct BinaryOp
        {
            public NodeAndPin Left;
            public NodeAndPin Right;
        }
    }
}
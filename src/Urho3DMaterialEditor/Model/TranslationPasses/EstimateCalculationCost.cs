using System.Linq;
using Toe.Scripting.Helpers;
using ScriptHelper = Toe.Scripting.Helpers.ScriptHelper<Urho3DMaterialEditor.Model.TranslatedMaterialGraph.NodeInfo>;
using NodeHelper = Toe.Scripting.Helpers.NodeHelper<Urho3DMaterialEditor.Model.TranslatedMaterialGraph.NodeInfo>;
using PinHelper = Toe.Scripting.Helpers.PinHelper<Urho3DMaterialEditor.Model.TranslatedMaterialGraph.NodeInfo>;

namespace Urho3DMaterialEditor.Model.TranslationPasses
{
    public class EstimateCalculationCost
    {
        private readonly ScriptHelper<TranslatedMaterialGraph.NodeInfo> _script;

        public EstimateCalculationCost(ScriptHelper<TranslatedMaterialGraph.NodeInfo> script)
        {
            _script = script;
        }

        public void Apply()
        {
            foreach (var node in _script.Nodes)
            {
                EvaluateCost(node);
            }
        }

        double EvaluateCost(NodeHelper node)
        {
            if (node.Extra.EstimatedCost.HasValue)
            {
                return node.Extra.EstimatedCost.Value;
            }

            node.Extra.EstimatedCost = float.MaxValue;
            if (NodeTypes.IsIfDefType(node.Type))
            {
                var max = node.InputPins.ConnectedPins.Select(_ => _.Node).Select(_ => EvaluateCost(_)).Max();
                node.Extra.EstimatedCost = max;
                return max;
            }
            var sum = node.InputPins.ConnectedPins.Select(_ => _.Node).Select(_ => EvaluateCost(_)).Sum();
            node.Extra.EstimatedCost = sum;
            if (NodeTypes.IsConstant(node.Type) ||
                (node.Type == NodeTypes.Special.Default) ||
                NodeTypes.IsConnectorType(node.Type) || 
                NodeTypes.IsParameter(node.Type) || 
                NodeTypes.IsSampler(node.Type) || 
                NodeTypes.IsUniform(node.Type))
                return sum;
            if (NodeTypes.IsAttribute(node.Type))
            {
                node.Extra.EstimatedCost = float.MaxValue;
                return float.MaxValue;
            }

            switch (node.Type)
            {
                case NodeTypes.PerPixelFloat://perPixelFloat"; //
                case NodeTypes.PerPixelVec2://perPixelVec2"; //
                case NodeTypes.PerPixelVec3://perPixelVec3"; //
                case NodeTypes.PerPixelVec4://perPixelVec4"; //
                    sum = float.MaxValue;
                    break;
                case NodeTypes.BreakVec2://breakVec2"; //
                case NodeTypes.BreakVec3://breakVec3"; //
                case NodeTypes.BreakVec4://breakVec4"; //
                case NodeTypes.BreakVec4ToVec3AndFloat://breakVec4toVec3Float"; //
                case NodeTypes.BreakVec3ToVec2AndFloat://breakVec3toVec2Float"; //
                case NodeTypes.BreakVec4ToVec2AndVec2://breakVec4toVec2Vec2"; //
                case NodeTypes.BreakVec4ToVec2AndFloats://breakVec4toVec2FloatFloat"; //
                case NodeTypes.MakeVec2://makeVec2"; //
                case NodeTypes.MakeVec3://makeVec3"; //
                case NodeTypes.MakeVec4://makeVec4"; //
                case NodeTypes.MakeMat2://makeMat2"; //
                case NodeTypes.MakeVec4FromVec3://makeVec4fromVec3"; //
                case NodeTypes.MakeVec4FromVec3AndFloat://makeVec4fromVec3Float"; //
                case NodeTypes.MakeVec3FromVec2AndFloat://makeVec3fromVec2Float"; //
                case NodeTypes.MakeVec4FromVec2AndVec2://makeVec4fromVec2Vec2"; //
                case NodeTypes.MakeMat3FromMat4://mat3(mat4)"; //
                case NodeTypes.MakeMat3FromVec3Vec3Vec3://mat3(vec3,vec3,vec3)"; //
                case NodeTypes.MakeMat3FromMat4x3://mat3(mat4x3)"; //
                case NodeTypes.MakeMat4x3FromVec4Vec4Vec4://mat4x3(vec4,vec4,vec4)"; //
                    break;
                default:
                    sum += 1.0;
                    break;
            }
            node.Extra.EstimatedCost = sum;
            return sum;
        }
    }
}
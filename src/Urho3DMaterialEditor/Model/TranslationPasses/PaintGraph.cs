using System;
using System.Collections.Generic;
using System.Linq;
using Toe.Scripting.Helpers;
using Urho;
using ScriptHelper = Toe.Scripting.Helpers.ScriptHelper<Urho3DMaterialEditor.Model.TranslatedMaterialGraph.NodeInfo>;
using NodeHelper = Toe.Scripting.Helpers.NodeHelper<Urho3DMaterialEditor.Model.TranslatedMaterialGraph.NodeInfo>;
using PinHelper = Toe.Scripting.Helpers.PinHelper<Urho3DMaterialEditor.Model.TranslatedMaterialGraph.NodeInfo>;

namespace Urho3DMaterialEditor.Model.TranslationPasses
{
    public class PaintGraph
    {
        private readonly ScriptHelper _script;

        public PaintGraph(ScriptHelper script)
        {
            _script = script;
        }

        public void Apply()
        {
            foreach (var scriptNode in _script.Nodes)
            {
                if (IsPixelShaderOnly(scriptNode))
                    PaintRightSide(scriptNode, _=>!_.Extra.RequiredInPixelShader, _=> _.Extra.RequiredInPixelShader = true);
                if (IsPixelShaderOutput(scriptNode))
                    PaintLefttSide(scriptNode, _ => !_.Extra.UsedInPixelShader, _ => _.Extra.UsedInPixelShader = true);
                else if (IsVertexShaderOutput(scriptNode))
                    PaintLefttSide(scriptNode, _ => !_.Extra.UsedInVertexShader, _ => _.Extra.UsedInVertexShader = true);
            }
        }

        private void PaintRightSide(NodeHelper scriptNode, Func<NodeHelper,bool> shouldContinue, Action<NodeHelper> mark)
        {
            Queue<NodeHelper> nodesToVisit = new Queue<NodeHelper>();
            nodesToVisit.Enqueue(scriptNode);

            while (nodesToVisit.Count > 0)
            {
                var n = nodesToVisit.Dequeue();
                if (shouldContinue(n))
                {
                    mark(n);
                    foreach (var node in n.OutputPins.ConnectedPins)
                    {
                        nodesToVisit.Enqueue(node.Node);
                    }
                }
            }
        }
        private void PaintLefttSide(NodeHelper scriptNode, Func<NodeHelper, bool> shouldContinue, Action<NodeHelper> mark)
        {
            Queue<NodeHelper> nodesToVisit = new Queue<NodeHelper>();
            nodesToVisit.Enqueue(scriptNode);

            while (nodesToVisit.Count > 0)
            {
                var n = nodesToVisit.Dequeue();
                if (shouldContinue(n))
                {
                    mark(n);
                    foreach (var node in n.InputPins.ConnectedPins)
                    {
                        nodesToVisit.Enqueue(node.Node);
                    }
                }
            }
        }
        public static bool IsVertexShaderOutput(NodeHelper scriptNode)
        {
            switch (scriptNode.Type)
            {
                case NodeTypes.PositionOutput:
                    return true;
            }
            return false;
        }

        public static bool IsPixelShaderOutput(NodeHelper scriptNode)
        {
            switch (scriptNode.Type)
            {
                case NodeTypes.Discard:
                case NodeTypes.Special.FinalColor:
                case NodeTypes.Special.FragData0:
                case NodeTypes.Special.FragData1:
                case NodeTypes.Special.FragData2:
                case NodeTypes.Special.FragData3:
                    return true;
            }
            return false;
        }

        public static bool IsPixelShaderOnly(NodeHelper scriptNode)
        {
            if (NodeTypes.IsUniform(scriptNode.Type))
            {
                if (scriptNode.Name.EndsWith("PS"))
                    return true;
            }
            switch (scriptNode.Type)
            {
                case NodeTypes.Sampler2D:
                case NodeTypes.SamplerCube:
                case NodeTypes.AmbientColor:
                case NodeTypes.LightColor:
                case NodeTypes.DeferredOutput:
                case NodeTypes.Discard:
                case NodeTypes.PerPixelFloat:
                case NodeTypes.PerPixelVec2:
                case NodeTypes.PerPixelVec3:
                case NodeTypes.PerPixelVec4:
                case NodeTypes.Special.FinalColor:
                case NodeTypes.Special.FragData0:
                case NodeTypes.Special.FragData1:
                case NodeTypes.Special.FragData2:
                case NodeTypes.Special.FragData3:
                case NodeTypes.Special.ShadowMapOutput:
                case NodeTypes.FragCoord:
                case NodeTypes.FrontFacing:
                    return true;
            }

            return false;
        }
    }
}
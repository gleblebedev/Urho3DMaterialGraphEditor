using Autofac;
using Toe.Scripting;
using Toe.Scripting.WPF.ViewModels;
using Urho3DMaterialEditor.Model;

namespace Urho3DMaterialEditor.ViewModels
{
    public class ViewModelFactory : INodeViewModelFactory
    {
        private readonly DependencyContainer _container;

        public ViewModelFactory(DependencyContainer container)
        {
            _container = container;
        }

        public NodeViewModel Create(ScriptViewModel script, ScriptNode node)
        {
            if (node.Type == NodeTypes.MakeType(NodeTypes.ParameterPrefix, PinTypes.Special.Color))
                if (node.Name == MaterialNodeRegistry.Parameters.MatSpecColor)
                    return new SpecularViewModel(script, node);
            switch (node.Type)
            {
                case NodeTypes.SamplerCube:
                case NodeTypes.Sampler2D:
                    switch (node.Name)
                    {
                        case SamplerNodeFactory.Screen:
                        case SamplerNodeFactory.LightRampMap:
                        case SamplerNodeFactory.ShadowMap:
                        case SamplerNodeFactory.FaceSelectCubeMap:
                        case SamplerNodeFactory.IndirectionCubeMap:
                        case SamplerNodeFactory.ZoneCubeMap:
                            return new NodeViewModel(script, node);
                    }

                    break;
            }

            NodeViewModel vm;
            if (!_container.TryResolveNamed(node.Type, out vm, TypedParameter.From(script), TypedParameter.From(node)))
                if (NodeTypes.IsConnectorType(node.Type))
                    vm = new ConnectorNodeViewModel(script, node);
                else if (NodeTypes._attributes.ContainsKey(node.Type))
                    vm = new NodeViewModel(script, node) {CanRename = true};
                else if (NodeTypes._uniforms.ContainsKey(node.Type))
                    vm = new NodeViewModel(script, node) {CanRename = true};
                else if (NodeTypes.IsIfDefType(node.Type))
                    vm = new IfDefViewModel(script, node);
                else
                    vm = new NodeViewModel(script, node);
            return vm;
        }
    }
}
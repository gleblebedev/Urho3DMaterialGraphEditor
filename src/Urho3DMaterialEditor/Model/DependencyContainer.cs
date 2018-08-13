using Autofac;
using Autofac.Core;
using Toe.Scripting;
using Toe.Scripting.WPF.ViewModels;
using Urho3DMaterialEditor.ViewModels;

namespace Urho3DMaterialEditor.Model
{
    public class DependencyContainer
    {
        private readonly IContainer _container;

        public DependencyContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(this).AsSelf().SingleInstance();

            #region ParametersAndValues

            builder.RegisterType<SamplerViewModel>()
                .Named<NodeViewModel>(NodeTypes.Sampler2D)
                .ExternallyOwned()
                .InstancePerDependency();

            builder.RegisterType<SamplerCubeViewModel>()
                .Named<NodeViewModel>(NodeTypes.SamplerCube)
                .ExternallyOwned()
                .InstancePerDependency();

            builder.RegisterType<ColorViewModel>()
                .Named<NodeViewModel>(NodeTypes.MakeType(NodeTypes.ParameterPrefix, PinTypes.Special.Color))
                .Named<NodeViewModel>(PinTypes.Special.Color)
                .ExternallyOwned()
                .InstancePerDependency();

            builder.RegisterType<ScalarViewModel>()
                .Named<NodeViewModel>(NodeTypes.MakeType(NodeTypes.ParameterPrefix, PinTypes.Float))
                .Named<NodeViewModel>(PinTypes.Float)
                .ExternallyOwned()
                .InstancePerDependency();

            builder.RegisterType<IntViewModel>()
                .Named<NodeViewModel>(NodeTypes.MakeType(NodeTypes.ParameterPrefix, PinTypes.Int))
                .Named<NodeViewModel>(PinTypes.Int)
                .ExternallyOwned()
                .InstancePerDependency();

            builder.RegisterType<Vector2ViewModel>()
                .Named<NodeViewModel>(NodeTypes.MakeType(NodeTypes.ParameterPrefix, PinTypes.Vec2))
                .Named<NodeViewModel>(PinTypes.Vec2)
                .ExternallyOwned()
                .InstancePerDependency();

            builder.RegisterType<Vector3ViewModel>()
                .Named<NodeViewModel>(NodeTypes.MakeType(NodeTypes.ParameterPrefix, PinTypes.Vec3))
                .Named<NodeViewModel>(PinTypes.Vec3)
                .ExternallyOwned()
                .InstancePerDependency();

            builder.RegisterType<Vector4ViewModel>()
                .Named<NodeViewModel>(NodeTypes.MakeType(NodeTypes.ParameterPrefix, PinTypes.Vec4))
                .Named<NodeViewModel>(PinTypes.Vec4)
                .ExternallyOwned()
                .InstancePerDependency();

            builder.RegisterType<Mat4ViewModel>()
                .Named<NodeViewModel>(PinTypes.Mat4)
                .ExternallyOwned()
                .InstancePerDependency();

            builder.RegisterType<EnumViewModel<Urho.CullMode>>()
                .Named<NodeViewModel>(NodeTypes.Cull)
                .Named<NodeViewModel>(NodeTypes.ShadowCull)
                .ExternallyOwned()
                .InstancePerDependency();

            builder.RegisterType<EnumViewModel<Urho.FillMode>>()
                .Named<NodeViewModel>(NodeTypes.Fill)
                .ExternallyOwned()
                .InstancePerDependency();

            builder.RegisterType<StringViewModel>()
                .Named<NodeViewModel>(NodeTypes.Define)
                .Named<NodeViewModel>(NodeTypes.Undefine)
                .Named<NodeViewModel>(NodeTypes.Special.FinalColor)
                .Named<NodeViewModel>(NodeTypes.Special.FragData0)
                .Named<NodeViewModel>(NodeTypes.Special.FragData1)
                .Named<NodeViewModel>(NodeTypes.Special.FragData2)
                .Named<NodeViewModel>(NodeTypes.Special.FragData3)
                .Named<NodeViewModel>(NodeTypes.Special.ShadowMapOutput)
                .ExternallyOwned()
                .InstancePerDependency();

            {
                var inlinableFunc = builder.RegisterType<InlineableNodeViewModel>();
                foreach (var func in MaterialNodeRegistry.Instance.InlineFunctions)
                    inlinableFunc.Named<NodeViewModel>(func.Type);

                inlinableFunc.ExternallyOwned().InstancePerDependency();
            }

            #endregion

            builder.RegisterType<GraphValidator>().AsSelf().SingleInstance();
            builder.RegisterGeneric(typeof(ConfigurationRepository<>)).AsSelf().SingleInstance();

            builder.RegisterType<ViewModelFactory>().As<INodeViewModelFactory>().SingleInstance();
            builder.RegisterType<MaterialNodeRegistry>().AsSelf().As<INodeRegistry>().SingleInstance();


            builder.RegisterType<UrhoContext>().AsSelf().SingleInstance();
            builder.RegisterType<MainViewModel>().AsSelf().SingleInstance();
            builder.RegisterType<ShaderGenerator>().AsSelf().SingleInstance();
            builder.RegisterType<ScriptViewModel>().AsSelf().SingleInstance();

            _container = builder.Build();
        }

        public T Resolve<T>()
        {
            return _container.Resolve<T>();
        }

        public T ResolveNamed<T>(string name, params Parameter[] parameters)
        {
            return _container.ResolveNamed<T>(name, parameters);
        }

        public bool TryResolveNamed<T>(string name, out T service, params Parameter[] parameters)
        {
            object res;
            if (!_container.TryResolveService(new KeyedService(name, typeof(T)), parameters,
                out res))
            {
                service = default(T);
                return false;
            }

            service = (T) res;
            return true;
        }
    }
}
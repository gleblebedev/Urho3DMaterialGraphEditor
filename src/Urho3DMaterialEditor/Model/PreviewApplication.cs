using System;
using System.Collections.Generic;
using System.Linq;
using Urho;

namespace Urho3DMaterialEditor.Model
{
    public class PreviewApplication : Application, IPreview
    {
        private bool _animatedModel;
        private Animation _animation;

        private LightType _lightType = LightType.Spot;
        private ShadowQuality _shadowQuality = ShadowQuality.PcfN24Bit;
        private Urho.Model _model;
        private Scene _scene;
        private bool isRotate = true;

        private readonly float mouseSensitivity = .1f;
        private float _cameraDistance = 10;

        public PreviewApplication(ApplicationOptions options) : base(options)
        {
        }

        public PreviewApplication(IntPtr handle) : base(handle)
        {
        }

        protected PreviewApplication(UrhoObjectFlag emptyFlag) : base(emptyFlag)
        {
        }

        protected float Yaw { get; set; }
        protected float Pitch { get; set; }

        public float CameraDistance
        {
            get => _cameraDistance;
            set
            {
                if (_cameraDistance != value)
                {
                    _cameraDistance = value;
                    NotifyCameraDistance();
                }
            }
        }

        public MonoDebugHud MonoDebugHud { get; private set; }

        public Node CameraNode { get; set; }
        public Node PreviewNode { get; set; }
        public Light Light { get; private set; }

        public Vector3 CameraTarget { get; set; }

        public void UpdatePreivew(PreivewContent content)
        {
            if (content == null)
                return;
            InvokeOnMain(() =>
            {
                try
                {
                    content.Save();
                    ResourceCache.ReloadResourceWithDependencies("Shaders/GLSL/"+ PreivewContent.Subfolder +"/_temp.glsl");
                    ResourceCache.ReloadResourceWithDependencies("Techniques/" + PreivewContent.Subfolder + "/_temp.xml");
                    ResourceCache.ReloadResourceWithDependencies("Materials/" + PreivewContent.Subfolder + "/_temp.xml");
                    if (PreviewNode == null)
                    {
                        CreateSceneImpl();
                        SetupViewport();
                    }
                }
                catch (Exception ex)
                {
                    OnError?.Invoke(this, new OnErrorEventArgs(ex));
                }
            });
        }

        protected override void Start()
        {
            MonoDebugHud = new MonoDebugHud(this);
            MonoDebugHud.Show();
            Input.MouseWheel += Input_MouseWheel;

            _lightType = LightType.Spot;
            _model = ResourceCache.GetModel("Models/MaterialGraphEditor/MaterialPreview.mdl", false);
            _animatedModel = false;
        }

        protected override void Stop()
        {
            Input.MouseWheel -= Input_MouseWheel;
            base.Stop();
        }

        public void CreateScene(LightType lightType)
        {
            _lightType = lightType;
            InvokeOnMain(() =>
            {
                CreateSceneImpl();
                SetupViewport();
            });
        }

        private void CreateSceneImpl()
        {
            var cache = ResourceCache;
            _scene = new Scene();
            // scene.CreateComponent<Octree>();  scene.CreateComponent<Camera>();

            _scene.CreateComponent<Octree>();
            _scene.CreateComponent<DebugRenderer>();
            var zone = _scene.CreateComponent<Zone>();
            zone.SetBoundingBox(new BoundingBox(new Vector3(-1000, -1000, -1000), new Vector3(1000, 1000, 1000)));
            zone.ZoneTexture = ResourceCache.GetTextureCube("Textures/MaterialGraphEditor/Skybox.xml", false);

            CameraNode = _scene.CreateChild("MainCamera");
            var camera = CameraNode.CreateComponent<Camera>();
            CameraNode.Position = new Vector3(0, -0.0860865f, -1.32894f);
            CameraNode.Rotation = new Quaternion(0.0436198f, 0f, 0f, 0.999048f);

            var lightNode = _scene.CreateChild("Light");
            lightNode.Position = new Vector3(-0.909645f, 2f, -2f);
            lightNode.Rotation = new Quaternion(0.416198f, 0.157379f, -0.0733869f, 0.892539f);
            Light = lightNode.CreateComponent<Light>();
            Light.Color = Color.White;
            Light.Fov = 40.0f;
            Light.LightType = _lightType;
            Light.CastShadows = true;

            var offset = new Vector3(2, 3, 7); //An offset to be sure that shader applies correct model matrix
            var bb = new BoundingBox(-Vector3.One, Vector3.One);

            PreviewNode = _scene.CreateChild("Preview");

            StaticModel modelComponent;
            if (_animatedModel)
            {
                var animatedModel = PreviewNode.CreateComponent<AnimatedModel>();
                animatedModel.Model = _model;
                modelComponent = animatedModel;
                if (_animation != null)
                {
                    var state = animatedModel.AddAnimationState(_animation);
                    state.Weight = 1.0f;
                    state.Looped = true;
                }
            }
            else
            {
                modelComponent = PreviewNode.CreateComponent<StaticModel>();
                modelComponent.Model = _model;
            }

            if (_model != null)
            {
                modelComponent.SetMaterial(ResourceCache.GetMaterial("Materials/" + PreivewContent.Subfolder + "/_temp.xml", false));
                bb = _model.BoundingBox;
            }

            var bottom = (new Vector3(bb.Min.X, bb.Min.Y, bb.Min.Z) + new Vector3(bb.Max.X, bb.Min.Y, bb.Max.Z)) * 0.5f;

            PreviewNode.Position = offset - bottom;
            CameraTarget = bb.Center + PreviewNode.Position;
            var size = bb.Size.Length;
            CameraDistance = size * 1.5f;
            CameraNode.Position = CameraTarget + new Vector3(0, 0.7071f, 0.7071f) * CameraDistance;
            CameraNode.LookAt(CameraTarget, Vector3.Up);
            lightNode.Position = CameraTarget + new Vector3(-0.211157f, 0.73685f, 0.6422342f) * (2.0f * size);
            lightNode.LookAt(CameraTarget, Vector3.Up);
            Yaw = CameraNode.Rotation.YawAngle;
            Pitch = CameraNode.Rotation.PitchAngle;

            Light.ShadowCascade = new CascadeParameters(size * 2.0f, size * 4.0f, size * 8.0f, size * 16.0f,
                Light.ShadowCascade.FadeStart, Light.ShadowCascade.BiasAutoAdjust);
            Light.Range = size * 3.0f;

            modelComponent.CastShadows = true;

            var floorNode = _scene.CreateChild("Preview");
            var scale = bb.Size.Length;
            floorNode.Position = offset + new Vector3(0, -scale * 0.5f, 0f);
            floorNode.Scale = new Vector3(scale, scale, scale);
            var floor = floorNode.CreateComponent<StaticModel>();
            floor.Model = ResourceCache.GetModel("Models/MaterialGraphEditor/Box.mdl", false);
            if (floor.Model != null)
                floor.SetMaterial(ResourceCache.GetMaterial("Materials/MaterialGraphEditor/DefaultGray.xml", false));

            floor.CastShadows = true;

            var skyboxNode = _scene.CreateChild("Skybox");
            var skybox = skyboxNode.CreateComponent<Skybox>();
            skybox.Model = ResourceCache.GetModel("Models/MaterialGraphEditor/Box.mdl", false);
            skybox.SetMaterial(ResourceCache.GetMaterial("Materials/MaterialGraphEditor/Skybox.xml", false));

            //_scene.LoadXml(FileSystem.ProgramDir + "Data/Scenes/Scene.xml");

            //CameraNode = _scene.GetChild("MainCamera", true);
            //PreviewNode = _scene.GetChild("Preview", true);
            //NotifyNodes();
            UpdateCameraPos();
        }

        public event EventHandler<NodeListUpdatedArgs> NodeListUpdated;
        public event EventHandler<EventArgs> CameraDistanceUpdated;

        private void NotifyNodes()
        {
            NodeListUpdated?.Invoke(this, new NodeListUpdatedArgs(_scene));
        }
        private void NotifyCameraDistance()
        {
            CameraDistanceUpdated?.Invoke(this, EventArgs.Empty);
        }
        public void LoadScene(string scene)
        {
            _scene = new Scene();
            _scene.LoadXml(scene);
            var camera = _scene.GetComponent<Camera>(true);
            if (camera != null)
            {
                CameraNode = camera.Node;
            }
            else
            {
                CameraNode = new Node();
                CameraNode.CreateComponent<Camera>();
                _scene.AddChild(CameraNode);
            }

            Light = null;
            var model = _scene.GetComponent<StaticModel>(true);
            if (model != null) PreviewNode = model.Node;

            NotifyNodes();
        }

        private void LoadMDL(string mdl)
        {
            InvokeOnMain(() =>
            {
                _model = ResourceCache.GetModel(mdl, false);
                _animatedModel = false;
                _animation = null;
                CreateSceneImpl();
                SetupViewport();
            });
        }

        private void LoadAnimatedMDL(string mdl, string animation)
        {
            InvokeOnMain(() =>
            {
                _model = ResourceCache.GetModel(mdl, false);
                _animatedModel = true;
                if (string.IsNullOrWhiteSpace(animation))
                    _animation = null;
                else
                    _animation = ResourceCache.GetAnimation(animation, false);
                CreateSceneImpl();
                SetupViewport();
            });
        }

        private void SetupViewport(string renderPath = null)
        {
            var renderer = Renderer;
            renderer.ShadowQuality = _shadowQuality;
            var rp = new RenderPath();
            rp.Load(ResourceCache.GetXmlFile(renderPath ?? "RenderPaths/Forward.xml"));
            rp.Append(ResourceCache.GetXmlFile("PostProcess/Bloom.xml"));
            rp.Append(ResourceCache.GetXmlFile("PostProcess/FXAA2.xml"));
            rp.SetShaderParameter("BloomMix", new Vector2(0.9f, 0.6f));
            renderer.SetViewport(0, new Viewport(Context, _scene, CameraNode.GetComponent<Camera>(), rp));
        }

        protected override void OnUpdate(float timeStep)
        {
            if (PreviewNode == null)
                return;

            if (isRotate)
                PreviewNode.Rotate(Quaternion.FromAxisAngle(Vector3.Up, timeStep * 10.0f));
            base.OnUpdate(timeStep);

            MoveCamera2D(timeStep);
            MoveCamera3D(timeStep);

            var animModel = PreviewNode.GetComponent<AnimatedModel>();
            if (animModel != null && animModel.NumAnimationStates > 0)
            {
                var state = animModel.AnimationStates.First();
                state.AddTime(timeStep);
            }
        }

        public event EventHandler<OnErrorEventArgs> OnError;

        public void SetRenderPath(string renderpath)
        {
            InvokeOnMain(() => { SetupViewport(renderpath); });
        }

        public void SetShadowQuality(ShadowQuality shadowQuality)
        {
            InvokeOnMain(() => { Renderer.ShadowQuality = _shadowQuality = shadowQuality; });
        }

        public void OpenScene(string file)
        {
            InvokeOnMain(() =>
            {
                LoadScene(file);
                SetupViewport();
            });
        }

        public void LoadModel(string file)
        {
            InvokeOnMain(() => { LoadMDL(file); });
        }

        public void LoadAnimatedModel(string file, string animation)
        {
            InvokeOnMain(() => { LoadAnimatedMDL(file, animation); });
        }

        public void SelectNode(NodeListUpdatedArgs.NodeInfo nodeInfo)
        {
            InvokeOnMain(() =>
            {
                PreviewNode = _scene.GetNode(nodeInfo.ID);
                if (PreviewNode != null)
                {
                    var mat = ResourceCache.GetMaterial("Materials/_temp.xml");
                    var model = PreviewNode.GetComponent<StaticModel>();
                    if (model != null) model.SetMaterial(mat);
                    var animatedModel = PreviewNode.GetComponent<AnimatedModel>();
                    if (animatedModel != null) animatedModel.SetMaterial(mat);
                }
            });
        }

        protected void MoveCamera2D(float timeStep)
        {
            //if (UI.FocusElement != null) return;

            //const float moveSpeed = 1.0f;

            //if (Input.GetKeyDown(Key.W)) CameraNode.Translate(Vector3.UnitY * moveSpeed * timeStep);
            //if (Input.GetKeyDown(Key.S)) CameraNode.Translate(-Vector3.UnitY * moveSpeed * timeStep);
            //if (Input.GetKeyDown(Key.A)) CameraNode.Translate(-Vector3.UnitX * moveSpeed * timeStep);
            //if (Input.GetKeyDown(Key.D)) CameraNode.Translate(Vector3.UnitX * moveSpeed * timeStep);

            //if (Input.GetKeyDown(Key.N)) CameraNode.Translate(-Vector3.UnitZ * moveSpeed * timeStep);
            //if (Input.GetKeyDown(Key.M)) CameraNode.Translate(Vector3.UnitZ * moveSpeed * timeStep);

            //if (Input.GetKeyDown(Key.Z)) {
            //    Camera camera = CameraNode.GetComponent<Camera>();
            //    camera.Zoom = camera.Zoom * 1.01f;
            //}

            //if (Input.GetKeyDown(Key.X)) {
            //    Camera camera = CameraNode.GetComponent<Camera>();
            //    camera.Zoom = camera.Zoom * 0.99f;
            //}

            //if (Input.GetMouseButtonDown(MouseButton.Right)) {
            //    var mouseMove = Input.MouseMove;
            //    if (mouseMove.Y > 0) CameraNode.Translate(Vector3.UnitY * moveSpeed * timeStep);
            //    if (mouseMove.Y < 0) CameraNode.Translate(-Vector3.UnitY * moveSpeed * timeStep);
            //    if (mouseMove.X > 0) CameraNode.Translate(-Vector3.UnitX * moveSpeed * timeStep);
            //    if (mouseMove.X < 0) CameraNode.Translate(Vector3.UnitX * moveSpeed * timeStep);
            //}
        }

        private void Input_MouseWheel(MouseWheelEventArgs obj)
        {
            if (obj.Wheel > 0)
                CameraDistance = CameraDistance * 0.9f;
            else
                CameraDistance = CameraDistance * 1.1f;

            UpdateCameraPos();
        }

        protected void MoveCamera3D(float timeStep)
        {
            //if (UI.FocusElement != null)  return;

            if (!Input.GetMouseButtonDown(MouseButton.Left))
            {
                if (Input.GetMouseButtonDown(MouseButton.Right)) isRotate = !isRotate;
                return;
            }

            var mouseMove = Input.MouseMove;
            Yaw += mouseSensitivity * mouseMove.X;
            Pitch += mouseSensitivity * mouseMove.Y;
            Pitch = MathHelper.Clamp(Pitch, -90, 90);

            UpdateCameraPos();
        }

        private void UpdateCameraPos()
        {
            CameraNode.Rotation = new Quaternion(Pitch, Yaw, 0);
            var forward = CameraNode.WorldDirection;
            CameraNode.Position = CameraTarget - forward * CameraDistance;
            var camera = CameraNode.GetComponent<Camera>();
            if (camera != null)
            {
                camera.NearClip = CameraDistance * 0.25f;
                camera.FarClip = CameraDistance * 8.0f;
            }

        }

        public class NodeListUpdatedArgs : EventArgs
        {
            public List<NodeInfo> Nodes = new List<NodeInfo>();

            public NodeListUpdatedArgs(Scene scene)
            {
                foreach (var sceneChild in scene.Children) AddNode(sceneChild);
            }

            private void AddNode(Node node)
            {
                if (node.HasComponent(new StringHash("StaticModel")) ||
                    node.HasComponent(new StringHash("AnimatedModel"))) Nodes.Add(new NodeInfo(node));
                foreach (var child in node.Children) AddNode(child);
            }

            public class NodeInfo
            {
                public NodeInfo(Node node)
                {
                    Name = node.Name;
                    ID = node.ID;
                }

                public string Name { get; }
                public uint ID { get; }

                public override string ToString()
                {
                    return string.Format("{0}: {1}", ID, Name ?? "Node " + ID);
                }
            }
        }

        public class OnErrorEventArgs : EventArgs
        {
            public OnErrorEventArgs(Exception ex)
            {
                Exception = ex;
            }

            public Exception Exception { get; }
        }
    }
}
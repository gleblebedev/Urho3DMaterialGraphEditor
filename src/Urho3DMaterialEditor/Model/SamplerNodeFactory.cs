using System.Collections.Generic;
using Toe.Scripting;

namespace Urho3DMaterialEditor.Model
{
    public class SamplerNodeFactory : AbstractNodeFactory
    {
        public const string Diffuse = "diffuse";
        public const string DiffuseCubeMap = "diffuseCubeMap";
        public const string Normal = "normal";
        public const string Specular = "specular";
        public const string Emissive = "emissive";
        public const string Environment = "environment";
        public const string EnvironmentCubeMap = "environmentCubeMap";
        public const string Screen = "screen";
        public const string ShadowMap = "shadowMap";
        public const string LightRampMap = "lightRampMap";
        public const string FaceSelectCubeMap = "faceSelectCubeMap";
        public const string IndirectionCubeMap = "indirectionCubeMap";
        public const string ZoneCubeMap = "zoneCubeMap";
        private readonly string _outputType;

        public SamplerNodeFactory(string name, string samplerType) : base(samplerType, name, "Samplers")
        {
            switch (Type)
            {
                case NodeTypes.Sampler2D:
                    _outputType = PinTypes.Sampler2D;
                    break;
                //case NodeTypes.Sampler3D:
                //    _outputType = PinTypes.Sampler3D;
                //    break;
                case NodeTypes.SamplerCube:
                    _outputType = PinTypes.SamplerCube;
                    break;
            }
        }

        public override IEnumerable<string> OutputTypes
        {
            get { yield return _outputType; }
        }


        public override ScriptNode Build()
        {
            var node = new ScriptNode();
            node.Type = Type;
            node.Name = Name;
            node.Category = NodeCategory.Parameter;
            node.OutputPins.Add(new Pin("", _outputType));
            return node;
        }
    }
}
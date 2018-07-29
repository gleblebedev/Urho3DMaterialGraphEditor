using Toe.Scripting;

namespace Urho3DMaterialEditor.Model
{
    public class LightDataNodeFactory : FunctionNodeFactory
    {
        public const string LightColor = "LightColor";
        public const string LightPos = "LightPos";
        public const string LightDir = "LightDir";
        public const string NormalOffsetScale = "NormalOffsetScale";
        public const string ShadowCubeAdjust = "ShadowCubeAdjust";
        public const string ShadowDepthFade = "ShadowDepthFade";
        public const string ShadowIntensity = "ShadowIntensity";
        public const string ShadowMapInvSize = "ShadowMapInvSize";
        public const string ShadowSplits = "ShadowSplits";
        public const string LightRad = "LightRad";
        public const string LightLength = "LightLength";

        public LightDataNodeFactory() : base(NodeTypes.LightData, "Light", new[] {MaterialNodeRegistry.Categories.Data},
            new Pin[] { },
            new[]
            {
                new Pin(LightColor, PinTypes.Vec4),
                new Pin(LightPos, PinTypes.Vec4),
                new Pin(LightDir, PinTypes.Vec3),
                new Pin(NormalOffsetScale, PinTypes.Vec4),
                new Pin(ShadowCubeAdjust, PinTypes.Vec4),
                new Pin(ShadowDepthFade, PinTypes.Vec4),
                new Pin(ShadowIntensity, PinTypes.Vec2),
                new Pin(ShadowMapInvSize, PinTypes.Vec2),
                new Pin(ShadowSplits, PinTypes.Vec4),
                new Pin(LightRad, PinTypes.Float),
                new Pin(LightLength, PinTypes.Float)
            })
        {
        }
    }
}
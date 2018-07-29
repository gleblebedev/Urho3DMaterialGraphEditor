using Toe.Scripting;

namespace Urho3DMaterialEditor.Model
{
    public class CameraDataNodeFactory : FunctionNodeFactory
    {
        public const string CameraPos = "CameraPos";
        public const string NearClip = "NearClip";
        public const string FarClip = "FarClip";
        public const string DepthMode = "DepthMode";
        public const string FrustumSize = "FrustumSize";
        public const string GBufferOffsets = "GBufferOffsets";
        public const string View = "View";
        public const string ViewInv = "ViewInv";
        public const string ViewProj = "ViewProj";
        public const string ClipPlane = "ClipPlane";

        public CameraDataNodeFactory() : base(NodeTypes.CameraData, "Camera",
            new[] {MaterialNodeRegistry.Categories.Data},
            new Pin[] { },
            new[]
            {
                new Pin(CameraPos, PinTypes.Vec3),
                new Pin(NearClip, PinTypes.Float),
                new Pin(FarClip, PinTypes.Float),
                new Pin(DepthMode, PinTypes.Vec4),
                new Pin(FrustumSize, PinTypes.Vec3),
                new Pin(GBufferOffsets, PinTypes.Vec4),
                new Pin(View, PinTypes.Mat4x3),
                new Pin(ViewInv, PinTypes.Mat4x3),
                new Pin(ViewProj, PinTypes.Mat4),
                new Pin(ClipPlane, PinTypes.Vec4)
            })
        {
        }
    }
}
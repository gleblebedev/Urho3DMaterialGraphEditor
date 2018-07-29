using Toe.Scripting.WPF.ViewModels;
using Urho3DMaterialEditor.Model;

namespace Urho3DMaterialEditor.ViewModels
{
    public class SceneNodeViewModel : ViewModelBase
    {
        public SceneNodeViewModel(PreviewApplication.NodeListUpdatedArgs.NodeInfo nodeInfo)
        {
            Info = nodeInfo;
        }

        public PreviewApplication.NodeListUpdatedArgs.NodeInfo Info { get; }

        public override string ToString()
        {
            return Info.ToString();
        }
    }
}
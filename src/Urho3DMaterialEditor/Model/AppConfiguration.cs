using System.Collections.Generic;
using System.Linq;

namespace Urho3DMaterialEditor.Model
{
    public class AppConfiguration
    {
        private List<DataFolder> _dataFolders;

        public List<DataFolder> DataFolders
        {
            get => _dataFolders ?? (_dataFolders = new List<DataFolder>());
            set => _dataFolders = value;
        }

        public IEnumerable<DataFolder> GetActiveDataFolders()
        {
            return DataFolders.Where(_ => _.IsEnabled);
        }

        public class DataFolder
        {
            public string Path { get; set; }
            public bool IsEnabled { get; set; }

            public override string ToString()
            {
                return Path ?? "";
            }
        }
    }
}
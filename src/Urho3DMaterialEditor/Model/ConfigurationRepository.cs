using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace Urho3DMaterialEditor.Model
{
    public class ConfigurationRepository<T> where T : new()
    {
        private readonly string _filePath;
        private readonly Lazy<T> _value;

        public ConfigurationRepository()
        {
            _filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                Assembly.GetEntryAssembly().GetName().Name, typeof(T).Name + ".json");
            _value = new Lazy<T>(Load);
            Directory.CreateDirectory(Path.GetDirectoryName(_filePath));
        }

        public T Value => _value.Value;

        private T Load()
        {
            if (File.Exists(_filePath))
                try
                {
                    return JsonConvert.DeserializeObject<T>(File.ReadAllText(_filePath, new UTF8Encoding(false)),
                        new JsonSerializerSettings {MissingMemberHandling = MissingMemberHandling.Ignore});
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex);
                }

            return new T();
        }

        public void Save()
        {
            if (_value.IsValueCreated) Save(_value.Value);
        }

        private void Save(T value)
        {
            using (var file = File.Open(_filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                using (var writer = new StreamWriter(file, new UTF8Encoding(false)))
                {
                    writer.Write(JsonConvert.SerializeObject(value, Formatting.Indented,
                        new JsonSerializerSettings {MissingMemberHandling = MissingMemberHandling.Ignore}));
                }
            }
        }
    }
}
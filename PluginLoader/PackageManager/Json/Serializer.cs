using Newtonsoft.Json;

namespace PluginLoader
{
    public static class JsonSerializer
    {
        public static JsonConverter[] Converters { get; } = new JsonConverter[]
        {
            new VersionConverter()
        };
        public static T Deserialize<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data, Converters);
        }
        public static string Serialize(object data)
        {
            return JsonConvert.SerializeObject(data, Converters);
        }
    }
}

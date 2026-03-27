using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ContextMenuCustomApp.Service.Common.Json
{
    public static class JsonUtil
    {
        static JsonUtil()
        {
            DefaultOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
        }

        private static readonly JsonSerializerOptions DefaultOptions;

        public static string Serialize(object obj, bool indented = false)
        {
            return JsonSerializer.Serialize(obj, DefaultOptions);
        }

        public static T Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, DefaultOptions);
        }
    }
}

using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace ContextMenuCustomApp.Service.Common.Json
{
    public static class JsonUtil
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy(),
            }
        };

        public static string Serialize(object obj, bool indented = false)
        {
            var formatting = indented ? Formatting.Indented : Formatting.None;
            return JsonConvert.SerializeObject(obj, formatting, JsonSerializerSettings);
        }

        public static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, JsonSerializerSettings);
        }

        public static void Populate(string json, object value)
        {
            JsonConvert.PopulateObject(json, value, JsonSerializerSettings);
        }

    }

}

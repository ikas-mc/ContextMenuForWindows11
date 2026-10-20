using ContextMenuCustomApp.Common;
using ContextMenuCustomApp.Service.Menu;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ContextMenuCustomApp.Service.Common.Json
{

    [JsonSourceGenerationOptions(
        WriteIndented = true,
        PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
       )]
    [JsonSerializable(typeof(MenuItem))]
    [JsonSerializable(typeof(AppLang))]
    internal partial class SourceGenerationContext : JsonSerializerContext
    {
        //https://github.com/dotnet/runtime/issues/94135
        static SourceGenerationContext()
        {
            Default = new SourceGenerationContext(CreateJsonSerializerOptions(SourceGenerationContext.Default));
        }

        private static JsonSerializerOptions CreateJsonSerializerOptions(SourceGenerationContext defaultContext)
        {
            var options = new JsonSerializerOptions(defaultContext.GeneratedSerializerOptions!)
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            };
            return options;
        }
    }


    public static class JsonUtil
    {
        public static string Serialize(MenuItem obj, bool indented = false)
        {
            return JsonSerializer.Serialize(obj, SourceGenerationContext.Default.MenuItem);
        }

        public static string Serialize(AppLang obj, bool indented = false)
        {
            return JsonSerializer.Serialize(obj, SourceGenerationContext.Default.AppLang);
        }

        //TODO
        public static T Deserialize<T>(string json) where T : class
        {
            if (typeof(T) == typeof(AppLang))
            {
                var t = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.AppLang);
                if (t is T appLang)
                {
                    return appLang;
                }
            }
            else if (typeof(T) == typeof(MenuItem))
            {
                var t = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.MenuItem);
                if (t is T menuItem)
                {
                    return menuItem;
                }
            }

            throw new System.Exception();
        }
    }
}

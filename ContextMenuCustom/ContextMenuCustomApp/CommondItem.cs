using System;
using System.Collections.Generic;
using System.Text;
using Windows.Data.Json;
using Windows.Storage;

namespace ContextMenuCustomApp
{
    public class CommondItem : BaseModel
    {
        private string title;
        private string exe;
        private string param;

        public string Title { get => title; set => SetProperty(ref title,value); }
        public string Exe { get => exe; set => SetProperty(ref exe, value); }
        public string Param { get => param; set => SetProperty(ref param, value); }
        public StorageFile File { get; set; }

        public static CommondItem ReadFromJson(string content)
        {
            if (JsonObject.TryParse(content, out var data))
            {
                return new CommondItem
                {
                    Title = data.GetNamedString("title","no title"),
                    Exe = data.GetNamedString("exe", ""),
                    Param = data.GetNamedString("param", ""),
                };
            }
            return null;
        }


        public static string WriteToJson(CommondItem content)
        {
            var json = new JsonObject();
            json["title"] = JsonValue.CreateStringValue(content.Title);
            json["exe"] = JsonValue.CreateStringValue(content.Exe ?? string.Empty);
            json["param"] = JsonValue.CreateStringValue(content.Param??string.Empty);

            return json.Stringify();
        }

        public static (bool,string) Check(CommondItem content)
        {
            if (string.IsNullOrEmpty(content.Title))
            {
                return(false,nameof(content.Title));
            }

            if (string.IsNullOrEmpty(content.Exe))
            {
                return (false, nameof(content.Exe));
            }

            if (string.IsNullOrEmpty(content.Param))
            {
                return (false, nameof(content.Param));
            }
            return (true, string.Empty);
        }
    }
}

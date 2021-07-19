using System;
using System.Collections.Generic;
using System.Text;
using Windows.Data.Json;
using Windows.Storage;

namespace _7zContextMenu2
{
   public class CommondItem1
    {
        public string Title { get; set; }
        public string Exe { get; set; }
        public string Param { get; set; }
        public StorageFile File { get; set; }

        public static CommondItem1 ReadFromJson(string content) {
            if (JsonObject.TryParse(content,out var data)){
                return new CommondItem1
                {
                    Title = data.GetNamedString("title"),
                    Exe = data.GetNamedString("exe"),
                    Param = data.GetNamedString("param"),
                };
            }
            return null;
        }


        public static string WriteToJson(CommondItem1 content)
        {
            var json=new  JsonObject();
            json["title"] =JsonValue.CreateStringValue(content.Title);
            json["exe"] = JsonValue.CreateStringValue(content.Exe);
            json["param"] = JsonValue.CreateStringValue(content.Param);

            return json.Stringify();
        }
    }
}

using Windows.Globalization;

namespace ContextMenuCustomApp.Service.Lang
{
    public class LangInfo
    {
        public string Name { get; set; }
        public string FileName { get; set; }
        public bool IsDefault { get; set; }
        public string DisplayName { get; set; }

        public static LangInfo Create(string name, string fileName, bool isDefault)
        {
            var language = new Language(name);
            var langInfo = new LangInfo()
            {
                Name = name,
                FileName = fileName,
                IsDefault = isDefault,
                DisplayName = language.DisplayName
            };
            return langInfo;
        }
    }
}
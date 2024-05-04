using Windows.Globalization;

namespace ContextMenuCustomApp.Service.Lang
{
    public class LangInfo
    {
        public string Name { get; set; }
        public string FileName { get; set; }
        public bool IsDefault { get; set; }
        public string DisplayName { get; set; }

        public static LangInfo Create(string Name, string FileName, bool IsDefault)
        {
            var language = new Language(Name);
            var langInfo = new LangInfo()
            {
                Name = Name,
                FileName = FileName,
                IsDefault = IsDefault,
                DisplayName = language.DisplayName
            };
            return langInfo;
        }
    }
}
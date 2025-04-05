namespace ContextMenuCustomApp.Service.Lang
{
    public class LangInfo
    {
        //language tag (fileName without ext)
        public string Name { get; set; }
        //fileName with ext
        public string FileName { get; set; }
        public bool IsDefault { get; set; }
        //language name
        public string DisplayName { get; set; }

        public static LangInfo Create(string name, string fileName, string displayName, bool isDefault)
        {
            var langInfo = new LangInfo()
            {
                Name = name,
                FileName = fileName,
                IsDefault = isDefault,
                DisplayName = displayName
            };
            return langInfo;
        }
    }
}
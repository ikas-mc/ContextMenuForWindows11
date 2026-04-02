using ContextMenuCustomApp.Common;
using ContextMenuCustomApp.Service.Lang;
using ContextMenuCustomApp.Service.Menu;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContextMenuCustomApp
{
    public sealed class AppContext
    {
        private static Task _task;
        public static AppLang AppLang { get; private set; }
        public static Settings AppSetting { get; } = Settings.Default;
        public static Settings AppSettings { get; } = Settings.Default;
        public static Dictionary<Type, object> Services { get; private set; } = new Dictionary<Type, object>();

        public static void Init()
        {
            _task = Task.Run(async () =>
            {
                var languageService = new LanguageService();
                Services.Add(typeof(LanguageService), languageService);
                var menusFolder = await MenuService.CreateDefualtMenusFolderAsync();
                Services.Add(typeof(MenuService), new MenuService(menusFolder));

                AppLang = await languageService.LoadAsync().ConfigureAwait(false);
            });
        }

        public static void WaitAll()
        {
            if (_task?.IsCompleted == false)
            {
                _task.Wait();
            }
            _task = null;
        }

        public static T GetService<T>()
        {
            return (T)Services[typeof(T)];
        }
    }
}
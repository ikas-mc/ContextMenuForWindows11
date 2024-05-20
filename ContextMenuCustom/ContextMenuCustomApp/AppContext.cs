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
        private Task _task;
        public AppLang AppLang { get; private set; }
        public Settings AppSetting { get; } = Settings.Default;
        public Dictionary<Type, object> Services { get; private set; } = new Dictionary<Type, object>();

        public void Init()
        {
            _task = Task.Run(async () =>
            {
                var languageService = new LanguageService();
                Services.Add(typeof(LanguageService), languageService);
                Services.Add(typeof(MenuService), new MenuService());

                AppLang = await languageService.LoadAsync().ConfigureAwait(false);
            });
        }

        public void WaitAll()
        {
            if (_task?.IsCompleted == false)
            {
                _task.Wait();
            }
            _task = null;
        }

        public T GetService<T>()
        {
            return (T)Services[typeof(T)];
        }

        #region singleton
        public static readonly AppContext Current;
        static AppContext()
        {
            Current = new AppContext();
        }
        #endregion
    }
}
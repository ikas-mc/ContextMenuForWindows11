using System;
using Windows.UI.Xaml.Controls;

namespace ContextMenuCustomApp.View.Common
{
    public class WeakEventHandler
    {
        private readonly WeakReference<Page> _weakReference;
        private readonly BaseViewModel _viewModel;

        public WeakEventHandler(BaseViewModel viewModel, Page page)
        {
            this._viewModel = viewModel;
            this._weakReference = new WeakReference<Page>(page);
            this._viewModel.Handler += OnEvent;
        }

        public void OnEvent(string message, Exception e)
        {
            if (_weakReference.TryGetTarget(out var page))
            {
                page.ShowMessage(message ?? e.Message, e == null ? MessageType.Success : MessageType.Error);
            }
            else
            {
                this._viewModel.Handler -= OnEvent;
            }
        }
    }

    public static class PageMessageExt
    {
        public static void RegisterMessageHandler(this Page page, BaseViewModel viewModel)
        {
            new WeakEventHandler(viewModel, page);
        }

        public static void ShowMessage(this Page page, string message, MessageType messageType)
        {
            if (page.Dispatcher.HasThreadAccess)
            {
                DoShowMessage(page, message, messageType);
            }
            else
            {
                _ = page.Dispatcher.TryRunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    DoShowMessage(page, message, messageType);
                });
            }
        }

        private static void DoShowMessage(Page page, string message, MessageType messageType)
        {
            if (page is IMessageSupport messageSupport)
            {
                messageSupport.UpdateMessage(true, messageType, message);
            }
            else
            {
                MessageHelper.UpdateMessage(true, messageType, message);
            }
        }
    }
}

using Microsoft.UI.Xaml.Controls;

namespace ContextMenuCustomApp.View.Common
{
    public static class MessageHelper
    {
        private static Frame? Shell()
        {
            return ContextMenuCustom.MainWindow.Instance?.AppRootFrame;
        }

        private static void UpdateStatus(bool busy)
        {
            if (Shell()?.Content is IStatusSupport statusSupport)
            {
                statusSupport.UpdateStatus(busy);
            }
        }

        public static void UpdateMessage(bool show, MessageType messageType, string message = "")
        {
            if (Shell()?.Content is IMessageSupport messageSupport)
            {
                messageSupport.UpdateMessage(show, messageType, message);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace ContextMenuCustomApp.View.Common
{
    public interface IMessageSupport
    {
        void UpdateMessage(bool show, MessageType messageType, string message = "");
    }

    public enum MessageType
    {
        Info,
        Success,
        Warnning,
        Error
    }
}

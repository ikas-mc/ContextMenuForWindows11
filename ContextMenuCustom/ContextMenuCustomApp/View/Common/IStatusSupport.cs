using System;
using System.Collections.Generic;
using System.Text;

namespace ContextMenuCustomApp.View.Common
{
    public interface IStatusSupport
    {
        void UpdateStatus(bool busy, string message = "");
    }
}

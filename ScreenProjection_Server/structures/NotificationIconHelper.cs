using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenProjection_Server
{
    class NotificationIconHelper
    {
        public static void enableNotificationIcon(NotifyIcon icon)
        {
            icon.Visible = true;
        }

        public static void disableNotificationIcon(NotifyIcon icon)
        {
            icon.Visible = false;
        }
    }
}

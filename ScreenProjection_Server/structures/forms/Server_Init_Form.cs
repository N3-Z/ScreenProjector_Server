using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenProjection_Server
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            NotificationIconHelper.enableNotificationIcon(notifyIcon);

            Connector.startOperation();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Connector.endOperation();
        }
    }
}

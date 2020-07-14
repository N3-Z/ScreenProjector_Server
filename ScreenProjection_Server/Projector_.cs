using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ScreenProjection_Server
{
    class Projector_ : ApplicationContext
    {
        private NotifyIcon trayIcon;
        private int portRequestConnect = 10063;
        private Socket socket;
        private IPEndPoint iPEndPoint;
        private Socket serverAccept;
        private Form2 form2 = null;
        private Thread processThread;

        public Projector_()
        {
            trayIcon = new NotifyIcon()
            {
                Icon = new Icon(SystemIcons.Exclamation, 40, 40),
                ContextMenu = new ContextMenu(new MenuItem[]
                {
                    new MenuItem("Exit", Exit)
                }),
                Visible = true
            };

            processThread = new Thread(listenRequestProcess);
            processThread.Start();
        }
        private void listenRequestProcess()
        {
            while (true)
            {
                listenRequest();
            }
        }

        private void listenRequest()
        {
            byte[] messageRecv;
            int byteRecv = 0;
            string recvMsg = "";
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            iPEndPoint = new IPEndPoint(IPAddress.Any, portRequestConnect);
            socket.Bind(iPEndPoint);
            socket.Listen(1);

            try
            {
                serverAccept = socket.Accept();
                messageRecv = new byte[512];
                byteRecv = serverAccept.Receive(messageRecv);
                recvMsg = Encoding.ASCII.GetString(messageRecv, 0, byteRecv);
            }
            catch (Exception)
            {
                socketClose();
                return;
            }

            string pc = recvMsg.Substring(recvMsg.Length - 2);
            string title = "Request Screen Projection";
            string message = "PC-" + pc + " want to project their screen. Allow?\n\n";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;
            result = MessageBox.Show(message, title, buttons);

            byte[] data;
            string responseRequest = "no";
            if (result == DialogResult.Yes) responseRequest = "yes"; 

            try
            {
                data = Encoding.ASCII.GetBytes(responseRequest);
                serverAccept.Send(data);
            }
            catch (Exception)
            {
                MessageBox.Show("Failed");
                responseRequest = "no";
            }

            socketClose();

            if (responseRequest.Equals("yes"))
            {
                form2 = new Form2(pc);
                form2.ShowDialog();
            }
        }

        private void socketClose()
        {
            try
            {
                socket.Close();
                serverAccept.Close();
            }
            catch (Exception)
            { }
        }


        private void Exit(object sender, EventArgs e)
        {
            socketClose();
            trayIcon.Visible = false;
            System.Environment.Exit(0);
        }
    }
}

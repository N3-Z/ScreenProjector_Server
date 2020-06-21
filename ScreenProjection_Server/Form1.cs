using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenProjection_Server
{
    public partial class Form1 : Form
    {
        private int portRequestConnect = 10063;
        private Socket socket;
        private IPEndPoint iPEndPoint;
        private Socket serverAccept;
        private Form2 form2 = null;
        private Thread listenRequestThread;
        private NotifyIcon notifyIcon;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            notifyIcon1.Visible = true;
            listenRequestThread = new Thread(listenRequestLoop);
            listenRequestThread.Start();
        }

        private void listenRequestLoop()
        {
            while (true)
            {
                listenRequest();
            }
        }

        private void listenRequest()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            iPEndPoint = new IPEndPoint(IPAddress.Any, portRequestConnect);
            socket.Bind(iPEndPoint);
            socket.Listen(10);

            serverAccept = socket.Accept();
            byte[] messageRecv = new byte[1024];
            int byteRecv = serverAccept.Receive(messageRecv);
            string recvMsg = Encoding.ASCII.GetString(messageRecv, 0, byteRecv);

            string pc = recvMsg.Substring(recvMsg.Length - 2);
            string title = "Request Screen Projection";
            string message = "PC-" + pc + " want to project their screen. Allow?\n\n";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;
            result = MessageBox.Show(message, title, buttons);

            byte[] data;
            string responseRequest;
            if (result == DialogResult.Yes) responseRequest = "yes";
            else responseRequest = "no";

            try
            {
                data = Encoding.ASCII.GetBytes(responseRequest);
                serverAccept.Send(data);
            }
            catch (Exception)
            {
                MessageBox.Show("Failed");
            }

            socket.Close();
            serverAccept.Close();

            if (responseRequest.Equals("yes"))
            {
                form2 = new Form2(pc, recvMsg);
                form2.ShowDialog();
            }
        }


        private void Button1_Click(object sender, EventArgs e)
        {
            //listenRequest();
            notifyIcon1.Visible = true;
            this.Hide();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                socket.Close();
                serverAccept.Close();
            }
            catch (Exception)
            { }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            
        }

        private void NotifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            notifyIcon1.Visible = false;

        }
    }
}

using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;

namespace ScreenProjection_Server
{
    public partial class Form2 : Form
    {
        private string pc;
        private int portRecvDesktop = 10064;

        private TcpClient client;
        private TcpListener listenener;
        private NetworkStream mainStream;

        private Thread listening_thread;
        private Thread getImage_thread;
        public Form2(string pc)
        {
            this.pc = pc;
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            client = new TcpClient();
            listenener = new TcpListener(IPAddress.Any, portRecvDesktop);
            listening_thread = new Thread(listen);
            getImage_thread = new Thread(receiveImage);
            listening_thread.Start();

            this.Text = "Screen Projection PC-" + pc;
        }
        private void listen()
        {
            while (!client.Connected)
            {
                try
                {
                    listenener.Start();
                    client = listenener.AcceptTcpClient();
                }
                catch (Exception)
                {
                }
            }
            getImage_thread.Start();
        }
        private void stoplisten()
        {
            listenener.Stop();
            client.Close();
            client = null;
            if (listening_thread.IsAlive) listening_thread.Abort();
            if (getImage_thread.IsAlive) getImage_thread.Abort();
        }
        private void receiveImage()
        {
            BinaryFormatter bin = new BinaryFormatter();
            while (client.Connected)
            {
                try
                {
                    mainStream = client.GetStream();
                    pictureBox1.Image = (Image)bin.Deserialize(mainStream);
                }
                catch (Exception)
                {
                }
            }
        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {

        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            stoplisten();
        }
    }
}

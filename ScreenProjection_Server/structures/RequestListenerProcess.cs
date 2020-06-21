using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenProjection_Server
{
    class RequestListenerProcess
    {
        private static bool listening;

        public static void requestListeningProcess()
        {
            startListening();
            while(listening)
            {
                Socket connectionSocket = acceptNewConnectionAnySrc(Settings.ConnectionRequestPort);

                string connectorIp = receiveFromSocket(connectionSocket);

                DialogResult promptResult = promptConnection(connectorIp);

                processPrompt(connectionSocket, connectorIp, promptResult);
            }
        }

        public static void startListening()
        {
            listening = true;
        }

        public static void stopListening()
        {
            listening = false;
        }

        private static Socket acceptNewConnectionAnySrc(int port)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, port);
            socket.Bind(iPEndPoint);
            socket.Listen(10);

            Socket newSocket = socket.Accept(); //stops
            socket.Close();

            return newSocket;
        }

        private static string receiveFromSocket(Socket socket)
        {
            byte[] messageRecv = new byte[1024];
            int byteRecv = socket.Receive(messageRecv); //stops
            string connectorIp = Encoding.ASCII.GetString(messageRecv, 0, byteRecv);

            return connectorIp;
        }

        private static DialogResult promptConnection(string ip)
        {
            string pcNumber = getPcNumberFromIp(ip);
            string title = "Request Screen Projection";
            string content = "PC-" + pcNumber + " want to project their screen. Allow?\n\n";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;

            return MessageBox.Show(content, title, buttons);
        }

        private static void processPrompt(Socket socket, string ip, DialogResult dialogResult)
        {
            byte[] data;
            string responseRequest;
            string pc = getPcNumberFromIp(ip);

            if (dialogResult == DialogResult.Yes) responseRequest = "yes";
            else responseRequest = "no";

            try
            {
                data = Encoding.ASCII.GetBytes(responseRequest);
                socket.Send(data);
            }
            catch (Exception)
            {
                MessageBox.Show("Failed");
            }
            socket.Close();

            if (responseRequest.Equals("yes"))
            {
                (new Form2(pc, ip)).ShowDialog();
            }
        }

        private static string getPcNumberFromIp(string text)
        {
            return text.Substring(text.Length - 2);
        }
    }
}

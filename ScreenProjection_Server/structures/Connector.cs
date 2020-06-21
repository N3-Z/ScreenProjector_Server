using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScreenProjection_Server
{
    class Connector
    {
        private static Thread requestListenerThread;

        public static void startOperation()
        {
            requestListenerThread = new Thread(RequestListenerProcess.requestListeningProcess);
            requestListenerThread.Start();
        }

        public static void endOperation()
        {
            RequestListenerProcess.stopListening();
        }
    }
}

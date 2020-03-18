using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Listener
{
    class Process
    {
        TcpListener server = null;
        public static HttpClient webClient = new HttpClient();

        public Process(string ip, int port)
        {
            try
            {

                IPAddress localAddr = IPAddress.Parse(ip);
                server = new TcpListener(IPAddress.Any, port);

                server.Start();
            }

            catch (SocketException e)
            {
                Console.WriteLine("SOCKET ERROR: " + e.Message);
            }
            finally
            {
                Console.WriteLine("INICIO DE ESCUCHA EN " + DateTime.Now);
            }


           // StartListener();
        }

    }
}


using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Listener
{
    class Program
    {


        static void Main(string[] args)
        {
            Thread t = new Thread(delegate ()
            {
                // replace the IP with your system IP Address...
                Server myserver = new Server("191.232.163.22", 5432);
            });
            t.Start();

            Console.WriteLine("Server Started...!");

            // Task.Run(() =>
            //{

            //   // replace the IP with your system IP Address...
            //    Process myserver = new Process("191.232.163.22", 5432);
            //});
            Console.WriteLine("Server Started...!");

        }
    }
}

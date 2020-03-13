
using Listener.Models.TrackerLog;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
class Server
{
    TcpListener server = null;
    public static HttpClient webClient = new HttpClient();
    public Server(string ip, int port)
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
    

        StartListener();
    }
    public void StartListener()
    {
        try
        {
            while (true)
            {
                Console.WriteLine("Waiting for a connection...");
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Connected!");
                Thread t = new Thread(new ParameterizedThreadStart(HandleDeivce));
                t.Start(client);
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
            server.Stop();
        }
    }
    public void HandleDeivce(Object obj)
    {
        TcpClient client = (TcpClient)obj;
        var stream = client.GetStream();
        string imei = String.Empty;
        string data = null;
        string strIdentifier = null;
        string coderesponse = "AP01HSO";
        Byte[] bytes = new Byte[256];
        int i;
        try
        {
            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                string hex = BitConverter.ToString(bytes);
                data = Encoding.ASCII.GetString(bytes, 0, i);
                SaveLog(data);
                Console.WriteLine(DateTime.Now.ToLongTimeString());
                Console.WriteLine("{1}: Received: {0}", data, Thread.CurrentThread.ManagedThreadId);


                if (data.Length < 40)
                {
                    //Requiere una respuesta
                    strIdentifier = data.Substring(1, 12);
                    string str = "("+ strIdentifier + coderesponse +")";
                    Byte[] reply = System.Text.Encoding.ASCII.GetBytes(str);
                    stream.Write(reply, 0, reply.Length);
                    Console.WriteLine("{1}: Sent: {0}", str, Thread.CurrentThread.ManagedThreadId);
                }

            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: {0}", e.ToString());
            client.Close();
        }
    }

    private void SaveLog(string strMessage)
    {
        CreateTrackerLog trackerLog = new CreateTrackerLog
        {
            Message = strMessage
        };
        HttpResponseMessage webResponse = webClient.PostAsJsonAsync("http://localhost/api/TrackerLogs/Create", trackerLog).Result;

    }




}
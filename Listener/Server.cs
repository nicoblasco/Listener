
using Listener.Models.Georeference;
using Listener.Models.GeoTracker;
using Listener.Models.TrackerLog;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
    public async void HandleDeivce(Object obj)
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
                ResponseTrackerLog responseTrackerLog = new ResponseTrackerLog();
                responseTrackerLog = await SaveLog(data);
                Console.WriteLine(DateTime.Now.ToLongTimeString());
                Console.WriteLine("{1}: Received: {0}", data, Thread.CurrentThread.ManagedThreadId);

                strIdentifier = data.Substring(1, 12);

                if (data.Length < 40)
                {
                    //Requiere una respuesta
                    string str = "(" + strIdentifier + coderesponse + ")";
                    Byte[] reply = System.Text.Encoding.ASCII.GetBytes(str);
                    stream.Write(reply, 0, reply.Length);
                    Console.WriteLine("{1}: Sent: {0}", str, Thread.CurrentThread.ManagedThreadId);
                }
                else
                {
                    //Saco las coodenadas
                    //Algo asi viene
                    //(072101557346BR00200305A3448.6169S05816.0160W000.0123604000.00,00000000L00000000)
                    //            string lat = "3448,6169";
                    //              string lng = "05816,0195";
                    string strLatitud = data.Substring(24, 9);
                    string strLongitud = data.Substring(34, 10);

                    //Busco el id gel GeoTracker
                    GeoTracker geoTracker = new GeoTracker();
                    //De lo contrario guardo las coordenadas
                    geoTracker = await  GetGeoTracker(strIdentifier);

                    if (geoTracker != null && responseTrackerLog!=null)
                    {
                        CreateGeoReference createGeoReference = new CreateGeoReference
                        {
                            GeoTrackerId = geoTracker.Id,
                            TrackerLogId = responseTrackerLog.Id,
                            Identifier = strIdentifier,
                            Latitude = "-"+Latitud(strLatitud).ToString(),
                            Longitude = "-" +Longitud(strLongitud).ToString()
                            

                        };

                         SaveGeoReference(createGeoReference);
                    }


                }

            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: {0}", e.ToString());
            client.Close();
        }
    }

    private async Task<ResponseTrackerLog> SaveLog(string strMessage)
    {
        CreateTrackerLog trackerLog = new CreateTrackerLog
        {
            Message = strMessage
        };
        //HttpResponseMessage webResponse = webClient.PostAsJsonAsync("http://localhost/api/TrackerLogs/Create", trackerLog).Result;

        HttpResponseMessage webResponse = webClient.PostAsJsonAsync("http://localhost/api/TrackerLogs/Create", trackerLog).Result;

        if (webResponse.IsSuccessStatusCode)
        {
            var jsonString = await  webResponse.Content.ReadAsStringAsync();

            // var deserialized = JsonConvert.DeserializeObject<IEnumerable<ResponseTrackerLog>>(jsonString);
            ResponseTrackerLog responseTrackerLog = new ResponseTrackerLog();
             return   responseTrackerLog = JsonConvert.DeserializeObject<ResponseTrackerLog>(jsonString);

      

        }
        else
            return null;
    }

    private async Task<GeoTracker> GetGeoTracker(string strParamIdentifier)
    {
    

        HttpResponseMessage webResponse = webClient.GetAsync("http://localhost/api/GeoTrackers/GetByIdentifier/"+strParamIdentifier).Result;

        if (webResponse.IsSuccessStatusCode)
        {
            var jsonString = await webResponse.Content.ReadAsStringAsync();            
            
            return JsonConvert.DeserializeObject<GeoTracker>(jsonString);



        }
        else
            return null;
    }


    private void SaveGeoReference(CreateGeoReference createGeoReference)
    {
        
        HttpResponseMessage webResponse = webClient.PostAsJsonAsync("http://localhost/api/Georeferences/Create", createGeoReference).Result;
        if (webResponse.IsSuccessStatusCode)
        {

            Console.WriteLine("Geolocalizacion OK");
        }
        else
            Console.WriteLine("Geolocalizacion Error: Exception: {0}", webResponse.StatusCode.ToString() );
    }

   private double Latitud(string g)
    {
        return (Convert.ToDouble(g.Substring(0, 2)) + (Convert.ToDouble(g.Substring(2, 7)) / 60));
    }


    private double Longitud(string g)
    {
        return (Convert.ToDouble(g.Substring(0, 3)) + (Convert.ToDouble(g.Substring(3, 7)) / 60));
    }

}
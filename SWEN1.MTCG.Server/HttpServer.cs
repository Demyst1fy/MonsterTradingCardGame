using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using SWEN1.MTCG.Game.Interfaces;
using SWEN1.MTCG.Server.Interfaces;

namespace SWEN1.MTCG.Server
{
    public class HttpServer : IHttpServer
    {
        private Thread _serverThread;
        private TcpListener _listener;
        private ConcurrentQueue<IMatch> _allBattles;
        public void Start(int port)
        {
            if (_serverThread == null)
            {
                IPAddress ipAddress = new IPAddress(0);
                _listener = new TcpListener(ipAddress, port);
                _allBattles = new ConcurrentQueue<IMatch>();
                _serverThread = new Thread(ServerHandler);
                _serverThread.Start();
            }
        }

        public void Stop()
        {
            if(_serverThread != null)
            {
                _serverThread.Interrupt();
                _serverThread = null;
            } 
        }

        private static string ReadRequest(NetworkStream stream)
        {
            MemoryStream contents = new MemoryStream();
            byte[] buffer = new byte[2048];

            do
            {
                int size = stream.Read(buffer, 0, buffer.Length);
                if (size == 0)
                    return null;

                contents.Write(buffer, 0, size);
            } while (stream.DataAvailable);

            string request = Encoding.UTF8.GetString(contents.ToArray());
            return request;
        }

        private void ServerHandler()
        {
            _listener.Start();
            
            while (true)
            {
                TcpClient client = _listener.AcceptTcpClient();
                ThreadPool.QueueUserWorkItem(HandleResponse, client);
            }
        }

        private void HandleResponse(object obj)
        {
            var client = (TcpClient)obj;

            NetworkStream stream = client.GetStream();
            string request = ReadRequest(stream);
            
            IServiceHandler serviceHandler = new ServiceHandler();

            Console.WriteLine($"{request} {Environment.NewLine}");
            IRequest parsedRequest = serviceHandler.ParseRequest(request);
            
            IResponse response = serviceHandler.HandleRequest(parsedRequest, ref _allBattles);

            StringBuilder responseText = new StringBuilder();
            responseText.Append($"Status: {response.Status} {response.Message}");
            responseText.Append($"{Environment.NewLine}");

            responseText.Append($"Mimetype: {response.MimeType}");
            responseText.Append($"{Environment.NewLine}");

            responseText.Append($"Content-Length: {response.ContentLength}");
            responseText.Append($"{Environment.NewLine}");

            responseText.Append($"Response-Body: {response.Body}");
            responseText.Append($"{Environment.NewLine}");

            byte[] data = Encoding.UTF8.GetBytes(responseText.ToString());
            stream.Write(data, 0, data.Length);
            
            stream.Close();
            client.Close();
        }
    }
}
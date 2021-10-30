using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using SWEN1.MTCG.Server.Interfaces;

namespace SWEN1.MTCG.Server
{
    public class HttpServer : IHttpServer
    {
        private Thread _serverThread;
        private TcpListener _listener;

        public void Start(int port)
        {
            if (_serverThread == null)
            {
                IPAddress ipAddress = new IPAddress(0);
                _listener = new TcpListener(ipAddress, port);
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
                {
                    return null;
                }

                contents.Write(buffer, 0, size);
            } while (stream.DataAvailable);

            string request = Encoding.UTF8.GetString(contents.ToArray());
            return request;
        }

        private void ServerHandler(object o)
        {
            _listener.Start();

            while(true)
            {
                using TcpClient client = _listener.AcceptTcpClient();
                using NetworkStream stream = client.GetStream();
                string request = ReadRequest(stream);

                IServiceHandler serviceHandler = new ServiceHandler();
                        
                IResponse response = serviceHandler.HandleRequest(request);

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
            }
        }
    }
}
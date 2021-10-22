using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ConsoleApp1;

public class HTTPServer
{
    private Thread _serverThread = null;
    private TcpListener _listener;

    public void Start(int port = 10001)
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
        var buffer = new byte[2048];
        do
        {
            int size = stream.Read(buffer, 0, buffer.Length);
            if(size == 0)
            {
                return null;
            }
            contents.Write(buffer, 0, size);
        } while (stream.DataAvailable);
        var retVal = Encoding.UTF8.GetString(contents.ToArray());
        return retVal;
    }

    private void ServerHandler(object o)
    {
        _listener.Start();
        while(true)
        {
            TcpClient client = _listener.AcceptTcpClient();
            NetworkStream stream = client.GetStream();
 
            try
            {
                var request = ReadRequest(stream);
                var response = Response.ProcessRequest(request);

                stream.Write(response.Data, 0, response.Data.Length);
            }
            finally
            {
                stream.Close();
                client.Close();
            }
        }
    }
}
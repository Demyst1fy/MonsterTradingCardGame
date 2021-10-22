using System;
using System.Text;

namespace ConsoleApp1
{
    public class Response
    {
        private string Version { get; } = "HTTP/1.1";
        private int Status { get; }
        private string Message { get; }
        private string ContentType { get; }
        private int ContentLength { get; }
        private string Body { get; }
        public byte[] Data { get; set; }

        private Response()
        {
            
        }
        public Response(int status, string message, string body)
        {
            Status = status;
            Message = message;
            ContentType = "text/plain";
            ContentLength = body.Length;
            Body = body;
        }
        
        
        public static Response ProcessRequest(string request)
        {
            Console.WriteLine($"Request:{request}");

            var response = ServiceHandler.HandleRequest(request);
            
            var responseText = $"({response.Status} {response.Message}): {response.Body}";
            var responseData = Encoding.UTF8.GetBytes(responseText);
            response.Data = responseData;
            return response;
        }
    }
}
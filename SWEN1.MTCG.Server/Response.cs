using SWEN1.MTCG.Server.Interfaces;

namespace SWEN1.MTCG.Server
{
    public class Response : IResponse
    {
        public int Status { get; }
        public string Message { get; }
        public string MimeType { get; }
        public int ContentLength { get; }
        public string Body { get; }

        public Response(int status, string body, string mimeType = "text/plain")
        {
            Status = status;
            Message = StatusMessage(status);
            
            switch (mimeType)
            {
                case "application/json": 
                    MimeType = "application/json";
                    break;
                default: 
                    MimeType = "text/plain";
                    break;
            }
            
            ContentLength = body.Length;
            Body = body;
        }
        private string StatusMessage(int status)
        {
            string message = "";
            switch (status)
            {
                case 200: message = "OK"; break;
                case 201: message = "Created"; break;
                case 204: message = "No Content"; break;
                case 400: message = "Bad Request"; break;
                case 401: message = "Unauthorized"; break;
                case 403: message = "Forbidden"; break;
                case 404: message = "Not Found"; break;
                case 405: message = "Method Not Allowed"; break;
                case 406: message = "Not Acceptable"; break;
                case 409: message = "Conflict"; break;
            }

            return message;
        }
    }
}
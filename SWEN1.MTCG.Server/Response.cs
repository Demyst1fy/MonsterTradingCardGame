namespace SWEN1.MTCG.Server
{
    public class Response
    {
        public int Status { get; }
        public string Message { get; }
        public string MimeType { get; }
        public int ContentLength { get; }
        public string Body { get; }

        public Response(int status, string message, string body, string mimeType = "text/plain")
        {
            Status = status;
            Message = message;
            if (mimeType is "text/plain" or "application/json")
            {
                MimeType = mimeType;
            }
            ContentLength = body.Length;
            Body = body;
        }
    }
}
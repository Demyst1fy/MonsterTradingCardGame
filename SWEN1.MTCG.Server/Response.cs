namespace SWEN1.MTCG.Server
{
    public class Response
    {
        public int Status { get; }
        public string Message { get; }
        public string ContentType { get; }
        public int ContentLength { get; }
        public string Body { get; }

        public Response(int status, string message, string body)
        {
            Status = status;
            Message = message;
            ContentType = "application/json";
            ContentLength = body.Length;
            Body = body;
        }
    }
}
namespace SWEN1.MTCG.Server
{
    public class Request
    {
        public string Method { get; private set; }
        public string Query { get; private set; }
        public string Version { get; private set; }
        public string ContentType { get; private set; }
        public string ContentLength { get; private set; }
        public string Content { get; private set; }
        public string AuthToken { get; private set; }

        public Request(string method, string query, string version, 
                        string contentType, string contentLength, string content)
        {
            Method = method;
            Query = query;
            Version = version;
            ContentType = contentType;
            ContentLength = contentLength;
            Content = content;
        }
        
        public Request(string method, string query, string version, 
            string contentType, string contentLength, string content, string authToken)
        {
            Method = method;
            Query = query;
            Version = version;
            ContentType = contentType;
            ContentLength = contentLength;
            Content = content;
            AuthToken = authToken;
        }
    }
}
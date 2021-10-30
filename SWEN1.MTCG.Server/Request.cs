using SWEN1.MTCG.Server.Interfaces;

namespace SWEN1.MTCG.Server
{
    public class Request : IRequest
    {
        public string Method { get; }
        public string Query { get; }
        public string Content { get; }
        public string AuthToken { get; }

        public Request(string method, string query, string content)
        {
            Method = method;
            Query = query;
            Content = content;
        }
        
        public Request(string method, string query, string content, string authToken)
        {
            Method = method;
            Query = query;
            Content = content;
            AuthToken = authToken;
        }
    }
}
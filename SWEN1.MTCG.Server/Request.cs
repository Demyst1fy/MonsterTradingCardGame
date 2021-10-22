using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    public class Request
    {
        public string Method { get; private set; }
        public string Resource { get; private set; }
        public string Version { get; private set; }
        public string ContentType { get; private set; }
        public string ContentLength { get; private set; }
        public string Content { get; private set; }

        public Request(string method, string resource, string version, 
                        string contentType, string contentLength, string content)
        {
            Method = method;
            Resource = resource;
            Version = version;
            ContentType = contentType;
            ContentLength = contentLength;
            Content = content;
        }
    }
}
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json.Linq;
using SWEN1.MTCG.ClassLibrary;

namespace ConsoleApp1
{
    public class ServiceHandler
    {
        private static Database db = new Database();
        public static Request ParseRequest(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return null;
            }
            
            string[] lines = data.Split("\r\n");
            
            string firstLine = lines[0];
            string[] partsFirstLine = firstLine.Split(' ');
            string method = partsFirstLine[0];
            string resource = partsFirstLine[1];
            string version = partsFirstLine[2];
            string contentType = "";
            string contentLength = "";

            foreach (var item in lines)
            {
                string[] itemType = item.Split(":");
                if (itemType[0] == "Content-Type")
                {
                    contentType = itemType[1];
                }
                if (itemType[0] == "Content-Length")
                {
                    contentLength = itemType[1];
                }
            }

            string[] tokens = data.Split("\r\n\r\n");
            string content = tokens[1];

            return new Request(method, resource, version, contentType, contentLength, content);
        }
        
        public static Response HandleRequest(string request)
        {
            Request parsedRequest = ParseRequest(request);

            if (parsedRequest == null)
            {
                return new Response(400, "Bad Request", null);
            }
            
            switch (parsedRequest.Method)
            {
                case "GET":
                    break;
                case "POST":
                    switch (parsedRequest.Resource)
                    {
                        case "/users":
                            return HandleRegistration(parsedRequest);
                        case "/sessions":
                            return HandleLogin(parsedRequest);
                    }
                    break;
                case "PUT":
                    break;
                case "DELETE":
                    break;
                default:
                    return new Response(405, "Method Not Allowed","Invalid method!");
            }
            return null;
        }

        private static Response HandleRegistration(Request request)
        {
            JObject json = JObject.Parse(request.Content);
            string username = (string)json["Username"];
            string password = (string)json["Password"];

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return new Response(400, "Bad Request", "Fields must not be empty!");
            }

            if (!db.RegisterUser(username, password))
            {
                return new Response(409, "Conflict", "User already exists!");
            }

            return new Response(201, "Created", "You are now registered!");
        }
        
        private static Response HandleLogin(Request request)
        {
            JObject json = JObject.Parse(request.Content);
            string username = (string)json["Username"];
            string password = (string)json["Password"];

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return new Response(400, "Bad Request", "Fields must not be empty!");
            }

            if (!db.LoginUser(username, password))
            {
                return new Response(400, "Bad Request", "Username or Password incorrect!");
            }

            return new Response(200, "OK", "You are logged in!");
        }
    }
}
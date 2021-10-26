using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using SWEN1.MTCG.ClassLibrary;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SWEN1.MTCG.Server
{
    public class ServiceHandler
    {
        private static readonly object _lockObj = new();
        private static Database db = new();

        private static Request ParseRequest(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return null;
            }
            
            string[] lines = data.Split(Environment.NewLine);
            
            string firstLine = lines[0];
            string[] partsFirstLine = firstLine.Split(' ');
            string method = partsFirstLine[0];
            string resource = partsFirstLine[1];
            string version = partsFirstLine[2];
            string contentType = "";
            string contentLength = "";
            string authToken = "";

            foreach (var item in lines)
            {
                string[] itemType = item.Split(": ");
                if (itemType[0] == "Content-Type")
                {
                    contentType = itemType[1];
                }
                if (itemType[0] == "Content-Length")
                {
                    contentLength = itemType[1];
                }
                if (itemType[0] == "Authorization")
                {
                    authToken = itemType[1];
                }
            }

            string[] tokens = data.Split(Environment.NewLine + Environment.NewLine);
            string content = tokens[1];

            if (string.IsNullOrEmpty(authToken))
            {
                return new Request(method, resource, version, contentType, contentLength, content);
            }
            return new Request(method, resource, version, contentType, contentLength, content, authToken);
        }
        
        private static string ParseQuery(string query)
        {
            string[] lines = query.Split("/");
            if (lines.Length == 3)
            {
                return lines[2];
            }

            return null;
        }
        
        public static Response HandleRequest(string request)
        {
            Console.WriteLine($"Request:{request} {Environment.NewLine}");
            Request parsedRequest = ParseRequest(request);

            string subQuery = ParseQuery(parsedRequest.Query);
            string usernameFromAuthKey = null;

            lock (_lockObj)
            {
                if (parsedRequest == null)
                {
                    return new Response(400, "Bad Request", null);
                }

                if (!string.IsNullOrEmpty(parsedRequest.AuthToken))
                {
                    usernameFromAuthKey = db.GetUsernameFromAuthKey(parsedRequest.AuthToken);
                }

                switch (parsedRequest.Method)
                {
                    case "GET":
                        if (parsedRequest.Query == "/cards")
                        {
                            return HandleShowStack(usernameFromAuthKey);
                        }
                        else if (parsedRequest.Query == "/deck")
                        {
                            return HandleShowDeck(usernameFromAuthKey);
                        }
                        else if (parsedRequest.Query == "/deck?format=plain")
                        {
                            return HandleShowDeckInPlain(usernameFromAuthKey);
                        }
                        else if (parsedRequest.Query == "/users/" + subQuery)
                        {
                            return HandleGetUserData(subQuery, usernameFromAuthKey);
                        }
                        else if (parsedRequest.Query == "/stats")
                        {
                            return HandleShowStats(usernameFromAuthKey);
                        }
                        else if (parsedRequest.Query == "/score")
                        {
                            return HandleShowScoreboard(usernameFromAuthKey);
                        }
                        else if (parsedRequest.Query == "/tradings")
                        {

                        }
                        else
                        {
                            return new Response(404, "Not Found", "The ressource is invalid!");
                        }

                        break;
                    case "POST":
                        if (parsedRequest.Query == "/users")
                        {
                            return HandleRegistration(parsedRequest);
                        }
                        else if (parsedRequest.Query == "/sessions")
                        {
                            return HandleLogin(parsedRequest);
                        }
                        else if (parsedRequest.Query == "/packages")
                        {

                        }
                        else if (parsedRequest.Query == "/transactions/package")
                        {

                        }
                        else if (parsedRequest.Query == "/battles")
                        {

                        }
                        else if (parsedRequest.Query == "/tradings")
                        {

                        }
                        else if (parsedRequest.Query == "/tradings/" + subQuery)
                        {

                        }
                        else
                        {
                            return new Response(404, "Not Found", "The ressource is invalid!");
                        }

                        break;
                    case "PUT":
                        if (parsedRequest.Query == "/deck")
                        {
                            return HandleConfigureDeck(parsedRequest, usernameFromAuthKey);
                        }
                        else if (parsedRequest.Query == "/users/" + subQuery)
                        {
                            return HandleEditUserData(subQuery, parsedRequest, usernameFromAuthKey);
                        }
                        else if (parsedRequest.Query == "/packages")
                        {

                        }
                        else if (parsedRequest.Query == "/transactions/package")
                        {

                        }
                        else
                        {
                            return new Response(404, "Not Found", "The ressource is invalid!");
                        }

                        break;
                    case "DELETE":
                        if (parsedRequest.Query == "/tradings/" + subQuery)
                        {

                        }
                        else
                        {
                            return new Response(404, "Not Found", "The ressource is invalid!");
                        }

                        break;
                    default:
                        return new Response(405, "Method Not Allowed", "Invalid method!");
                }

                return null;
            }
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
            string username = (string) json["Username"];
            string password = (string) json["Password"];

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
        
        private static Response HandleShowStack(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return new Response(401, "Unauthorized", "You are not logged in! (Authentication token invalid)");
            }

            List<ICard> stack = db.GetUserStack(username);
            
            if (stack.Count <= 0)
            {
                return new Response(404, "Not Found", $"{username} didn't buy packages yet!");
            }
            
            Console.WriteLine($"Card-Stack from {username}:");
            foreach (var card in stack)
            {
                Console.WriteLine($"- {card.Id}: {card.Name} ({card.Damage} Damage)");
            }
            string json = JsonConvert.SerializeObject(stack, Formatting.Indented, new StringEnumConverter());
            return new Response(200, "OK", json);
        }
        
        private static Response HandleShowDeck(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return new Response(401, "Unauthorized", "You are not logged in! (Authentication token invalid)");
            }

            List<ICard> deck = db.GetUserDeck(username);

            if (deck.Count <= 0)
            {
                return new Response(404, "Not Found", "Deck not configured yet!");
            }
            
            Console.WriteLine($"Card-Deck from {username}:");
            foreach (var card in deck)
            {
                Console.WriteLine($"- {card.Id}: {card.Name} ({card.Damage} Damage)");
            }
            string json = JsonConvert.SerializeObject(deck, Formatting.Indented, new StringEnumConverter());
            return new Response(200, "OK", json);
        }
        
        private static Response HandleShowDeckInPlain(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return new Response(401, "Unauthorized", "You are not logged in! (Authentication token invalid)");
            }

            List<ICard> deck = db.GetUserDeck(username);

            if (deck.Count <= 0)
            {
                return new Response(404, "Not Found", "Deck not configured yet!");
            }

            StringBuilder deckPlain = new StringBuilder();

            Console.WriteLine($"Card-Deck from {username}:");
            deckPlain.Append($"Card-Deck from {username}: {Environment.NewLine}");
            
            foreach (var card in deck)
            {
                Console.WriteLine($"- {card.Id}: {card.Name} ({card.Damage} Damage)");
                deckPlain.Append($"- {card.Id}: {card.Name} ({card.Damage} Damage) {Environment.NewLine}");
            }
            
            return new Response(200, "OK", deckPlain.ToString());
        }
        
        private static Response HandleConfigureDeck(Request request, string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return new Response(401, "Unauthorized", "You are not logged in! (Authentication token invalid)");
            }
            
            string[] cardArray = JsonConvert.DeserializeObject<string[]>(request.Content);

            if (cardArray.Length != 4)
            {
                return new Response(400, "Bad Request", "You have to set 4 cards for the deck!");
            }

            if (!db.ConfigureDeck(cardArray, username))
            {
                return new Response(400, "Bad Request", "Card IDs doesn't match with your chosen Cards");
            }

            return new Response(200, "OK", "Deck configured!");
        }

        private static Response HandleEditUserData(string subQuery, Request request, string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return new Response(401, "Unauthorized", "You are not logged in! (Authentication token invalid)");
            }
            if (subQuery != username)
            {
                return new Response(403, "Forbidden", "You are not allowed to access the bio from another user!");
            }
            
            JObject json = JObject.Parse(request.Content);
            string name = (string)json["Name"];
            string bio = (string)json["Bio"];
            string image = (string)json["Image"];

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(bio) || string.IsNullOrEmpty(image))
            {
                return new Response(400, "Bad Request", "Fields must not be empty!");
            }

            db.EditUserData(username, name, bio, image);

            return new Response(200, "Created", "You have changed your bio!");
        }
        
        private static Response HandleGetUserData(string subQuery, string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return new Response(401, "Unauthorized", "You are not logged in! (Authentication token invalid)");
            }

            if (subQuery != username)
            {
                return new Response(403, "Forbidden", "You are not allowed to access the bio from another user!");
            }

            UserData user = db.GetUserData(username);
            string json = JsonConvert.SerializeObject(user, Formatting.Indented, new StringEnumConverter());
            
            return new Response(200, "OK", json);
        }
        
        private static Response HandleShowStats(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return new Response(401, "Unauthorized", "You are not logged in! (Authentication token invalid)");
            }

            Stats stats = db.GetUserStats(username);
            string json = JsonConvert.SerializeObject(stats, Formatting.Indented, new StringEnumConverter());
            
            return new Response(200, "OK", json);
        }
        
        private static Response HandleShowScoreboard(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return new Response(401, "Unauthorized", "You are not logged in! (Authentication token invalid)");
            }

            List<Stats> scoreBoard = db.GetScoreBoard();
            string json = JsonConvert.SerializeObject(scoreBoard, Formatting.Indented, new StringEnumConverter());
            
            return new Response(200, "OK", json);
        }
    }
}
using System;
using System.Collections.Concurrent;
using SWEN1.MTCG.Game.Interfaces;
using SWEN1.MTCG.Server.Interfaces;

namespace SWEN1.MTCG.Server
{
    public class ServiceHandler : IServiceHandler
    {
        private IDatabase _database;
        private IHandleActions _action;

        public ServiceHandler()
        {
            _database = new Database();
            _action = new HandleActions(_database);
        }
        
        public ServiceHandler(IHandleActions action)
        {
            _action = action;
        }
        
        public IRequest ParseRequest(string data)
        {
            if (string.IsNullOrEmpty(data))
                return null;

            string[] lines = data.Split(Environment.NewLine);
            
            string firstLine = lines[0];
            string[] partsFirstLine = firstLine.Split(' ');
            string method = partsFirstLine[0];
            string resource = partsFirstLine[1];
            string authToken = "";

            foreach (var item in lines)
            {
                string[] itemType = item.Split(": ");
                if (itemType[0] == "Authorization")
                    authToken = itemType[1];
            }

            string[] tokens = data.Split(Environment.NewLine + Environment.NewLine);
            
            string content = tokens[1];
            if (string.IsNullOrEmpty(authToken))
                return new Request(method, resource, content);
            
            return new Request(method, resource, content, authToken);
        }
        
        private string ParseQuery(string query)
        {
            string[] lines = query.Split("/");
            if (lines.Length == 3)
                return lines[2];

            return null;
        }
        
        public IResponse HandleRequest(IRequest parsedRequest, ref ConcurrentQueue<IMatch> allBattles)
        {
            string subQuery = ParseQuery(parsedRequest.Query);

            string usernameFromAuthKey = null;
            if (!string.IsNullOrEmpty(parsedRequest.AuthToken))
                usernameFromAuthKey = _database.GetUsernameFromAuthKey(parsedRequest.AuthToken);
            
            switch (parsedRequest.Method)
            {
                case "GET":
                    if (parsedRequest.Query == "/transactions")
                        return _action.HandleShowTransactions(usernameFromAuthKey);
                    else if (parsedRequest.Query == "/cards")
                        return _action.HandleShowStack(usernameFromAuthKey);
                    else if (parsedRequest.Query == "/deck")
                        return _action.HandleShowDeck(usernameFromAuthKey);
                    else if (parsedRequest.Query == "/deck?format=plain")
                        return _action.HandleShowDeckInPlain(usernameFromAuthKey);
                    else if (parsedRequest.Query == "/users/" + subQuery)
                        return _action.HandleGetUserData(subQuery, usernameFromAuthKey);
                    else if (parsedRequest.Query == "/stats")
                        return _action.HandleShowStats(usernameFromAuthKey);
                    else if (parsedRequest.Query == "/score")
                        return _action.HandleShowScoreboard(usernameFromAuthKey);
                    else if (parsedRequest.Query == "/tradings")
                        return _action.HandleShowTradingDeals(usernameFromAuthKey);
                    else
                        return new Response(404, "The ressource is invalid!");
                case "POST":
                    if (parsedRequest.Query == "/users")
                        return _action.HandleRegistration(parsedRequest.Content);
                    else if (parsedRequest.Query == "/sessions")
                        return _action.HandleLogin(parsedRequest.Content);
                    else if (parsedRequest.Query == "/packages")
                        return _action.HandleCreatePackage(parsedRequest.Content, usernameFromAuthKey);
                    else if (parsedRequest.Query == "/transactions/packages")
                        return _action.HandleAcquirePackage(parsedRequest.Content, usernameFromAuthKey);
                    else if (parsedRequest.Query == "/battles")
                        return _action.HandleBattle(usernameFromAuthKey, ref allBattles);
                    else if (parsedRequest.Query == "/tradings")
                        return _action.HandleCreateTradingDeal(parsedRequest.Content, usernameFromAuthKey);
                    else if (parsedRequest.Query == "/tradings/" + subQuery)
                        return _action.HandleProcessTradingDeal(subQuery, parsedRequest.Content, usernameFromAuthKey);
                    else
                        return new Response(404, "The ressource is invalid!");
                case "PUT":
                    if (parsedRequest.Query == "/deck")
                        return _action.HandleConfigureDeck(parsedRequest.Content, usernameFromAuthKey);
                    else if (parsedRequest.Query == "/users/" + subQuery)
                        return _action.HandleEditUserData(subQuery, parsedRequest.Content, usernameFromAuthKey);
                    else
                        return new Response(404, "The ressource is invalid!");
                case "DELETE":
                    if (parsedRequest.Query == "/tradings/" + subQuery)
                        return _action.HandleDeleteTradingDeal(subQuery, usernameFromAuthKey);
                    else
                        return new Response(404, "The ressource is invalid!");
                default:
                    return new Response(405, "Invalid request!");
            }
        }
    }
}
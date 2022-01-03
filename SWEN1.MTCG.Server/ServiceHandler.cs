using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SWEN1.MTCG.Game;
using SWEN1.MTCG.Game.Interfaces;
using SWEN1.MTCG.Server.DatabaseClasses;
using SWEN1.MTCG.Server.Interfaces;

using SWEN1.MTCG.Server.JsonClasses;

namespace SWEN1.MTCG.Server
{
    public class ServiceHandler : IServiceHandler
    {
        private IDatabase _db;
        private IRequest ParseRequest(string data)
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
        
        public Response HandleRequest(string request, ref ConcurrentBag<Match> allBattles)
        {
            _db = new Database();

            Console.WriteLine($"{request} {Environment.NewLine}");
            IRequest parsedRequest = ParseRequest(request);

            string subQuery = ParseQuery(parsedRequest.Query);

            if (string.IsNullOrEmpty(request))
                return new Response(400, "Empty request!");

            string usernameFromAuthKey = null;
            if (!string.IsNullOrEmpty(parsedRequest.AuthToken))
                usernameFromAuthKey = _db.GetUsernameFromAuthKey(parsedRequest.AuthToken);

            switch (parsedRequest.Method)
            {
                case "GET":
                    if (parsedRequest.Query == "/cards")
                        return HandleShowStack(usernameFromAuthKey);
                    else if (parsedRequest.Query == "/deck")
                        return HandleShowDeck(usernameFromAuthKey);
                    else if (parsedRequest.Query == "/deck?format=plain")
                        return HandleShowDeckInPlain(usernameFromAuthKey);
                    else if (parsedRequest.Query == "/users/" + subQuery)
                        return HandleGetUserData(subQuery, usernameFromAuthKey);
                    else if (parsedRequest.Query == "/stats")
                        return HandleShowStats(usernameFromAuthKey);
                    else if (parsedRequest.Query == "/score")
                        return HandleShowScoreboard(usernameFromAuthKey);
                    else if (parsedRequest.Query == "/tradings")
                        return HandleShowTradingDeals(usernameFromAuthKey);
                    else
                        return new Response(404, "The ressource is invalid!");
                case "POST":
                    if (parsedRequest.Query == "/users")
                        return HandleRegistration(parsedRequest.Content);
                    else if (parsedRequest.Query == "/sessions")
                        return HandleLogin(parsedRequest.Content);
                    else if (parsedRequest.Query == "/packages")
                        return HandleCreatePackage(parsedRequest.Content, usernameFromAuthKey);
                    else if (parsedRequest.Query == "/transactions/packages")
                        return HandleAcquirePackage(parsedRequest.Content, usernameFromAuthKey);
                    else if (parsedRequest.Query == "/battles")
                        return HandleBattle(usernameFromAuthKey, ref allBattles);
                    else if (parsedRequest.Query == "/tradings")
                        return HandleCreateTradingDeal(parsedRequest.Content, usernameFromAuthKey);
                    else if (parsedRequest.Query == "/tradings/" + subQuery)
                        return HandleProcessTradingDeal(subQuery, parsedRequest.Content, usernameFromAuthKey);
                    else
                        return new Response(404, "The ressource is invalid!");
                case "PUT":
                    if (parsedRequest.Query == "/deck")
                        return HandleConfigureDeck(parsedRequest.Content, usernameFromAuthKey);
                    else if (parsedRequest.Query == "/users/" + subQuery)
                        return HandleEditUserData(subQuery, parsedRequest.Content, usernameFromAuthKey);
                    else
                        return new Response(404, "The ressource is invalid!");
                case "DELETE":
                    if (parsedRequest.Query == "/tradings/" + subQuery)
                        return HandleDeleteTradingDeal(subQuery, usernameFromAuthKey);
                    else
                        return new Response(404, "The ressource is invalid!");
                default:
                    return new Response(405, "Invalid method!");
            }
        }

        private Response HandleRegistration(string requestContent)
        {
            UserJson json = JsonConvert.DeserializeObject<UserJson>(requestContent);
            string username = json.Username;
            string password = json.Password;

            var register = _db.RegisterUser(username, password);

            switch (register)
            {
                case RegisterStatus.FieldEmpty: 
                    return new Response(400,"Fields must not be empty!");
                case RegisterStatus.AlreadyExist: 
                    return new Response(409,"User already exists!");
                default: 
                    return new Response(201,"You are now registered!");
            }
        }

        private Response HandleLogin(string requestContent)
        {
            UserJson json = JsonConvert.DeserializeObject<UserJson>(requestContent);
            string username = json.Username;
            string password = json.Password;

            var login = _db.LoginUser(username, password);

            switch (login)
            {
                case LoginStatus.FieldEmpty: 
                    return new Response(400,"Fields must not be empty!");
                case LoginStatus.IncorrectData: 
                    return new Response(400,"Username or Password incorrect!");
                default: 
                    return new Response(200,"You are logged in!");
            }
        }

        private Response HandleCreatePackage(string requestContent, string username)
        {
            if (string.IsNullOrEmpty(username))
                return new Response(401,"You are not logged in! (Authentication token invalid)");
            if (username != "admin")
                return new Response(403,"Only Administrator are permitted to create package!");

            PackageJson packageJson = JsonConvert.DeserializeObject<PackageJson>(requestContent);

            var checkPackageExist = _db.CheckPackageExist(packageJson.PackId);
            if (checkPackageExist)
                return new Response(400,"Package already exists!");

            foreach (var card in packageJson.Cards)
            {
                PackageCardJson cardJson = JsonConvert.DeserializeObject<PackageCardJson>(card.ToString());

                var createPackage = _db.CreatePackage(packageJson.PackId, cardJson.Id, cardJson.Name, cardJson.Damage);
                if (createPackage == CreatePackageStatus.FieldEmpty)
                    return new Response(400, "Fields must not be empty!");
                if (createPackage == CreatePackageStatus.AlreadyExist)
                    return new Response(409, "Card already exists!");
            }

            return new Response(201,"Package has been created!");
        }
        
        private Response HandleAcquirePackage(string requestContent, string username)
        {
            if (string.IsNullOrEmpty(username))
                return new Response(401,"You are not logged in! (Authentication token invalid)");
            if (string.IsNullOrEmpty(requestContent))
                return new Response(400,"Fields must not be empty!");
            
            requestContent = requestContent.Replace("\"", "");

            var acquirePackage = _db.AcquirePackage(requestContent, username);
            switch (acquirePackage)
            {
                case AcquirePackageStatus.NotExist: 
                    return new Response(400,"Package doesn't exist!");
                case AcquirePackageStatus.NoCoins: 
                    return new Response(409,"Not enough Coins!");
                case AcquirePackageStatus.Success: default:
                    return new Response(201,"Package has been bought!");
            }
        }

        private Response HandleShowStack(string username)
        {
            if (string.IsNullOrEmpty(username))
                return new Response(401,"You are not logged in! (Authentication token invalid)");

            List<ICard> stack = _db.GetUserStack(username);
            
            if (stack.Count <= 0)
                return new Response(404,$"You didn't buy packages yet!");

            string json = JsonConvert.SerializeObject(stack, Formatting.Indented, new StringEnumConverter());
            return new Response(200,json, "application/json");
        }
        
        private Response HandleShowDeck(string username)
        {
            if (string.IsNullOrEmpty(username))
                return new Response(401,"You are not logged in! (Authentication token invalid)");

            List<ICard> deck = _db.GetUserDeck(username);

            if (deck.Count <= 0)
                return new Response(404, "You didn't configure your deck yet!");

            string json = JsonConvert.SerializeObject(deck, Formatting.Indented, new StringEnumConverter());
            return new Response(200,json, "application/json");
        }
        
        private Response HandleShowDeckInPlain(string username)
        {
            if (string.IsNullOrEmpty(username))
                return new Response(401,"You are not logged in! (Authentication token invalid)");

            List<ICard> deck = _db.GetUserDeck(username);

            if (deck.Count <= 0)
                return new Response(204,"Deck not configured yet!");

            StringBuilder deckPlain = new StringBuilder();
            deckPlain.Append($"{username}'s Card-Deck: {Environment.NewLine}");
            
            foreach (var card in deck)
                deckPlain.Append($"- {card.Id}: {card.Name} ({card.Damage} Damage) {Environment.NewLine}");

            return new Response(200,deckPlain.ToString());
        }
        
        private Response HandleConfigureDeck(string requestContent, string username)
        {
            if (string.IsNullOrEmpty(username))
                return new Response(401,"You are not logged in! (Authentication token invalid)");

            string[] cardArray = JsonConvert.DeserializeObject<string[]>(requestContent);

            var configureDeck = _db.ConfigureDeck(cardArray, username);

            switch (configureDeck)
            {
                case ConfigDeckStatus.NotFourCards: 
                    return new Response(400,"You have to set 4 cards for the deck!");
                case ConfigDeckStatus.NoMatchCards: 
                    return new Response(400,"Card IDs doesn't match with your chosen Cards");
                case ConfigDeckStatus.Success: default: 
                    return new Response(200,"Deck configured!");
            }
        }

        private Response HandleEditUserData(string subQuery, string requestContent, string username)
        {
            if (string.IsNullOrEmpty(username))
                return new Response(401,"You are not logged in! (Authentication token invalid)");

            if (subQuery != username)
                return new Response(403,"You are not allowed to access the bio from another user!");

            UserinfoJson json = JsonConvert.DeserializeObject<UserinfoJson>(requestContent);
            string name = json.Name;
            string bio = json.Bio;
            string image = json.Image;

            var editUser = _db.EditUserData(username, name, bio, image);
            
            switch (editUser)
            {
                case EditUserDataStatus.FieldEmpty: 
                    return new Response(400,"Fields must not be empty!");
                case EditUserDataStatus.Success: default: 
                    return new Response(200, "You have changed your bio!");
            }
        }
        
        private Response HandleGetUserData(string subQuery, string username)
        {
            if (string.IsNullOrEmpty(username))
                return new Response(401,"You are not logged in! (Authentication token invalid)");

            if (subQuery != username)
                return new Response(403, "You are not allowed to access the bio from another user!");

            UserTable user = _db.GetUserData(username);
            string json = JsonConvert.SerializeObject(user, Formatting.Indented, new StringEnumConverter());
            
            return new Response(200,json, "application/json");
        }
        
        private Response HandleShowStats(string username)
        {
            if (string.IsNullOrEmpty(username))
                return new Response(401,"You are not logged in! (Authentication token invalid)");

            StatsTable stats = _db.GetUserStats(username);
            string json = JsonConvert.SerializeObject(stats, Formatting.Indented, new StringEnumConverter());
            
            return new Response(200,json, "application/json");
        }
        
        private Response HandleShowScoreboard(string username)
        {
            if (string.IsNullOrEmpty(username))
                return new Response(401,"You are not logged in! (Authentication token invalid)");

            List<StatsTable> scoreBoard = _db.GetScoreBoard();
            
            if (scoreBoard.Count <= 0)
                return new Response(404,"No user registered yet!");

            string json = JsonConvert.SerializeObject(scoreBoard, Formatting.Indented, new StringEnumConverter());
            return new Response(200, json, "application/json");
        }

        private Response HandleBattle(string username, ref ConcurrentBag<Match> allMatches)
        {
            if (string.IsNullOrEmpty(username))
                return new Response(401,"You are not logged in! (Authentication token invalid)");
            
            Console.WriteLine("Current Battles: " + allMatches.Count);
            
            foreach (var match in allMatches)
            {
                Console.WriteLine(match.Player2 == null
                    ? $"Battle: {match.Player1.Username} wartet"
                    : $"Battle: {match.Player1.Username} gegen {match.Player2.Username}");
            }

            List<ICard> deck = _db.GetUserDeck(username);
            if (deck.Count != 4)
                return new Response(400,"You have to set 4 cards for the deck!");
            
            Stats stats = _db.GetUserStatsWithoutUserName(username);
            
            User user = new User(username, deck, stats);

            bool joinedMatch = false;
            
            foreach (var match in allMatches)
            {
                if (match.PlayerCount < 2)
                {
                    joinedMatch = true;
                    match.AddUser(user);

                    Thread.Sleep(1000);
                    while (match.Running)
                    {
                        Console.WriteLine("Match is running.");
                        Thread.Sleep(1000);
                    }

                    return new Response(200, match.Logger.LogString());
                }
            }

            if (!joinedMatch)
            {
                Match newMatch = new Match(user);
                allMatches.Add(newMatch);

                while (newMatch.PlayerCount < 2)
                {
                    Console.WriteLine("Waiting for 2nd Player.");
                    Thread.Sleep(1000);
                }
                
                Logging logger = new Logging(newMatch.Player1, newMatch.Player2);
                newMatch.BattleAction(logger);
                
                while (newMatch.Running)
                {
                    Console.WriteLine("Match is running.");
                    Thread.Sleep(1000);
                }

                if (newMatch.Player1.Deck.Count <= 0)
                {
                    _db.UpdateStatsAfterMatch(newMatch.Player2.Username, newMatch.Player1.Username);
                }
                else if (newMatch.Player2.Deck.Count <= 0)
                {
                    _db.UpdateStatsAfterMatch(newMatch.Player1.Username, newMatch.Player2.Username);
                }
                else
                {
                    _db.UpdateStatsAfterMatchDraw(newMatch.Player1.Username, newMatch.Player2.Username);
                }

                return new Response(200, newMatch.Logger.LogString());
            }

            return new Response(400,"Error!");
        }
        private Response HandleCreateTradingDeal(string requestContent, string username)
        {
            if (string.IsNullOrEmpty(username))
                return new Response(401,"You are not logged in! (Authentication token invalid)");

            TradeJson json = JsonConvert.DeserializeObject<TradeJson>(requestContent);

            string tradeId = json.Id;
            string cardId = json.CardToTrade;
            string type = json.Type;
            double minimumDamage = json.MinimumDamage;

            var createTradingDeal = _db.CreateTradingDeal(username, tradeId, cardId, type, minimumDamage);

            switch (createTradingDeal)
            {
                case CreateTradingDealStatus.FieldEmpty: 
                    return new Response(400,"Fields must not be empty!");
                case CreateTradingDealStatus.CardInDeck: 
                    return new Response(400,"Card must not be in your deck!");
                case CreateTradingDealStatus.Success: default: 
                    return new Response(200,"You have created a trading deal!");
            }
        }
        private Response HandleShowTradingDeals(string username)
        {
            if (string.IsNullOrEmpty(username))
                return new Response(401,"You are not logged in! (Authentication token invalid)");

            List<TradeTable> tradingDeals = _db.GetTradingDeals();
            
            if (tradingDeals.Count <= 0)
                return new Response(404,"No trading deals yet!");

            string json = JsonConvert.SerializeObject(tradingDeals, Formatting.Indented, new StringEnumConverter());
            return new Response(200,json, "application/json");
        }
        private Response HandleDeleteTradingDeal(string subQuery, string username)
        {
            if (string.IsNullOrEmpty(username))
                return new Response(401,"You are not logged in! (Authentication token invalid)");

            var deleteTradingDeal = _db.DeleteTradingDeal(subQuery, username);

            switch (deleteTradingDeal)
            {
                case DeleteTradingDealStatus.FromOtherUser: 
                    return new Response(400,"User can't delete trades from other players!");
                case DeleteTradingDealStatus.Success: default:
                    return new Response(200,"You have delete a trading deal!");
            }
        }

        private Response HandleProcessTradingDeal(string subQuery, string requestContent, string username)
        {
            if (string.IsNullOrEmpty(username))
                return new Response(401,"You are not logged in! (Authentication token invalid)");

            requestContent = requestContent.Replace("\"", "");

            var processTradingDeal = _db.ProcessTradingDeal(subQuery, requestContent, username);
            
            switch (processTradingDeal)
            {
                case ProcessTradingDealStatus.NotExist: 
                    return new Response(404, "Card ID doesn't exist!");
                case ProcessTradingDealStatus.SameUser: 
                    return new Response(406, "You cannot trade with yourself!");
                case ProcessTradingDealStatus.RequestNotExist: 
                    return new Response(404, "Offered card doesn't exist!");
                case ProcessTradingDealStatus.NotWanted: 
                    return new Response(406, "Offered Card doesn't match with the searched Cardterms!");
                case ProcessTradingDealStatus.Success: default: 
                    return new Response(200, "You traded successfully!");
            }
        }
    }
}
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
    public class HandleActions : IHandleActions
    {
        private readonly IDatabase _database;
        public HandleActions()
        {
            _database = Database.GetDataBase();
        }

        public string GetUsernameFromAuthKey(string authToken)
        {
            return _database.GetUsernameFromDatabase(authToken);
        }
        
        public IResponse HandleRegistration(string requestContent)
        {
            UserJson json = JsonConvert.DeserializeObject<UserJson>(requestContent);
            string username = json.Username;
            string password = json.Password;

            var register = _database.RegisterUser(username, password);

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

        public IResponse HandleLogin(string requestContent)
        {
            UserJson json = JsonConvert.DeserializeObject<UserJson>(requestContent);
            string username = json.Username;
            string password = json.Password;

            var login = _database.LoginUser(username, password);

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

        public IResponse HandleCreatePackage(string requestContent, string username)
        {
            if (string.IsNullOrEmpty(username))
                return new Response(401,"You are not logged in! (Authentication token invalid)");
            if (username != "admin")
                return new Response(403,"Only Administrator are permitted to create package!");

            PackageJson packageJson = JsonConvert.DeserializeObject<PackageJson>(requestContent);

            var checkPackageExist = _database.CheckPackageExist(packageJson.PackId);
            if (checkPackageExist)
                return new Response(400,"Package already exists!");

            foreach (var card in packageJson.Cards)
            {
                PackageCardJson cardJson = JsonConvert.DeserializeObject<PackageCardJson>(card.ToString());

                var createPackage = _database.CreatePackage(packageJson.PackId, cardJson.Id, cardJson.Name, cardJson.Damage);
                if (createPackage == CreatePackageStatus.FieldEmpty)
                    return new Response(400, "Fields must not be empty!");
                if (createPackage == CreatePackageStatus.AlreadyExist)
                    return new Response(409, "Card already exists!");
            }

            return new Response(201,"Package has been created!");
        }
        
        public IResponse HandleAcquirePackage(string requestContent, string username)
        {
            if (string.IsNullOrEmpty(username))
                return new Response(401,"You are not logged in! (Authentication token invalid)");
            if (string.IsNullOrEmpty(requestContent))
                return new Response(400,"Fields must not be empty!");
            
            requestContent = requestContent.Replace("\"", "");

            var acquirePackage = _database.AcquirePackage(requestContent, username);
            switch (acquirePackage)
            {
                case AcquirePackageStatus.NotExist: 
                    return new Response(400,"Package doesn't exist!");
                case AcquirePackageStatus.NoCoins: 
                    return new Response(409,"Not enough Coins!");
                case AcquirePackageStatus.AlreadyOwn:
                    return new Response(409,"You already own these cards!");
                case AcquirePackageStatus.Success: default:
                    return new Response(201,"Package has been bought!");
            }
        }

        public IResponse HandleShowTransactions(string username)
        {
            if (string.IsNullOrEmpty(username))
                return new Response(401,"You are not logged in! (Authentication token invalid)");

            List<TransactionTable> transactions = _database.GetTransactions(username);
            
            if (transactions.Count <= 0)
                return new Response(404,"You didn't buy packages yet!");

            string json = JsonConvert.SerializeObject(transactions, Formatting.Indented, new StringEnumConverter());
            return new Response(200, json, "application/json");
        }

        public IResponse HandleShowStack(string username)
        {
            if (string.IsNullOrEmpty(username))
                return new Response(401,"You are not logged in! (Authentication token invalid)");

            List<ICard> stack = _database.GetUserStack(username);
            
            if (stack.Count <= 0)
                return new Response(404,"You didn't buy packages yet!");

            string json = JsonConvert.SerializeObject(stack, Formatting.Indented, new StringEnumConverter());
            return new Response(200,json, "application/json");
        }
        
        public IResponse HandleShowDeck(string username)
        {
            if (string.IsNullOrEmpty(username))
                return new Response(401,"You are not logged in! (Authentication token invalid)");

            List<ICard> deck = _database.GetUserDeck(username);

            if (deck.Count <= 0)
                return new Response(404, "You didn't configure your deck yet!");

            string json = JsonConvert.SerializeObject(deck, Formatting.Indented, new StringEnumConverter());
            return new Response(200,json, "application/json");
        }
        
        public IResponse HandleShowDeckInPlain(string username)
        {
            if (string.IsNullOrEmpty(username))
                return new Response(401,"You are not logged in! (Authentication token invalid)");

            List<ICard> deck = _database.GetUserDeck(username);

            if (deck.Count <= 0)
                return new Response(204,"Deck not configured yet!");

            StringBuilder deckPlain = new StringBuilder();
            deckPlain.Append($"{username}'s Card-Deck: {Environment.NewLine}");
            
            foreach (var card in deck)
                deckPlain.Append($"- {card.Id}: {card.Name} ({card.Damage} Damage) {Environment.NewLine}");

            return new Response(200,deckPlain.ToString());
        }
        
        public IResponse HandleConfigureDeck(string requestContent, string username)
        {
            if (string.IsNullOrEmpty(username))
                return new Response(401,"You are not logged in! (Authentication token invalid)");

            string[] cardArray = JsonConvert.DeserializeObject<string[]>(requestContent);

            var configureDeck = _database.ConfigureDeck(cardArray, username);

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

        public IResponse HandleEditUserData(string subQuery, string requestContent, string username)
        {
            if (string.IsNullOrEmpty(username))
                return new Response(401,"You are not logged in! (Authentication token invalid)");

            if (subQuery != username)
                return new Response(403,"You are not allowed to access the bio from another user!");

            UserinfoJson json = JsonConvert.DeserializeObject<UserinfoJson>(requestContent);
            string name = json.Name;
            string bio = json.Bio;
            string image = json.Image;

            var editUser = _database.EditUserData(username, name, bio, image);
            
            switch (editUser)
            {
                case EditUserDataStatus.FieldEmpty: 
                    return new Response(400,"Fields must not be empty!");
                case EditUserDataStatus.Success: default: 
                    return new Response(200, "You have changed your bio!");
            }
        }
        
        public IResponse HandleGetUserData(string subQuery, string username)
        {
            if (string.IsNullOrEmpty(username))
                return new Response(401,"You are not logged in! (Authentication token invalid)");

            if (subQuery != username)
                return new Response(403, "You are not allowed to access the bio from another user!");

            UserTable user = _database.GetUserData(username);
            string json = JsonConvert.SerializeObject(user, Formatting.Indented, new StringEnumConverter());
            
            return new Response(200,json, "application/json");
        }
        
        public IResponse HandleShowStats(string username)
        {
            if (string.IsNullOrEmpty(username))
                return new Response(401,"You are not logged in! (Authentication token invalid)");

            StatsTable stats = _database.GetUserStats(username);
            string json = JsonConvert.SerializeObject(stats, Formatting.Indented, new StringEnumConverter());
            
            return new Response(200,json, "application/json");
        }
        
        public IResponse HandleShowScoreboard(string username)
        {
            if (string.IsNullOrEmpty(username))
                return new Response(401,"You are not logged in! (Authentication token invalid)");

            List<StatsTable> scoreBoard = _database.GetScoreBoard();
            
            if (scoreBoard.Count <= 0)
                return new Response(404,"No user registered yet!");
            
            scoreBoard.Sort((x, y) => y.Elo.CompareTo(x.Elo));

            string json = JsonConvert.SerializeObject(scoreBoard, Formatting.Indented, new StringEnumConverter());
            return new Response(200, json, "application/json");
        }

        public IResponse HandleBattle(string username, ref ConcurrentQueue<IMatch> allMatches)
        {
            if (string.IsNullOrEmpty(username))
                return new Response(401,"You are not logged in! (Authentication token invalid)");
            
            Console.WriteLine("Current Battles: " + allMatches.Count);
            
            foreach (var match in allMatches)
            {
                Console.WriteLine(match.Player2 == null
                    ? $"Battle: {match.Player1.Username} is waiting in queue."
                    : $"Battle: {match.Player1.Username} vs {match.Player2.Username}");
            }

            List<ICard> deck = _database.GetUserDeck(username);
            if (deck.Count != 4)
                return new Response(400,"You have to set 4 cards for the deck!");
            
            Stats stats = _database.GetUserStatsWithoutUserName(username);
            User user = new User(username, deck, stats);

            bool joinedMatch = false;
            
            foreach (var match in allMatches)
            {
                if (match.Player2 == null)
                {
                    if(string.Equals(user.Username, match.Player1.Username))
                        return new Response(404,"You are already in queue!");
                    
                    joinedMatch = true;
                    match.AddUser(user);
                    
                    match.ProcessRunningGame();

                    return new Response(200, match.Logger.LogString());
                }
            }

            if (!joinedMatch)
            {
                IMatch newMatch = new Match(user, 100);
                allMatches.Enqueue(newMatch);

                while (newMatch.Player2 == null)
                {
                    Console.WriteLine("Waiting for 2nd Player.");
                    Thread.Sleep(1000);
                }
                
                Logging logger = new Logging(newMatch.Player1, newMatch.Player2);
                newMatch.BattleAction(logger);

                newMatch.ProcessRunningGame();

                if (newMatch.Player1.Deck.Count <= 0)
                    _database.UpdateStatsAfterMatch(newMatch.Player2.Username, newMatch.Player1.Username);
                else if (newMatch.Player2.Deck.Count <= 0)
                    _database.UpdateStatsAfterMatch(newMatch.Player1.Username, newMatch.Player2.Username);
                else
                    _database.UpdateStatsAfterMatchDraw(newMatch.Player1.Username, newMatch.Player2.Username);

                allMatches.TryDequeue(out newMatch);
                return new Response(200, newMatch?.Logger.LogString());
            }

            return new Response(400,"Error!");
        }
        public IResponse HandleCreateTradingDeal(string requestContent, string username)
        {
            if (string.IsNullOrEmpty(username))
                return new Response(401,"You are not logged in! (Authentication token invalid)");

            TradeJson json = JsonConvert.DeserializeObject<TradeJson>(requestContent);

            string tradeId = json.Id;
            string cardId = json.CardToTrade;
            string type = json.Type;
            double minimumDamage = json.MinimumDamage;

            var createTradingDeal = _database.CreateTradingDeal(username, tradeId, cardId, type, minimumDamage);

            switch (createTradingDeal)
            {
                case CreateTradingDealStatus.FieldEmpty: 
                    return new Response(400,"Fields must not be empty!");
                case CreateTradingDealStatus.CardInDeckOrNotOwn: 
                    return new Response(400,"Card is either in your deck or you don't own it!");
                case CreateTradingDealStatus.Success: default: 
                    return new Response(200,"You have created a trading deal!");
            }
        }
        public IResponse HandleShowTradingDeals(string username)
        {
            if (string.IsNullOrEmpty(username))
                return new Response(401,"You are not logged in! (Authentication token invalid)");

            List<TradeTable> tradingDeals = _database.GetTradingDeals();
            
            if (tradingDeals.Count <= 0)
                return new Response(404,"No trading deals yet!");

            string json = JsonConvert.SerializeObject(tradingDeals, Formatting.Indented, new StringEnumConverter());
            return new Response(200,json, "application/json");
        }
        
        public IResponse HandleDeleteTradingDeal(string subQuery, string username)
        {
            if (string.IsNullOrEmpty(username))
                return new Response(401,"You are not logged in! (Authentication token invalid)");

            var deleteTradingDeal = _database.DeleteTradingDeal(subQuery, username);

            switch (deleteTradingDeal)
            {
                case DeleteTradingDealStatus.FromOtherUser: 
                    return new Response(400,"User can't delete trades from other players!");
                case DeleteTradingDealStatus.Success: default:
                    return new Response(200,"You have delete a trading deal!");
            }
        }

        public IResponse HandleProcessTradingDeal(string subQuery, string requestContent, string username)
        {
            if (string.IsNullOrEmpty(username))
                return new Response(401,"You are not logged in! (Authentication token invalid)");

            requestContent = requestContent.Replace("\"", "");

            var processTradingDeal = _database.ProcessTradingDeal(subQuery, requestContent, username);
            
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
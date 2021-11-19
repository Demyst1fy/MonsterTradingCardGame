using System;
using System.Collections.Generic;
using Npgsql;
using SWEN1.MTCG.GameClasses;
using SWEN1.MTCG.GameClasses.Interfaces;
using SWEN1.MTCG.Server.DatabaseClasses;
using SWEN1.MTCG.Server.Interfaces;

namespace SWEN1.MTCG.Server
{
    public class Database : IDatabase
    {
        private readonly string host = "localhost";
        private readonly int port = 5432;
        private readonly string dbusername = "postgres";
        private readonly string dbpassword = "postgres";
        private readonly string dbname = "mtcg";

        private static NpgsqlConnection Con;
        public Database()
        {
            Con = new NpgsqlConnection($"Host={host};" +
                                       $"Port={Convert.ToString(port)};" +
                                       $"Username={dbusername};" +
                                       $"Password={dbpassword};" +
                                       $"Database={dbname}");
        }

        public RegisterStatus RegisterUser(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return RegisterStatus.FieldEmpty;
            }
            if (CheckUserAlreadyExist(username))
            {
                return RegisterStatus.AlreadyExist;
            }
            
            string fullname = char.ToUpper(username[0]) + username.Substring(1);
            
            const string sql = "INSERT INTO usertable(u_username, u_password, u_fullname)" +
                               "VALUES (:u_username, :u_password, :u_fullname)";
            
            Con.Open();
            var cmd = new NpgsqlCommand(sql, Con);
            
            cmd.Parameters.AddWithValue(":u_username", username);
            cmd.Parameters.AddWithValue(":u_password", password);
            cmd.Parameters.AddWithValue(":u_fullname", fullname);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
            Con.Close();
            
            const string sql2 = "INSERT INTO statstable(u_username)" +
                               "VALUES (:u_username)";
            
            Con.Open();
            var cmd2 = new NpgsqlCommand(sql2, Con);
            
            cmd2.Parameters.AddWithValue(":u_username", username);
            cmd2.Prepare();
            cmd2.ExecuteNonQuery();
            Con.Close();
            
            return RegisterStatus.Success;
        }
        
        public LoginStatus LoginUser(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return LoginStatus.FieldEmpty;
            }
            
            const string sql = "SELECT u_id FROM usertable WHERE u_username = :u_username AND u_password = :u_password";
            
            Con.Open();
            var cmd = new NpgsqlCommand(sql, Con);
            cmd.Parameters.AddWithValue(":u_username", username);
            cmd.Parameters.AddWithValue(":u_password", password);
            cmd.Prepare();

            var rdr = cmd.ExecuteReader();
            
            if (!rdr.HasRows)
            {
                rdr.Close();
                Con.Close();
                return LoginStatus.IncorrectData;
            }
            
            rdr.Read();
            int userId = rdr.GetInt32(0);
            rdr.Close();
            Con.Close();

            string sql2;
            if (!CheckTokenIdAlreadyExist(userId))
            {
                sql2 = "INSERT INTO tokentable (u_id, t_token) VALUES(:u_id, :t_token)";
            }
            else
            {
                sql2 = "UPDATE tokentable SET t_token = :t_token WHERE u_id = :u_id";
            }
            
            Con.Open();
            var cmd2 = new NpgsqlCommand(sql2, Con);
            cmd2.Parameters.AddWithValue(":u_id", userId);
            cmd2.Parameters.AddWithValue(":t_token","Basic " + username + "-mtcgToken");
            cmd2.Prepare();
            cmd2.ExecuteNonQuery();
            Con.Close();
            
            return LoginStatus.Success;
        }
        
        public Usertable GetUserData(string username)
        {
            const string sql = "SELECT u_coins, u_fullname, u_bio, u_image FROM usertable WHERE u_username = :u_username";
            
            Con.Open();
            var cmd = new NpgsqlCommand(sql, Con);
            cmd.Parameters.AddWithValue(":u_username", username);

            var rdr = cmd.ExecuteReader();
            
            Usertable user = null;
            
            while(rdr.Read())
            {
                var coins = rdr.GetInt32(0);
                var fullname = rdr.GetString(1);
                var bio = rdr.GetString(2);
                var image = rdr.GetString(3);

                user = new Usertable(username, coins, fullname, bio, image);
            }
            rdr.Close();
            Con.Close();
            return user;
        }
        
        public EditUserDataStatus EditUserData(string username, string fullname, string bio, string image)
        {
            if (string.IsNullOrEmpty(fullname) || string.IsNullOrEmpty(bio) || string.IsNullOrEmpty(image))
            {
                return EditUserDataStatus.FieldEmpty;
            }
            
            const string sql = "UPDATE usertable SET u_fullname = :u_fullname, u_bio = :u_bio, u_image = :u_image WHERE u_username = :u_username";
            
            Con.Open();
            var cmd = new NpgsqlCommand(sql, Con);
            cmd.Parameters.AddWithValue(":u_fullname", fullname);
            cmd.Parameters.AddWithValue(":u_bio", bio);
            cmd.Parameters.AddWithValue(":u_image", image);
            cmd.Parameters.AddWithValue(":u_username", username);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
            Con.Close();
            
            return EditUserDataStatus.Success;
        }
        
        
        public void InsertNewCardStack(int uid, string cid, string _cardName, int _dmg)
        {
            const string sql = "INSERT INTO cardtable(u_id, c_id, c_name, c_damage, c_indeck)" +
                               "VALUES (:u_id, :c_id, :c_name, :c_damage, :c_indeck)";
            Con.Open();
            var cmd = new NpgsqlCommand(sql, Con);
            cmd.Parameters.AddWithValue(":u_id", uid);
            cmd.Parameters.AddWithValue(":c_id", cid);
            cmd.Parameters.AddWithValue(":c_name", _cardName);
            cmd.Parameters.AddWithValue(":c_damage", _dmg);
            cmd.Parameters.AddWithValue(":c_indeck", false);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
            Con.Close();
        }

        public ConfigDeckStatus ConfigureDeck(string[] chosenCardIDs, string username)
        {
            if (chosenCardIDs.Length != 4)
            {
                return ConfigDeckStatus.NotFourCards;
            }
            
            const string sql = "SELECT c_id FROM cardtable WHERE u_username = :u_username";
            
            Con.Open();
            var cmd = new NpgsqlCommand(sql, Con);
            cmd.Parameters.AddWithValue(":u_username", username);
            cmd.Prepare();
            
            var rdr = cmd.ExecuteReader();
            
            List<string> stackCardIDs = new List<string>();
            
            while(rdr.Read())
            {
                stackCardIDs.Add(rdr.GetString(0));
            }

            rdr.Close();
            Con.Close();
            
            bool[] found = { false, false, false, false };
            
            for (var i = 0; i < chosenCardIDs.Length; i++)
            {
                for (var j = 0; j < stackCardIDs.Count; j++)
                {
                    if (chosenCardIDs[i] == stackCardIDs[j])
                    {
                        found[i] = true;
                        break;
                    }
                }
            }
            
            if (!found[0] || !found[1] || !found[2] || !found[3])
            {
                return ConfigDeckStatus.NoMatchCards;
            }

            const string sql2 = "UPDATE cardtable SET c_indeck = :c_indeck WHERE u_username = :u_username";
            
            Con.Open();
            var cmd2 = new NpgsqlCommand(sql2, Con);
            cmd2.Parameters.AddWithValue(":c_indeck", false);
            cmd2.Parameters.AddWithValue(":u_username", username);
            cmd2.Prepare();
            cmd2.ExecuteNonQuery();
            Con.Close();

            for (var i = 0; i < chosenCardIDs.Length; i++)
            {
                const string sql3 = "UPDATE cardtable SET c_indeck = :c_indeck WHERE u_username = :u_username AND c_id = :c_id";
                
                Con.Open();
                var cmd3 = new NpgsqlCommand(sql3, Con);
                cmd3.Parameters.AddWithValue(":c_indeck", true);
                cmd3.Parameters.AddWithValue(":u_username", username);
                cmd3.Parameters.AddWithValue(":c_id", chosenCardIDs[i]);
                cmd3.Prepare();
                cmd3.ExecuteNonQuery();
                Con.Close();
            }

            return ConfigDeckStatus.Success;
        }
        
        public List<ICard> GetUserStack(string username)
        {
            const string sql = "SELECT c_id, c_name, c_damage FROM cardtable WHERE u_username = :u_username";
            Con.Open();
            var cmd = new NpgsqlCommand(sql, Con);
            cmd.Parameters.AddWithValue(":u_username", username);
            cmd.Prepare();
            
            var rdr = cmd.ExecuteReader();
            
            List<ICard> stack = new List<ICard>();
            while(rdr.Read())
            {
                var id = rdr.GetString(0);
                var name = rdr.GetString(1);
                var damage = rdr.GetDouble(2);
                
                stack.Add(new Card(id, name, damage));
            }
            
            rdr.Close();
            Con.Close();
            return stack;
        }
        public List<ICard> GetUserDeck(string username)
        {
            const string sql = "SELECT c_id, c_name, c_damage FROM cardtable WHERE u_username = :u_username AND c_indeck = :c_indeck";
            
            Con.Open();
            var cmd = new NpgsqlCommand(sql, Con);
            cmd.Parameters.AddWithValue(":u_username", username);
            cmd.Parameters.AddWithValue(":c_indeck", true);
            cmd.Prepare();
            
            var rdr = cmd.ExecuteReader();
            
            List<ICard> deck = new List<ICard>();
            while(rdr.Read())
            {
                var id = rdr.GetString(0);
                var name = rdr.GetString(1);
                var damage = rdr.GetDouble(2);
                
                deck.Add(new Card(id, name, damage));
            }
            
            rdr.Close();
            Con.Close();
            return deck;
        }

        public Statstable GetUserStats(string username)
        {
            const string sql = "SELECT s_wins, s_losses, s_draws, s_elo FROM statstable WHERE u_username = :u_username";
            
            Con.Open();
            var cmd = new NpgsqlCommand(sql, Con);
            cmd.Parameters.AddWithValue(":u_username", username);
            cmd.Prepare();
            
            var rdr = cmd.ExecuteReader();
            
            Statstable stats = null;
            
            while(rdr.Read())
            {
                var wins = rdr.GetInt32(0);
                var losses = rdr.GetInt32(1);
                var draws = rdr.GetInt32(2);
                var elo = rdr.GetInt32(3);
                double winRate = 0;
                
                if (wins > 0 || losses > 0 || draws > 0)
                {
                    winRate = Math.Round((wins / (double) (wins + losses + draws)), 2);
                }

                stats = new Statstable(username, wins, losses, draws, elo, winRate);
            }
            
            rdr.Close();
            Con.Close();
            return stats;
        }
        
        public List<Statstable> GetScoreBoard()
        {
            const string sql = "SELECT * FROM statstable";

            Con.Open();
            var cmd = new NpgsqlCommand(sql, Con);
            cmd.Prepare();
            
            var rdr = cmd.ExecuteReader();
            
            List<Statstable> scoreBoard = new List<Statstable>();
            
            while(rdr.Read())
            {
                var wins = rdr.GetInt32(0);
                var losses = rdr.GetInt32(1);
                var draws = rdr.GetInt32(2);
                var elo = rdr.GetInt32(3);
                var user = rdr.GetString(4);
                double winRate = 0;
                
                if (wins > 0 || losses > 0 || draws > 0)
                {
                    winRate = Math.Round((wins / (double) (wins + losses + draws)), 2);
                }

                scoreBoard.Add(new Statstable(user, wins, losses, draws, elo, winRate));
            }
            
            rdr.Close();
            Con.Close();
            return scoreBoard;
        }

        public CreateTradingDealStatus CreateTradingDeal(string username, string tradeId, string cardId, string searchType, double? minimumDamage)
        {
            if (string.IsNullOrEmpty(tradeId) || string.IsNullOrEmpty(cardId) || string.IsNullOrEmpty(searchType) || minimumDamage == null)
            {
                return CreateTradingDealStatus.FieldEmpty;
            }
            
            const string sql = "SELECT * FROM cardtable WHERE c_id = :c_id AND c_indeck = :c_indeck";
            
            Con.Open();
            var cmd = new NpgsqlCommand(sql, Con);
            cmd.Parameters.AddWithValue(":c_id", cardId);
            cmd.Parameters.AddWithValue(":c_indeck", false);
            cmd.Prepare();
            
            var rdr = cmd.ExecuteReader();

            if (!rdr.HasRows)
            {
                rdr.Close();
                Con.Close();
                return CreateTradingDealStatus.CardInDeck;
            }

            rdr.Close();
            Con.Close();
            
            const string sql2 = "INSERT INTO tradetable(u_username, tr_id, c_id, tr_searchtype, tr_minimumdamage)" +
                               "VALUES (:u_username, :tr_id, :c_id, :tr_searchtype, :tr_minimumdamage)";
            
            Con.Open();
            var cmd2 = new NpgsqlCommand(sql2, Con);
            
            cmd2.Parameters.AddWithValue(":u_username", username);
            cmd2.Parameters.AddWithValue(":tr_id", tradeId);
            cmd2.Parameters.AddWithValue(":c_id", cardId);
            cmd2.Parameters.AddWithValue(":tr_searchtype", searchType);
            cmd2.Parameters.AddWithValue(":tr_minimumdamage", minimumDamage);
            cmd2.Prepare();
            cmd2.ExecuteNonQuery();
            Con.Close();
            
            return CreateTradingDealStatus.Success;
        }
        public List<Tradetable> GetTradingDeals()
        {
            const string sql = "SELECT * FROM tradetable";
            
            Con.Open();
            var cmd = new NpgsqlCommand(sql, Con);
            cmd.Prepare();
            
            var rdr = cmd.ExecuteReader();
            
            List<Tradetable> tradingDeals = new List<Tradetable>();
            
            while(rdr.Read())
            {
                var tradeId = rdr.GetString(0);
                var cardId = rdr.GetString(1);
                var searchType = rdr.GetString(2);
                var minimumDamage = rdr.GetDouble(3);
                var username = rdr.GetString(4);
                
                tradingDeals.Add(new Tradetable(username, tradeId, cardId, searchType, minimumDamage));
            }
            
            rdr.Close();
            Con.Close();
            return tradingDeals;
        }
        public DeleteTradingDealStatus DeleteTradingDeal(string tradeId, string username)
        {
            const string sql = "DELETE FROM tradetable WHERE tr_id = :tr_id AND u_username = :u_username";
            
            Con.Open();
            var cmd = new NpgsqlCommand(sql, Con);
            cmd.Parameters.AddWithValue(":tr_id", tradeId);
            cmd.Parameters.AddWithValue(":u_username", username);
            cmd.Prepare();
            
            int result = cmd.ExecuteNonQuery();
            Con.Close();
            
            if (result == -1)
            {
                return DeleteTradingDealStatus.FromOtherUser;
            }

            return DeleteTradingDealStatus.Success;
        }
        
        public ProcessTradingDealStatus ProcessTradingDeal(string tradeId, string offeredCardId, string username)
        {
            const string sql = "SELECT c_id, tr_searchtype, tr_minimumdamage, u_username FROM tradetable WHERE tr_id = :tr_id";
            
            Con.Open();
            var cmd = new NpgsqlCommand(sql, Con);
            cmd.Parameters.AddWithValue(":tr_id", tradeId);
            cmd.Prepare();
            var rdr = cmd.ExecuteReader();

            if (!rdr.HasRows)
            {
                rdr.Close();
                Con.Close();
                return ProcessTradingDealStatus.NotExist;
            }

            string wantedCardId = "";
            string wantedSearchType = "";
            double wantedMinimumDamage = 0;
            string tradeFromUser = "";
            
            while(rdr.Read())
            {
                wantedCardId = rdr.GetString(0);
                wantedSearchType = rdr.GetString(1);
                wantedMinimumDamage = rdr.GetDouble(2);
                tradeFromUser = rdr.GetString(3);
            }
            
            rdr.Close();
            Con.Close();

            if (username == tradeFromUser)
            {
                return ProcessTradingDealStatus.SameUser;
            }

            const string sql2 = "SELECT c_name, c_damage FROM cardtable WHERE c_id = :c_id AND c_indeck = :c_indeck";
            Con.Open();
            var cmd2 = new NpgsqlCommand(sql2, Con);
            cmd2.Parameters.AddWithValue(":c_id", offeredCardId);
            cmd2.Parameters.AddWithValue(":c_indeck", false);
            cmd2.Prepare();
            var rdr2 = cmd2.ExecuteReader();
            
            if (!rdr2.HasRows)
            {
                rdr2.Close();
                Con.Close();
                return ProcessTradingDealStatus.RequestNotExist;
            }

            string offeredCardName = "";
            double offeredCardDamage = 0;

            while(rdr2.Read())
            {
                offeredCardName = rdr2.GetString(0);
                offeredCardDamage = rdr2.GetDouble(1);
            }
            
            rdr2.Close();
            Con.Close();

            string offeredCardType = "Spell";
            if (!offeredCardName.Contains("Spell"))
            {
                offeredCardType = "Monster";
            }

            if (offeredCardDamage < wantedMinimumDamage || offeredCardType != wantedSearchType)
            {
                return ProcessTradingDealStatus.NotWanted;
            }

            UpdateCardUser(username, wantedCardId);
            UpdateCardUser(tradeFromUser, offeredCardId);
            DeleteTradingDeal(tradeId, tradeFromUser);
            
            return ProcessTradingDealStatus.Success;
        }

        public void UpdateCardUser(string username, string cardId)
        {
            const string sql = "UPDATE cardtable SET u_username = :u_username WHERE c_id = :c_id";
            Con.Open();
            var cmd = new NpgsqlCommand(sql, Con);
            cmd.Parameters.AddWithValue(":u_username", username);
            cmd.Parameters.AddWithValue(":c_id", cardId);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
            Con.Close();
        }
        
        public bool CheckUserAlreadyExist(string _username)
        {
            const string sql = "SELECT u_username FROM usertable WHERE u_username = :u_username";
            
            Con.Open();
            var cmd = new NpgsqlCommand(sql, Con);
            cmd.Parameters.AddWithValue(":u_username", _username);
            cmd.Prepare();
            var rdr = cmd.ExecuteReader();
            var result = rdr.Read();
            
            rdr.Close();
            Con.Close();
            
            return result;
        }
        
        public bool CheckTokenIdAlreadyExist(int _uid)
        {
            const string sql = "SELECT t_id FROM tokentable WHERE u_id = :u_id";
            
            Con.Open();
            var cmd = new NpgsqlCommand(sql, Con);
            cmd.Parameters.AddWithValue(":u_id", _uid);
            cmd.Prepare();

            var rdr = cmd.ExecuteReader();
            var result = rdr.Read();
            
            rdr.Close();
            Con.Close();
            
            return result;
        }
        
        public string GetUsernameFromAuthKey(string authToken)
        {
            if (string.IsNullOrEmpty(authToken))
            {
                return null;
            }
            
            string sql = "SELECT usertable.u_username FROM usertable INNER JOIN tokentable ON usertable.u_id = tokentable.u_id WHERE tokentable.t_token = :t_token";
            
            Con.Open();
            var cmd = new NpgsqlCommand(sql, Con);
            cmd.Parameters.AddWithValue(":t_token", authToken);
            cmd.Prepare();

            var rdr = cmd.ExecuteReader();

            string username = "";
            if (rdr.HasRows)
            {
                rdr.Read();
                username = rdr.GetString(0);
            }
            
            rdr.Close();
            Con.Close();
            
            return username;
        }
    }
}
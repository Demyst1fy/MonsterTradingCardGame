using System;
using System.Collections.Generic;
using System.Linq;
using Npgsql;

using SWEN1.MTCG.Game;
using SWEN1.MTCG.Game.Interfaces;
using SWEN1.MTCG.Server.DatabaseClasses;
using SWEN1.MTCG.Server.Interfaces;

namespace SWEN1.MTCG.Server
{
    public class Database : IDatabase
    {
        private static Database _dbinstance;
        
        private readonly string _host = "localhost";
        private readonly int _port = 5432;
        private readonly string _dbusername = "postgres";
        private readonly string _dbpassword = "postgres";
        private readonly string _dbname = "mtcg";

        private string _conString;
        
        private Database()
        {
            _conString = $"Host={_host};" +
                         $"Port={Convert.ToString(_port)};" +
                         $"Username={_dbusername};" +
                         $"Password={_dbpassword};" +
                         $"Database={_dbname}";
        }

        public static IDatabase GetDataBase()
        {
            if (_dbinstance == null)
                _dbinstance = new Database();

            return _dbinstance;
        }

        private NpgsqlConnection ConOpen()
        {
            var con = new NpgsqlConnection(_conString);
            con.Open();
            return con;
        }

        public RegisterStatus RegisterUser(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return RegisterStatus.FieldEmpty;
            
            if (CheckUserExist(username))
                return RegisterStatus.AlreadyExist;

            string fullname = char.ToUpper(username[0]) + username.Substring(1);
            
            const string sql = "INSERT INTO \"UserTable\"(u_username, u_password, u_fullname)" +
                               "VALUES (@u_username, @u_password, @u_fullname)";

            var con = ConOpen();
            var cmd = new NpgsqlCommand(sql, con);
            
            cmd.Parameters.AddWithValue("@u_username", username);
            cmd.Parameters.AddWithValue("@u_password", password);
            cmd.Parameters.AddWithValue("@u_fullname", fullname);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
            
            const string sql2 = "INSERT INTO \"StatsTable\"(u_username)" +
                                    "VALUES (@u_username)";
            con.Close();
            
            
            con = ConOpen();
            var cmd2 = new NpgsqlCommand(sql2, con);
            cmd2.Parameters.AddWithValue("@u_username", username);
            cmd2.Prepare();
            cmd2.ExecuteNonQuery();

            con.Close();
            return RegisterStatus.Success;
        }
        
        public LoginStatus LoginUser(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return LoginStatus.FieldEmpty;

            const string sql = "SELECT u_id FROM \"UserTable\" WHERE u_username = @u_username AND u_password = @u_password";

            var con = ConOpen();
            var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@u_username", username);
            cmd.Parameters.AddWithValue("@u_password", password);
            cmd.Prepare();

            var rdr = cmd.ExecuteReader();
            
            if (!rdr.HasRows)
            {
                rdr.Close();
                con.Close();
                return LoginStatus.IncorrectData;
            }
            
            rdr.Read();
            int userId = rdr.GetInt32(0);
            rdr.Close();
            con.Close();

            var sql2 = !CheckTokenIdExist(userId) 
                ? 
                "INSERT INTO \"TokenTable\" (u_id, t_token) VALUES(@u_id, @t_token)" 
                : 
                "UPDATE \"TokenTable\" SET t_token = @t_token WHERE u_id = @u_id";

            con = ConOpen();
            var cmd2 = new NpgsqlCommand(sql2, con);
            cmd2.Parameters.AddWithValue("@u_id", userId);
            cmd2.Parameters.AddWithValue("@t_token","Basic " + username + "-mtcgToken");
            cmd2.Prepare();
            cmd2.ExecuteNonQuery();
            con.Close();
            
            return LoginStatus.Success;
        }
        
        public UserTable GetUserData(string username)
        {
            const string sql = "SELECT u_coins, u_spentcoins, u_fullname, u_bio, u_image FROM \"UserTable\" WHERE u_username = @u_username";
            
            var con = ConOpen();
            var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@u_username", username);

            var rdr = cmd.ExecuteReader();
            
            UserTable user = null;
            
            while(rdr.Read())
            {
                var coins = rdr.GetInt32(0);
                var spentCoins = rdr.GetInt32(1);
                var fullname = rdr.GetString(2);
                var bio = rdr.GetString(3);
                var image = rdr.GetString(4);

                user = new UserTable(username, coins, spentCoins, fullname, bio, image);
            }
            
            rdr.Close();
            con.Close();
            return user;
        }
        
        public EditUserDataStatus EditUserData(string username, string fullname, string bio, string image)
        {
            if (string.IsNullOrEmpty(fullname) || string.IsNullOrEmpty(bio) || string.IsNullOrEmpty(image))
                return EditUserDataStatus.FieldEmpty;

            const string sql = "UPDATE \"UserTable\" SET u_fullname = @u_fullname, u_bio = @u_bio, u_image = @u_image WHERE u_username = @u_username";
            
            var con = ConOpen();
            var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@u_fullname", fullname);
            cmd.Parameters.AddWithValue("@u_bio", bio);
            cmd.Parameters.AddWithValue("@u_image", image);
            cmd.Parameters.AddWithValue("@u_username", username);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
            con.Close();
            
            return EditUserDataStatus.Success;
        }

        public CreatePackageStatus CreatePackage(string packId, string cardId, string cardName, double cardDamage)
        {
            if (string.IsNullOrEmpty(cardId) || string.IsNullOrEmpty(cardName))
                return CreatePackageStatus.FieldEmpty;
            
            if (CheckCardExist(cardId))
                return CreatePackageStatus.AlreadyExist;

            const string sql = "INSERT INTO \"PackageTable\"(p_id, p_cid, p_cname, p_cdamage)" +
                               "VALUES (@p_id, @p_cid, @p_cname, @p_cdamage)";
            var con = ConOpen();
            var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@p_id", packId);
            cmd.Parameters.AddWithValue("@p_cid", cardId);
            cmd.Parameters.AddWithValue("@p_cname", cardName);
            cmd.Parameters.AddWithValue("@p_cdamage", cardDamage);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
            con.Close();
            return CreatePackageStatus.Success;
        }

        public AcquirePackageStatus AcquirePackage(string packId, string username)
        {
            if (!CheckPackageExist(packId))
                return AcquirePackageStatus.NotExist;
            
            if (GetUserCoins(username) < 5)
                return AcquirePackageStatus.NoCoins;

            List<CardTable> cards = GetCardsFromPackage(packId);
            
            if(CheckCardAlreadyOwn(username, cards)) 
                return AcquirePackageStatus.AlreadyOwn;
            
            foreach (var card in cards)
            {
                const string sql = "INSERT INTO \"UserCardTable\"(c_id, c_name, c_damage, c_indeck, u_username)" +
                                   "VALUES (@c_id, @c_name, @c_damage, @c_indeck, @u_username)";
                
                var con = ConOpen();
                var cmd = new NpgsqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@c_id", card.Id);
                cmd.Parameters.AddWithValue("@c_name", card.Name);
                cmd.Parameters.AddWithValue("@c_damage", card.Damage);
                cmd.Parameters.AddWithValue("@c_indeck", false);
                cmd.Parameters.AddWithValue("@u_username", username);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
                con.Close();
            }

            TransactionPackage(packId, username);

            return AcquirePackageStatus.Success;
        }
        
        public List<TransactionTable> GetTransactions(string username)
        {
            const string sql = "SELECT ta_pid, ta_cost, ta_datetime FROM \"TransactionTable\" WHERE u_username = @u_username";

            var con = ConOpen();
            var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@u_username", username);
            cmd.Prepare();
            
            var rdr = cmd.ExecuteReader();
            
            List<TransactionTable> transactions = new List<TransactionTable>();
            
            while(rdr.Read())
            {
                var packId = rdr.GetString(0);
                var cost = rdr.GetInt32(1);
                var dateTime = rdr.GetString(2);

                transactions.Add(new TransactionTable(packId, cost, dateTime));
            }
            
            rdr.Close();
            con.Close();
            
            return transactions;
        }

        private List<CardTable> GetCardsFromPackage(string packId)
        {
            const string sql = "SELECT p_cid, p_cname, p_cdamage FROM \"PackageTable\" WHERE p_id = @p_id";
            var con = ConOpen();
            var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@p_id", packId);
            var rdr = cmd.ExecuteReader();
            
            List<CardTable> cards = new List<CardTable>();
            
            while(rdr.Read())
            {
                var cardId = rdr.GetString(0);
                var cardName = rdr.GetString(1);
                var cardDamage = rdr.GetDouble(2);

                cards.Add(new CardTable(cardId, cardName, cardDamage));
            }
            rdr.Close();
            con.Close();
            
            return cards;
        }

        private bool CheckCardAlreadyOwn(string username, List<CardTable> packCards)
        {
            foreach (var packCard in packCards)
            {
                const string sql = "SELECT * FROM \"UserCardTable\" WHERE u_username = @u_username AND c_id = @c_id";
                
                var con = ConOpen();
                var cmd = new NpgsqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@u_username", username);
                cmd.Parameters.AddWithValue("@c_id", packCard.Id);
                var rdr = cmd.ExecuteReader();

                bool result = rdr.HasRows;

                rdr.Close();
                con.Close();
                
                if (result)
                    return true;
            }

            return false;
        }

        private void TransactionPackage(string packId, string username)
        {
            int cost = 5;
            
            const string sql = "UPDATE \"UserTable\" SET u_coins = u_coins - @cost, u_spentcoins = u_spentcoins + @cost WHERE u_username = @u_username";
            
            var con = ConOpen();
            var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@cost", cost);
            cmd.Parameters.AddWithValue("@u_username", username);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
            con.Close();
            
            const string sql2 = "INSERT INTO \"TransactionTable\"(ta_pid, ta_cost, ta_datetime, u_username)" +
                               "VALUES (@ta_pid, @ta_cost, @ta_datetime, @u_username)";
            
            var con2 = ConOpen();
            var cmd2 = new NpgsqlCommand(sql2, con2);
            cmd2.Parameters.AddWithValue("@ta_pid", packId);
            cmd2.Parameters.AddWithValue("@ta_cost", cost);
            cmd2.Parameters.AddWithValue("@ta_datetime", DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss"));
            cmd2.Parameters.AddWithValue("@u_username", username);
            cmd2.Prepare();
            cmd2.ExecuteNonQuery();
            con2.Close();
        }
        
        public void InsertNewCardStack(int uid, string cid, string _cardName, int _dmg)
        {
            const string sql = "INSERT INTO \"UserCardTable\"(u_id, c_id, c_name, c_damage, c_indeck)" +
                               "VALUES (@u_id, @c_id, @c_name, @c_damage, @c_indeck)";
            var con = ConOpen();
            var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@u_id", uid);
            cmd.Parameters.AddWithValue("@c_id", cid);
            cmd.Parameters.AddWithValue("@c_name", _cardName);
            cmd.Parameters.AddWithValue("@c_damage", _dmg);
            cmd.Parameters.AddWithValue("@c_indeck", false);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public ConfigDeckStatus ConfigureDeck(string[] chosenCardIDs, string username)
        {
            if (chosenCardIDs.Length != 4)
                return ConfigDeckStatus.NotFourCards;

            const string sql = "SELECT c_id FROM \"UserCardTable\" WHERE u_username = @u_username";
            
            var con = ConOpen();
            var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@u_username", username);
            cmd.Prepare();
            
            var rdr = cmd.ExecuteReader();
            
            List<string> stackCardIDs = new List<string>();
            
            while(rdr.Read())
                stackCardIDs.Add(rdr.GetString(0));

            rdr.Close();
            con.Close();
            
            bool[] found = { false, false, false, false };
            
            for (var i = 0; i < chosenCardIDs.Length; i++)
                if (stackCardIDs.Any(stackCard => chosenCardIDs[i] == stackCard))
                    found[i] = true;

            if (!found[0] || !found[1] || !found[2] || !found[3])
                return ConfigDeckStatus.NoMatchCards;

            const string sql2 = "UPDATE \"UserCardTable\" SET c_indeck = @c_indeck WHERE u_username = @u_username";
            
            con = ConOpen();
            var cmd2 = new NpgsqlCommand(sql2, con);
            cmd2.Parameters.AddWithValue("@c_indeck", false);
            cmd2.Parameters.AddWithValue("@u_username", username);
            cmd2.Prepare();
            cmd2.ExecuteNonQuery();
            con.Close();

            foreach (var chosenCardID in chosenCardIDs)
            {
                const string sql3 = "UPDATE \"UserCardTable\" SET c_indeck = @c_indeck WHERE u_username = @u_username AND c_id = @c_id";
                
                con = ConOpen();
                var cmd3 = new NpgsqlCommand(sql3, con);
                cmd3.Parameters.AddWithValue("@c_indeck", true);
                cmd3.Parameters.AddWithValue("@u_username", username);
                cmd3.Parameters.AddWithValue("@c_id", chosenCardID);
                cmd3.Prepare();
                cmd3.ExecuteNonQuery();
                con.Close();
            }

            return ConfigDeckStatus.Success;
        }
        
        public List<ICard> GetUserStack(string username)
        {
            const string sql = "SELECT c_id, c_name, c_damage FROM \"UserCardTable\" WHERE u_username = @u_username";
            var con = ConOpen();
            var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@u_username", username);
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
            con.Close();
            return stack;
        }
        public List<ICard> GetUserDeck(string username)
        {
            const string sql = "SELECT c_id, c_name, c_damage FROM \"UserCardTable\" WHERE u_username = @u_username AND c_indeck = @c_indeck";
            
            var con = ConOpen();
            var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@u_username", username);
            cmd.Parameters.AddWithValue("@c_indeck", true);
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
            con.Close();
            return deck;
        }

        public StatsTable GetUserStats(string username)
        {
            const string sql = "SELECT s_wins, s_losses, s_draws, s_elo FROM \"StatsTable\" WHERE u_username = @u_username";
            
            var con = ConOpen();
            var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@u_username", username);
            cmd.Prepare();
            
            var rdr = cmd.ExecuteReader();
            
            StatsTable stats = null;
            
            while(rdr.Read())
            {
                var wins = rdr.GetInt32(0);
                var losses = rdr.GetInt32(1);
                var draws = rdr.GetInt32(2);
                var elo = rdr.GetInt32(3);
                double winRate = 0;
                
                if (wins > 0 || losses > 0 || draws > 0)
                    winRate = Math.Round((wins / (double) (wins + losses + draws)) * 100, 2);

                stats = new StatsTable(username, wins, losses, draws, elo, winRate);
            }
            
            rdr.Close();
            con.Close();
            return stats;
        }
        
        public Stats GetUserStatsWithoutUserName(string username)
        {
            const string sql = "SELECT s_wins, s_losses, s_draws, s_elo FROM \"StatsTable\" WHERE u_username = @u_username";
            
            var con = ConOpen();
            var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@u_username", username);
            cmd.Prepare();
            
            var rdr = cmd.ExecuteReader();
            
            Stats stats = null;
            
            while(rdr.Read())
            {
                var wins = rdr.GetInt32(0);
                var losses = rdr.GetInt32(1);
                var draws = rdr.GetInt32(2);
                var elo = rdr.GetInt32(3);

                stats = new Stats(wins, losses, draws, elo);
            }
            
            rdr.Close();
            con.Close();
            return stats;
        }
        
        public List<StatsTable> GetScoreBoard()
        {
            const string sql = "SELECT * FROM \"StatsTable\"";

            var con = ConOpen();
            var cmd = new NpgsqlCommand(sql, con);
            cmd.Prepare();
            
            var rdr = cmd.ExecuteReader();
            
            List<StatsTable> scoreBoard = new List<StatsTable>();
            
            while(rdr.Read())
            {
                var wins = rdr.GetInt32(0);
                var losses = rdr.GetInt32(1);
                var draws = rdr.GetInt32(2);
                var elo = rdr.GetInt32(3);
                var user = rdr.GetString(4);
                double winRate = 0;
                
                if (wins > 0 || losses > 0 || draws > 0)
                    winRate = Math.Round((wins / (double) (wins + losses + draws)), 2);

                scoreBoard.Add(new StatsTable(user, wins, losses, draws, elo, winRate));
            }
            
            rdr.Close();
            con.Close();
            return scoreBoard;
        }

        public void UpdateStatsAfterMatch(string winner, string loser)
        {
            UpdateStatsWin(winner);
            UpdateStatsLoss(loser);
        }
        
        public void UpdateStatsAfterMatchDraw(string player1, string player2)
        {
            UpdateStatsDraw(player1);
            UpdateStatsDraw(player2);
        }

        public CreateTradingDealStatus CreateTradingDeal(string username, string tradeId, string cardId, string searchType, double? minimumDamage)
        {
            if (string.IsNullOrEmpty(tradeId) || string.IsNullOrEmpty(cardId) || string.IsNullOrEmpty(searchType) || minimumDamage == null)
                return CreateTradingDealStatus.FieldEmpty;

            const string sql = "SELECT * FROM \"UserCardTable\" WHERE c_id = @c_id AND c_indeck = @c_indeck AND u_username = @u_username";
            
            var con = ConOpen();
            var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@c_id", cardId);
            cmd.Parameters.AddWithValue("@c_indeck", false);
            cmd.Parameters.AddWithValue("@u_username", username);
            cmd.Prepare();
            
            var rdr = cmd.ExecuteReader();

            if (!rdr.HasRows)
            {
                rdr.Close();
                con.Close();
                return CreateTradingDealStatus.CardInDeckOrNotOwn;
            }

            rdr.Close();
            con.Close();
            
            const string sql2 = "INSERT INTO \"TradeTable\"(u_username, tr_id, c_id, tr_searchtype, tr_minimumdamage)" +
                               "VALUES (@u_username, @tr_id, @c_id, @tr_searchtype, @tr_minimumdamage)";
            
            con = ConOpen();
            var cmd2 = new NpgsqlCommand(sql2, con);
            
            cmd2.Parameters.AddWithValue("@u_username", username);
            cmd2.Parameters.AddWithValue("@tr_id", tradeId);
            cmd2.Parameters.AddWithValue("@c_id", cardId);
            cmd2.Parameters.AddWithValue("@tr_searchtype", searchType);
            cmd2.Parameters.AddWithValue("@tr_minimumdamage", minimumDamage);
            cmd2.Prepare();
            cmd2.ExecuteNonQuery();
            con.Close();
            
            return CreateTradingDealStatus.Success;
        }
        public List<TradeTable> GetTradingDeals()
        {
            const string sql = "SELECT * FROM \"TradeTable\"";
            
            var con = ConOpen();
            var cmd = new NpgsqlCommand(sql, con);
            cmd.Prepare();
            
            var rdr = cmd.ExecuteReader();
            
            List<TradeTable> tradingDeals = new List<TradeTable>();
            
            while(rdr.Read())
            {
                var tradeId = rdr.GetString(0);
                var cardId = rdr.GetString(1);
                var searchType = rdr.GetString(2);
                var minimumDamage = rdr.GetDouble(3);
                var username = rdr.GetString(4);
                
                tradingDeals.Add(new TradeTable(username, tradeId, cardId, searchType, minimumDamage));
            }
            
            rdr.Close();
            con.Close();
            return tradingDeals;
        }
        public DeleteTradingDealStatus DeleteTradingDeal(string tradeId, string username)
        {
            const string sql = "DELETE FROM \"TradeTable\" WHERE tr_id = @tr_id AND u_username = @u_username";
            
            var con = ConOpen();
            var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@tr_id", tradeId);
            cmd.Parameters.AddWithValue("@u_username", username);
            cmd.Prepare();
            
            int result = cmd.ExecuteNonQuery();
            con.Close();
            
            if (result == -1)
                return DeleteTradingDealStatus.FromOtherUser;

            return DeleteTradingDealStatus.Success;
        }
        
        public ProcessTradingDealStatus ProcessTradingDeal(string tradeId, string offeredCardId, string username)
        {
            const string sql = "SELECT c_id, tr_searchtype, tr_minimumdamage, u_username FROM \"TradeTable\" WHERE tr_id = @tr_id";
            
            var con = ConOpen();
            var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@tr_id", tradeId);
            cmd.Prepare();
            var rdr = cmd.ExecuteReader();

            if (!rdr.HasRows)
            {
                rdr.Close();
                con.Close();
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
            con.Close();

            if (username == tradeFromUser)
                return ProcessTradingDealStatus.SameUser;

            const string sql2 = "SELECT c_name, c_damage FROM \"UserCardTable\" WHERE c_id = @c_id AND c_indeck = @c_indeck";
            
            con = ConOpen();
            var cmd2 = new NpgsqlCommand(sql2, con);
            cmd2.Parameters.AddWithValue("@c_id", offeredCardId);
            cmd2.Parameters.AddWithValue("@c_indeck", false);
            cmd2.Prepare();
            var rdr2 = cmd2.ExecuteReader();
            
            if (!rdr2.HasRows)
            {
                rdr2.Close();
                con.Close();
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
            con.Close();

            string offeredCardType = "Spell";
            if (!offeredCardName.Contains("Spell", StringComparison.CurrentCultureIgnoreCase))
                offeredCardType = "Monster";

            if (offeredCardDamage < wantedMinimumDamage || 
                !string.Equals(offeredCardType, wantedSearchType, StringComparison.CurrentCultureIgnoreCase)
                )
                return ProcessTradingDealStatus.NotWanted;

            UpdateCardUser(username, wantedCardId);
            UpdateCardUser(tradeFromUser, offeredCardId);
            DeleteTradingDeal(tradeId, tradeFromUser);
            
            return ProcessTradingDealStatus.Success;
        }

        private void UpdateCardUser(string username, string cardId)
        {
            const string sql = "UPDATE \"UserCardTable\" SET u_username = @u_username WHERE c_id = @c_id";
            var con = ConOpen();
            var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@u_username", username);
            cmd.Parameters.AddWithValue("@c_id", cardId);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
            con.Close();
        }
        
        private int GetUserCoins(string username)
        {
            const string sql = "SELECT u_coins FROM \"UserTable\" WHERE u_username = @u_username";
            var con = ConOpen();
            var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@u_username", username);
            cmd.Prepare();
            var rdr = cmd.ExecuteReader();
            
            int coins = 0;
            if (rdr.HasRows)
            {
                rdr.Read();
                coins = rdr.GetInt32(0);
            }
            
            rdr.Close();
            con.Close();
            
            return coins;
        }

        private bool CheckUserExist(string username)
        {
            const string sql = "SELECT u_username FROM \"UserTable\" WHERE u_username = @u_username";
            
            var con = ConOpen();
            var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@u_username", username);
            cmd.Prepare();
            var rdr = cmd.ExecuteReader();
            var result = rdr.Read();
            
            rdr.Close();
            con.Close();
             
            return result;
        }
        
        private bool CheckCardExist(string cardId)
        {
            const string sql = "SELECT p_cid FROM \"PackageTable\" WHERE p_cid = @p_cid";
            
            var con = ConOpen();
            var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@p_cid", cardId);
            cmd.Prepare();
            var rdr = cmd.ExecuteReader();
            var result = rdr.Read();
            
            rdr.Close();
            con.Close();
            
            return result;
        }

        public bool CheckPackageExist(string packId)
        {
            const string sql = "SELECT p_id FROM \"PackageTable\" WHERE p_id = @p_id";
            
            var con = ConOpen();
            var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@p_id", packId);
            cmd.Prepare();
            var rdr = cmd.ExecuteReader();
            var result = rdr.Read();
            
            rdr.Close();
            con.Close();
            
            return result;
        }

        private bool CheckTokenIdExist(int uid)
        {
            const string sql = "SELECT t_id FROM \"TokenTable\" WHERE u_id = @u_id";
            
            var con = ConOpen();
            var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@u_id", uid);
            cmd.Prepare();

            var rdr = cmd.ExecuteReader();
            var result = rdr.Read();
            
            rdr.Close();
            con.Close();
            
            return result;
        }
        
        public string GetUsernameFromDatabase(string authToken)
        {
            if (string.IsNullOrEmpty(authToken))
                return null;

            string sql = "SELECT \"UserTable\".u_username " + 
                         "FROM \"UserTable\" " +
                         "INNER JOIN \"TokenTable\" " +
                         "ON \"UserTable\".u_id = \"TokenTable\".u_id " +
                         "WHERE \"TokenTable\".t_token = @t_token";
            
            var con = ConOpen();
            var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@t_token", authToken);
            cmd.Prepare();

            var rdr = cmd.ExecuteReader();

            string username = "";
            if (rdr.HasRows)
            {
                rdr.Read();
                username = rdr.GetString(0);
            }
            
            rdr.Close();
            con.Close();
            
            return username;
        }

        private void UpdateStatsWin(string winner)
        {
            string sql = "UPDATE \"StatsTable\" SET s_elo = s_elo + @plus, s_wins = s_wins + @incr WHERE u_username = @u_username";
            
            var con = ConOpen();
            var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@plus", 3);
            cmd.Parameters.AddWithValue("@incr", 1);
            cmd.Parameters.AddWithValue("@u_username", winner);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
            con.Close();

            CoinsRewards(winner);
        }
        
        private void UpdateStatsLoss(string loser)
        {
            string sql = "UPDATE \"StatsTable\" SET s_elo = s_elo - @minus, s_losses = s_losses + @incr WHERE u_username = @u_username";
            
            var con = ConOpen();
            var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@minus", 5);
            cmd.Parameters.AddWithValue("@incr", 1);
            cmd.Parameters.AddWithValue("@u_username", loser);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
            con.Close();
            
            CoinsRewards(loser);
        }
        
        private void UpdateStatsDraw(string player)
        {
            string sql = "UPDATE \"StatsTable\" SET s_draws = s_draws + @incr WHERE u_username = @u_username";
            
            var con = ConOpen();
            var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@incr", 1);
            cmd.Parameters.AddWithValue("@u_username", player);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
            con.Close();
            
            CoinsRewards(player);
        }

        private void CoinsRewards(string player)
        {
            const string sql = "SELECT \"StatsTable\".s_wins, \"StatsTable\".s_losses, \"StatsTable\".s_draws " +
                               "FROM \"StatsTable\" " +
                               "INNER JOIN \"UserTable\" " +
                               "ON \"StatsTable\".u_username = \"UserTable\".u_username " +
                               "WHERE \"UserTable\".u_username = @u_username";
            
            var con = ConOpen();
            var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@u_username", player);
            cmd.Prepare();
            
            var rdr = cmd.ExecuteReader();
            rdr.Read();
            
            int gamesPlayed = rdr.GetInt32(0) + rdr.GetInt32(1) + rdr.GetInt32(2);
            
            rdr.Close();
            con.Close();
            
            if (gamesPlayed % 10 == 0 && gamesPlayed > 0)
            {
                const string sql2 = "UPDATE \"UserTable\" SET u_coins = u_coins + @reward WHERE u_username = @u_username";
            
                var con2 = ConOpen();
                var cmd2 = new NpgsqlCommand(sql2, con2);
                cmd2.Parameters.AddWithValue("@reward", 2);
                cmd2.Parameters.AddWithValue("@u_username", player);
                cmd2.Prepare();
                cmd2.ExecuteNonQuery();
                con2.Close();
            }
        }
    }
}
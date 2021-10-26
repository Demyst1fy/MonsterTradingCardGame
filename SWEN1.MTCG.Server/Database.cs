using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;

namespace SWEN1.MTCG.ClassLibrary
{
    public class Database
    {
        private readonly string host = "localhost";
        private readonly int port = 5432;
        private readonly string dbusername = "postgres";
        private readonly string dbpassword = "postgres";
        private readonly string dbname = "mtcg";
        
        public static NpgsqlConnection con;
        
        public Database()
        {
            con = new NpgsqlConnection($"Host={host};" +
                                       $"Port={Convert.ToString(port)};" +
                                       $"Username={dbusername};" +
                                       $"Password={dbpassword};" +
                                       $"Database={dbname}");
        }

        public bool RegisterUser(string username, string password)
        {
            if (CheckUserAlreadyExist(username))
            {
                return false;
            }
            
            string fullname = char.ToUpper(username[0]) + username.Substring(1);
            
            con.Open();
            const string sql = "INSERT INTO usertable(u_username, u_password, u_fullname)" +
                               "VALUES (:u_username, :u_password, :u_fullname)";
            
            var cmd = new NpgsqlCommand(sql, con);
            
            cmd.Parameters.AddWithValue(":u_username", username);
            cmd.Parameters.AddWithValue(":u_password", password);
            cmd.Parameters.AddWithValue(":u_fullname", fullname);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
            con.Close();
            
            con.Open();
            const string sql2 = "INSERT INTO statstable(u_username)" +
                               "VALUES (:u_username)";
            
            var cmd2 = new NpgsqlCommand(sql2, con);
            
            cmd2.Parameters.AddWithValue(":u_username", username);
            cmd2.Prepare();
            cmd2.ExecuteNonQuery();
            con.Close();
            
            return true;
        }
        
        public bool LoginUser(string username, string password)
        {
            con.Open();
            const string sql = "SELECT u_id FROM usertable WHERE u_username = :u_username AND u_password = :u_password";
            var cmd = new NpgsqlCommand(sql, con);
            
            cmd.Parameters.AddWithValue(":u_username", username);
            cmd.Parameters.AddWithValue(":u_password", password);
            cmd.Prepare();

            var rdr = cmd.ExecuteReader();

            bool valid = false;
            if (rdr.HasRows)
            {
                rdr.Read();
                int userId = rdr.GetInt32(0);
                rdr.Close();
                con.Close();

                string sql2;
                if (!CheckTokenIDAlreadyExist(userId))
                {
                    sql2 = "INSERT INTO tokentable (u_id, t_token) VALUES(:u_id, :t_token)";
                }
                else
                {
                    sql2 = "UPDATE tokentable SET t_token = :t_token WHERE u_id = :u_id";
                }
                con.Open();
                var cmd2 = new NpgsqlCommand(sql2, con);
                cmd2.Parameters.AddWithValue(":u_id", userId);
                cmd2.Parameters.AddWithValue(":t_token","Basic " + username + "-mtcgToken");
                
                cmd2.Prepare();
                cmd2.ExecuteNonQuery();
                valid = true;
            }
            con.Close();
            return valid;
        }
        
        public UserData GetUserData(string username)
        {
            con.Open();
            const string sql = "SELECT u_coins, u_fullname, u_bio, u_image FROM usertable WHERE u_username = :u_username";
            var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue(":u_username", username);

            var rdr = cmd.ExecuteReader();
            
            UserData userData = null;
            
            while(rdr.Read())
            {
                var coins = rdr.GetInt32(0);
                var fullname = rdr.GetString(1);
                var bio = rdr.GetString(2);
                var image = rdr.GetString(3);

                userData = new UserData(username, coins, fullname, bio, image);
            }
            
            con.Close();
            return userData;
        }
        
        public void EditUserData(string username, string fullname, string bio, string image)
        {
            con.Open();
            const string sql = "UPDATE usertable SET u_fullname = :u_fullname, u_bio = :u_bio, u_image = :u_image WHERE u_username = :u_username";
            var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue(":u_fullname", fullname);
            cmd.Parameters.AddWithValue(":u_bio", bio);
            cmd.Parameters.AddWithValue(":u_image", image);
            cmd.Parameters.AddWithValue(":u_username", username);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void GetAllData()
        {
            con.Open();
            const string sql = "SELECT * FROM Usertable";
            var cmd = new NpgsqlCommand(sql, con);
            
            var rdr = cmd.ExecuteReader();

            if (rdr.HasRows)
            {
                int count = rdr.FieldCount;
                while(rdr.Read()) {
                    for(int i = 0 ; i < count ; i++) {
                        Console.WriteLine(rdr.GetValue(i));
                    }
                }
                rdr.Close();
                con.Close();
            }
        }
        
        public void InsertNewCardStack(int uid, string cid, string _cardName, int _dmg)
        {
            con.Open();
            const string sql = "INSERT INTO cardtable(u_id, c_id, c_name, c_damage, c_indeck)" +
                               "VALUES (:u_id, :c_id, :c_name, :c_damage, :c_indeck)";
            var cmd = new NpgsqlCommand(sql, con);
            
            cmd.Parameters.AddWithValue(":u_id", uid);
            cmd.Parameters.AddWithValue(":c_id", cid);
            cmd.Parameters.AddWithValue(":c_name", _cardName);
            cmd.Parameters.AddWithValue(":c_damage", _dmg);
            cmd.Parameters.AddWithValue(":c_indeck", false);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public bool ConfigureDeck(string[] chosenCardIDs, string username)
        {
            con.Open();
            const string sql = "SELECT c_id FROM cardtable WHERE u_username = :u_username";
            var cmd = new NpgsqlCommand(sql, con);
            
            cmd.Parameters.AddWithValue(":u_username", username);
            cmd.Prepare();
            
            var rdr = cmd.ExecuteReader();
            
            List<string> stackCardIDs = new List<string>();
            
            while(rdr.Read())
            {
                stackCardIDs.Add(rdr.GetString(0));
            }

            rdr.Close();
            con.Close();
            
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
                return false;
            }

            con.Open();
            const string sql2 = "UPDATE cardtable SET c_indeck = :c_indeck WHERE u_username = :u_username";
            var cmd2 = new NpgsqlCommand(sql2, con);
            
            cmd2.Parameters.AddWithValue(":c_indeck", false);
            cmd2.Parameters.AddWithValue(":u_username", username);
            cmd2.Prepare();
            cmd2.ExecuteNonQuery();
            con.Close();

            for (var i = 0; i < chosenCardIDs.Length; i++)
            {
                con.Open();
                const string sql3 = "UPDATE cardtable SET c_indeck = :c_indeck WHERE u_username = :u_username AND c_id = :c_id";
                var cmd3 = new NpgsqlCommand(sql3, con);
            
                cmd3.Parameters.AddWithValue(":c_indeck", true);
                cmd3.Parameters.AddWithValue(":u_username", username);
                cmd3.Parameters.AddWithValue(":c_id", chosenCardIDs[i]);
                cmd3.Prepare();
                cmd3.ExecuteNonQuery();
                con.Close();
            }

            return true;
        }
        
        public List<ICard> GetUserStack(string username)
        {
            con.Open();
            const string sql = "SELECT c_id, c_name, c_damage FROM cardtable WHERE u_username = :u_username";
            var cmd = new NpgsqlCommand(sql, con);
            
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
            
            con.Close();
            return stack;
        }
        public List<ICard> GetUserDeck(string username)
        {
            con.Open();
            const string sql = "SELECT c_id, c_name, c_damage FROM cardtable WHERE u_username = :u_username AND c_indeck = :c_indeck";
            var cmd = new NpgsqlCommand(sql, con);
            
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
            
            con.Close();
            return deck;
        }

        public Stats GetUserStats(string username)
        {
            con.Open();
            const string sql = "SELECT s_wins, s_losses, s_draws, s_elo FROM statstable WHERE u_username = :u_username";
            var cmd = new NpgsqlCommand(sql, con);
            
            cmd.Parameters.AddWithValue(":u_username", username);
            cmd.Prepare();
            
            var rdr = cmd.ExecuteReader();
            
            Stats stats = null;
            
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

                stats = new Stats(username, wins, losses, draws, elo, winRate);
            }
            
            con.Close();
            return stats;
        }
        
        public List<Stats> GetScoreBoard()
        {
            con.Open();
            const string sql = "SELECT * FROM statstable";
            var cmd = new NpgsqlCommand(sql, con);
            cmd.Prepare();
            
            var rdr = cmd.ExecuteReader();
            
            List<Stats> scoreBoard = new List<Stats>();
            
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

                scoreBoard.Add(new Stats(user, wins, losses, draws, elo, winRate));
            }
            
            con.Close();
            return scoreBoard;
        }

        public bool CheckUserAlreadyExist(string _username)
        {
            con.Open();
            const string sql = "SELECT u_username FROM usertable WHERE u_username = :u_username";
            var cmd = new NpgsqlCommand(sql, con);

            cmd.Parameters.AddWithValue(":u_username", _username);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
            
            var rdr = cmd.ExecuteReader();
            
            var result = rdr.Read();
            
            con.Close();
            
            return result;
        }
        
        public bool CheckTokenIDAlreadyExist(int _uid)
        {
            con.Open();
            const string sql = "SELECT t_id FROM tokentable WHERE u_id = :u_id";
            var cmd = new NpgsqlCommand(sql, con);

            cmd.Parameters.AddWithValue(":u_id", _uid);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
            
            var rdr = cmd.ExecuteReader();
            
            var result = rdr.Read();
            
            con.Close();
            
            return result;
        }
        
        public string GetUsernameFromAuthKey(string authToken)
        {
            if (string.IsNullOrEmpty(authToken))
            {
                return null;
            }
            
            con.Open();
            string sql = "SELECT usertable.u_username FROM usertable INNER JOIN tokentable ON usertable.u_id = tokentable.u_id WHERE tokentable.t_token = :t_token";
            var cmd = new NpgsqlCommand(sql, con);
            
            cmd.Parameters.AddWithValue(":t_token", authToken);
            cmd.Prepare();
            cmd.ExecuteNonQuery();

            var rdr = cmd.ExecuteReader();

            string username = "";
            if (rdr.HasRows)
            {
                rdr.Read();
                username = rdr.GetString(0);
            }
            
            con.Close();
            
            return username;
        }
    }
}
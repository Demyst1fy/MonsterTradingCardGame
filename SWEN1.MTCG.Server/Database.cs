using System;
using System.Collections.Generic;
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

        public bool RegisterUser(string _username, string _password)
        {
            if (CheckUserAlreadyExist(_username))
            {
                Console.WriteLine("User already exists!");
                return false;
            }
            
            con.Open();
            const string sql = "INSERT INTO Usertable(u_username, u_password)" +
                               "VALUES (:u_username, :u_password)";
            
            var cmd = new NpgsqlCommand(sql, con);
            
            cmd.Parameters.AddWithValue(":u_username", _username);
            cmd.Parameters.AddWithValue(":u_password", _password);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
            con.Close();
            
            return true;
        }
        public bool LoginUser(string _username, string _password)
        {
            bool valid = false;
            con.Open();
            const string sql = "SELECT u_id, u_username, u_coins FROM Usertable WHERE u_username = :u_username AND u_password = :u_password";
            var cmd = new NpgsqlCommand(sql, con);
            
            cmd.Parameters.AddWithValue(":u_username", _username);
            cmd.Parameters.AddWithValue(":u_password", _password);
            cmd.Prepare();

            var rdr = cmd.ExecuteReader();

            if (rdr.HasRows)
            {
                valid = true;
            }
            con.Close();
            return valid;
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
        
        public List<ICard> GetDeck(int uid)
        {
            List<ICard> deck = new List<ICard>();
            
            con.Open();
            const string sql = "SELECT c_id, c_name, c_damage FROM Cardtable WHERE u_id = :u_id AND c_indeck = :c_indeck";
            var cmd = new NpgsqlCommand(sql, con);
            
            cmd.Parameters.AddWithValue(":u_id", uid);
            cmd.Parameters.AddWithValue(":c_indeck", true);
            cmd.Prepare();
            
            var rdr = cmd.ExecuteReader();
            
            while(rdr.Read())
            {
                var id = rdr.GetString(0);
                var name = rdr.GetString(1);
                var damage = rdr.GetInt32(2);
                
                deck.Add(new Card(id, name, damage));
            }
            
            con.Close();
            return deck;
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
    }
}
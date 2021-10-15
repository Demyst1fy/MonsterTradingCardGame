using System;
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

        public bool RegisterUser(string[] userData)
        {
            var username = userData[0];
            var password1 = userData[1];
            var password2 = userData[2];

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password1) || string.IsNullOrEmpty(password2))
            {
                Console.WriteLine("Fields must not be empty!");
                return false;
            } if (CheckUserAlreadyExist(username))
            {
                Console.WriteLine("User already exists!");
                return false;
            } if (password1 != password2)
            {
                Console.WriteLine("Passwort confirmation does not match!");
                return false;
            }
            
            con.Open();
            const string sql = "INSERT INTO Usertable(u_username, u_password)" +
                               "VALUES (:u_username, :u_password)";
            
            var cmd = new NpgsqlCommand(sql, con);
            
            cmd.Parameters.AddWithValue(":u_username", username);
            cmd.Parameters.AddWithValue(":u_password", password1);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
            con.Close();
            
            Console.WriteLine("You are now registered!");
            return true;
        }
        public void LoginUser(string _username, string _password)
        {
            con.Open();
            const string sql = "SELECT u_id, u_username, u_coins FROM Usertable WHERE u_username = :u_username AND u_password = :u_password";
            var cmd = new NpgsqlCommand(sql, con);
            
            cmd.Parameters.AddWithValue(":u_username", _username);
            cmd.Parameters.AddWithValue(":u_password", _password);
            cmd.Prepare();

            var rdr = cmd.ExecuteReader();

            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    Console.WriteLine("{0} {1} {2}", rdr.GetInt32(0),
                        rdr.GetString(1), rdr.GetInt32(2) );
                }
                con.Close();
            }
            Console.WriteLine("No rows found.");
            con.Close();
        }
        
        public void InsertNewCardStack(int id, string _id, string _cardName, int _dmg)
        {
            con.Open();
            const string sql = "INSERT INTO cardtable(u_id, c_id, c_name, c_damage, c_indeck)" +
                               "VALUES (:u_id, :c_id, :c_name, :c_damage, :c_indeck)";
            var cmd = new NpgsqlCommand(sql, con);
            
            cmd.Parameters.AddWithValue(":u_id", id);
            cmd.Parameters.AddWithValue(":c_id", _id);
            cmd.Parameters.AddWithValue(":c_name", _cardName);
            cmd.Parameters.AddWithValue(":c_damage", _dmg);
            cmd.Parameters.AddWithValue(":c_indeck", false);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
            con.Close();
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
        
        public void Reset()
        {
            con.Close();
        }
    }
}
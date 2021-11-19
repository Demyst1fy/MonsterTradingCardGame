namespace SWEN1.MTCG.Server.JSONClasses
{
    public class UserJSON
    {
        public string Username { get; }
        public string Password { get; }
        
        public UserJSON(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}
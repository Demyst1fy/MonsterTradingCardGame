namespace SWEN1.MTCG.Server.JsonClasses
{
    public class UserJson
    {
        public string Username { get; }
        public string Password { get; }
        
        public UserJson(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}
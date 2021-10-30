namespace SWEN1.MTCG.Server.DatabaseClasses
{
    public class Usertable
    {
        public string Username { get; }
        public int Coins { get; }
        public string Fullname { get; }
        public string Bio { get; }
        public string Image { get; }
        
        public Usertable(string username, int coins, string fullname, string bio, string image)
        {
            Username = username;
            Coins = coins;
            Fullname = fullname;
            Bio = bio;
            Image = image;
        }
    }
}
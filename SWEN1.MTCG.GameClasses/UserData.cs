namespace SWEN1.MTCG.ClassLibrary
{
    public class UserData
    {
        public string Username { get; }
        public int Coins { get; }
        public string Fullname { get; }
        public string Bio { get; }
        public string Image { get; }
        
        public UserData(string username, int coins, string fullname, string bio, string image)
        {
            Username = username;
            Coins = coins;
            Fullname = fullname;
            Bio = bio;
            Image = image;
        }
    }
}
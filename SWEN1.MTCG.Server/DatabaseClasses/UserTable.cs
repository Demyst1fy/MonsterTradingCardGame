namespace SWEN1.MTCG.Server.DatabaseClasses
{
    public class UserTable
    {
        public string Username { get; }
        public int Coins { get; }
        public int SpentCoins { get; }
        public string Fullname { get; }
        public string Bio { get; }
        public string Image { get; }
        
        public UserTable(string username, int coins, int spentCoins, string fullname, string bio, string image)
        {
            Username = username;
            Coins = coins;
            SpentCoins = spentCoins;
            Fullname = fullname;
            Bio = bio;
            Image = image;
        }
    }
}
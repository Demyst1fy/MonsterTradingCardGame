using System.Collections.Generic;

namespace SWEN1.MTCG.ClassLibrary
{
    public class User : IUser
    {
        public string Username { get; }
        public string Password { get; }
        public string AuthToken { get; private set; }
        public int Coins { get; }
        public List<Card> StackCollection { get; }
        public List<Card> DeckCollection { get; private set; }
        public int Wins { get; private set; }
        public int Losses { get; private set; }

        public User(string username, string password, int coins, List<Card> deckCollection)
        {
            Username = username;
            Password = password;
            Coins = coins;
            DeckCollection = deckCollection;
        }

        public void increWins() { Wins++; }
        public void increLosses() { Losses++; }
    }
}
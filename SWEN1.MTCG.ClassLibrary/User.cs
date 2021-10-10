using System.Collections.Generic;

namespace SWEN1.MTCG.ClassLibrary
{
    public class User : IUser
    {
        public string Username { get; }
        public string Password { get; }
        public int Coins { get; }
        public List<Card> StackCollection { get; }
        public List<Card> DeckCollection { get; private set;  }

        public User(string username, string password, int coins, List<Card> stackCollection)
        {
            Username = username;
            Password = password;
            Coins = coins;
            StackCollection = stackCollection;
        }

        public void ChooseDeckCards(List<Card> deckCollection)
        {
            DeckCollection = deckCollection;
        }
    }
}
using System.Collections.Generic;

namespace SWEN1.MTCG.ClassLibrary
{
    public interface IUser
    {
        
    }
    public class User : IUser
    {
        public string Username { get; private set; }
        public string Password { get; private set; }
        public int Coins { get; private set; }
        
        public List<Card> StackCollection { get; private set; }
        public List<Card> DeckCollection { get; private set; }

        public User(string username, string password, int coins)
        {
            Username = username;
            Password = password;
            Coins = coins;
        }

        public void ChooseDeckCards(List<Card> deckCollection)
        {
            DeckCollection = deckCollection;
        }
    }
}
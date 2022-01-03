using System.Collections.Generic;
using SWEN1.MTCG.Game.Interfaces;

namespace SWEN1.MTCG.Game
{
    public class User : IUser
    {
        public string Username { get; }
        public List<ICard> Deck { get; }
        public Stats Stats { get; }
        
        public User(string username, List<ICard> deck, Stats stats)
        {
            Username = username;
            Deck = deck;
            Stats = stats;
        }
    }
}
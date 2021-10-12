using System;
using System.Collections.Generic;

namespace SWEN1.MTCG.ClassLibrary
{
    public class User : IUser
    {
        public string Username { get; }
        public string Password { get; }
        public string AuthToken { get; private set; }
        public int Coins { get; }
        public List<ICard> StackCollection { get; set; }
        public List<ICard> DeckCollection { get; set; }
        public int Wins { get; private set; }
        public int Losses { get; private set; }
        public int Draws { get; private set; }

        public User(IUser previousPerson)
        {
            Username = previousPerson.Username;
            Password = previousPerson.Username;
            Coins = previousPerson.Coins;
            DeckCollection = new List<ICard>(previousPerson.DeckCollection);
        }
        public User(string username, string password, int coins)
        {
            Username = username;
            Password = password;
            Coins = coins;
            DeckCollection = new List<ICard>();
        }

        public void IncreWins() { Wins++; }
        public void IncreLosses() { Losses++; }
        public void IncreDraws() { Draws++; }

        public void OutPutWinRate()
        {
            if (Wins == 0 && Losses == 0 && Draws == 0)
            {
                return;
            }
            Console.WriteLine($"Winrate: {Math.Round((Wins/(double)(Wins+Losses+Draws))*100, 2)}% ({Wins}W/{Losses}L/{Draws}D)");
        }
    }
}
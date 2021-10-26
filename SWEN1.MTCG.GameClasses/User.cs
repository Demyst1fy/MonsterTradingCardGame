using System;
using System.Collections.Generic;

namespace SWEN1.MTCG.ClassLibrary
{
    public class User : IUser
    {
        public int ID { get; }
        public string Username { get; }
        public List<ICard> Deck { get; set; }
        public int Wins { get; private set; }
        public int Losses { get; private set; }
        public int Draws { get; private set; }
        public int Elo { get; private set; }

        public User(IUser previousPerson)
        {
            ID = previousPerson.ID;
            Username = previousPerson.Username;
            Deck = new List<ICard>(previousPerson.Deck);
        }
        public User(int id, string username)
        {
            ID = id;
            Username = username;
            Deck = new List<ICard>();
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
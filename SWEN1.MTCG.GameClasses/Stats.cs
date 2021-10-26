﻿namespace SWEN1.MTCG.ClassLibrary
{
    public class Stats
    {
        public string User { get; }
        public int Wins { get; }
        public int Losses { get; }
        public int Draws { get; }
        public int Elo { get; }
        
        public double WinRate { get; }

        public Stats(string user, int wins, int losses, int draws, int elo, double winRate)
        {
            User = user;
            Wins = wins;
            Losses = losses;
            Draws = draws;
            Elo = elo;
            WinRate = winRate;
        }
    }
}
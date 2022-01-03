using System;

namespace SWEN1.MTCG.Game
{
    public class Stats
    {
        public int Wins { get; }
        public int Losses { get; }
        public int Draws { get; }
        
        public int Elo { get; }

        public double WinRate { get; }

        public Stats(int wins, int losses, int draws, int elo)
        {
            Wins = wins;
            Losses = losses;
            Draws = draws;
            Elo = elo;

            if (wins > 0 || losses > 0 || draws > 0)
                WinRate = Math.Round((wins / (double) (wins + losses + draws)) * 100, 2);
            else
                WinRate = 0;
        }
    }
}
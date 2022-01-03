using System;
using System.Text;
using SWEN1.MTCG.Game.Interfaces;

namespace SWEN1.MTCG.Game
{
    public class Logging : ILogging
    {
        public IUser Player1 { get; }
        public IUser Player2 { get; }
        public StringBuilder Log { get; }

        public Logging(IUser player1, IUser player2)
        {
            Player1 = player1;
            Player2 = player2;

            Log = new StringBuilder();
            
            double avgElo = (double) (Player1.Stats.Elo + Player2.Stats.Elo)/2;
            AppendLogWithLine($"{Player1.Username} vs {Player2.Username} ({avgElo})");
        }

        public void AppendLogWithLine(string text)
        {
            Log.Append($"{text}{Environment.NewLine}");
            Console.WriteLine(text);
        }

        public string LogString()
        {
            return Log.ToString();
        }
    }
}
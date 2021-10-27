using System;

namespace SWEN1.MTCG.ClassLibrary
{
    public class Trade
    {
        public string Username { get; }
        public string Id { get; }
        public string CardToTrade { get; }
        public string Type { get; }
        public double MinimumDamage { get; }

        public Trade(string username, string id, string cardToTrade, string type, double minimumDamage)
        {
            Username = username;
            Id = id;
            CardToTrade = cardToTrade;
            Type = type;
            MinimumDamage = minimumDamage;
        }
    }
}
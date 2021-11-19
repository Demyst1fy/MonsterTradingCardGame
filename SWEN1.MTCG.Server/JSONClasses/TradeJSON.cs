namespace SWEN1.MTCG.Server.JSONClasses
{
    public class TradeJSON
    {
        public string Id { get; }
        public string CardToTrade { get; }
        public string Type { get; }
        public double MinimumDamage { get; }
        
        public TradeJSON(string id, string cardToTrade, string type, double minimumDamage)
        {
            Id = id;
            CardToTrade = cardToTrade;
            Type = type;
            MinimumDamage = minimumDamage;
        }
    }
}
namespace SWEN1.MTCG.Server.JsonClasses
{
    public class TradeJson
    {
        public string Id { get; }
        public string CardToTrade { get; }
        public string Type { get; }
        public double MinimumDamage { get; }
        
        public TradeJson(string id, string cardToTrade, string type, double minimumDamage)
        {
            Id = id;
            CardToTrade = cardToTrade;
            Type = type;
            MinimumDamage = minimumDamage;
        }
    }
}
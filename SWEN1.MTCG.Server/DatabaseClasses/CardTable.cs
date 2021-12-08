namespace SWEN1.MTCG.Server.DatabaseClasses
{
    public class CardTable
    {
        public string Id { get; }
        public string Name { get; }
        public double Damage { get; }
        
        public CardTable(string id, string name, double damage)
        {
            Id = id;
            Name = name;
            Damage = damage;
        }
    }
}
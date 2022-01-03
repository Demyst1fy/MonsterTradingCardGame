namespace SWEN1.MTCG.Server.JsonClasses
{
    public class PackageCardJson
    {
        public string Id { get; }
        public string Name { get; }
        public double Damage { get; }
        
        public PackageCardJson(string id, string name, double damage)
        {
            Id = id;
            Name = name;
            Damage = damage;
        }
    }
}
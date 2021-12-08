namespace SWEN1.MTCG.Server.JSONClasses
{
    public class PackageCardJSON
    {
        public string Id { get; }
        public string Name { get; }
        public double Damage { get; }
        
        public PackageCardJSON(string id, string name, double damage)
        {
            Id = id;
            Name = name;
            Damage = damage;
        }
    }
}
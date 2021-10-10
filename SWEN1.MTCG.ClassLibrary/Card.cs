namespace SWEN1.MTCG.ClassLibrary
{
    public abstract class Card : ICard
    {
        public string Id { get; }
        public string Name { get; }
        public double Damage { get; }
        public Element Element { get; }

        protected Card(string id, string name, double damage)
        {
            Id = id;
            Name = name;
            Damage = damage;

            if (name.Contains("Fire")) { Element = Element.Fire; }
            else if (name.Contains("Water")) { Element = Element.Water; }
            else { Element = Element.Normal; }
            
        }
    }
}
namespace SWEN1.MTCG.ClassLibrary
{
    public interface ICard
    {
        
    }

    public abstract class Card : ICard
    {
        public string Name { get; private set; }
        public double Damage { get; private set; }
        public Element Element { get; private set; }

        protected Card(string name, double damage)
        {
            Name = name;
            Damage = damage;

            if (name.Contains("Fire")) { Element = Element.Fire; }
            else if (name.Contains("Water")) { Element = Element.Water; }
            else { Element = Element.Normal; }
        }
    }
}
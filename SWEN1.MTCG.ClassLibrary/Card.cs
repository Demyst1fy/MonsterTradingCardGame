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

        protected Card(string name, double damage, Element element)
        {
            Name = name;
            Damage = damage;
            Element = element;
        }
    }
}
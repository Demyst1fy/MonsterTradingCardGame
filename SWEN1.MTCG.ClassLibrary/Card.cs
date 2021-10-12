using System;

namespace SWEN1.MTCG.ClassLibrary
{
    public class Card : ICard
    {
        public string Id { get; }
        public string Name { get; }
        public double Damage { get; }
        public Element Element { get; }
        public Type Type { get; }
        public Card(string id, string name, double damage)
        {
            Id = id;
            Name = name;
            Damage = damage;

            if (name.Contains("Fire")) { Element = Element.Fire; }
            else if (name.Contains("Water")) { Element = Element.Water; }
            else { Element = Element.Normal; }

            if (name.Contains("Goblin")) { Type = Type.Goblin; }
            else if (name.Contains("Dragon")) { Type = Type.Dragon; }
            else if (name.Contains("Wizard")) { Type = Type.Wizard; }
            else if (name.Contains("Ork")) { Type = Type.Ork; }
            else if (name.Contains("Knight")) { Type = Type.Knight; }
            else if (name.Contains("Kraken")) { Type = Type.Kraken; }
            else if (name.Contains("Elf")) { Type = Type.Elf; }
            else { Type = Type.Spell; }
        }
        
        public double CompareElement(Element enemyElement)
        {
            double damageAdj;

            switch (Element)
            {
                case Element.Water when enemyElement == Element.Fire:
                case Element.Fire when enemyElement == Element.Normal:
                case Element.Normal when enemyElement == Element.Water:
                {
                    damageAdj = Damage * 2;
                    break;
                }
                case Element.Fire when enemyElement == Element.Water:
                case Element.Water when enemyElement == Element.Normal:
                case Element.Normal when enemyElement == Element.Fire:
                {
                    damageAdj = Damage * 0.5;
                    break;
                }
                default: 
                    damageAdj = Damage * 0;
                    break;
            }

            return damageAdj;
        }

        public bool CompareCard(ICard enemyCard)
        {
            if(Type != Type.Spell && enemyCard.Type != Type.Spell) {
                if (Type == Type.Goblin && enemyCard.Type == Type.Dragon)
                {
                    Console.WriteLine($"{Name} is afraid of {enemyCard.Name}!");
                    return true;
                }
                if (Type == Type.Ork && enemyCard.Type == Type.Wizard)
                {
                    Console.WriteLine($"{enemyCard.Name} is putting {Name} under control!");
                    return true;
                }
                if (Type == Type.Dragon && enemyCard.Type == Type.Elf && enemyCard.Element == Element.Fire)
                {
                    Console.WriteLine($"{enemyCard.Name} is able to evade {Name}'s attack!");
                    return true;
                }
            }
            else if (Type == Type.Knight && enemyCard.Type == Type.Spell && enemyCard.Element == Element.Water)
            {
                Console.WriteLine($"{Name} drowned in the {enemyCard.Name}!");
                return true;
            }
            else if (Type == Type.Spell && enemyCard.Type == Type.Kraken)
            {
                Console.WriteLine($"{enemyCard.Name} is immune against spells!");
                return true;
            }

            return false;
        }
    }
}
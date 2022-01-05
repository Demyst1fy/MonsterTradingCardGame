using System;
using SWEN1.MTCG.Game.Interfaces;

namespace SWEN1.MTCG.Game
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

            if (name.Contains("Fire", StringComparison.OrdinalIgnoreCase)) 
                Element = Element.Fire;
            else if (name.Contains("Water", StringComparison.OrdinalIgnoreCase)) 
                Element = Element.Water;
            else if (name.Contains("Light", StringComparison.OrdinalIgnoreCase)) 
                Element = Element.Light;
            else if (name.Contains("Dark", StringComparison.OrdinalIgnoreCase)) 
                Element = Element.Dark;
            else 
                Element = Element.Normal;

            if (name.Contains("Goblin", StringComparison.OrdinalIgnoreCase)) 
                Type = Type.Goblin;
            else if (name.Contains("Dragon", StringComparison.OrdinalIgnoreCase)) 
                Type = Type.Dragon;
            else if (name.Contains("Wizard", StringComparison.OrdinalIgnoreCase)) 
                Type = Type.Wizard;
            else if (name.Contains("Ork", StringComparison.OrdinalIgnoreCase)) 
                Type = Type.Ork;
            else if (name.Contains("Knight", StringComparison.OrdinalIgnoreCase)) 
                Type = Type.Knight;
            else if (name.Contains("Kraken", StringComparison.OrdinalIgnoreCase)) 
                Type = Type.Kraken;
            else if (name.Contains("Elf", StringComparison.OrdinalIgnoreCase)) 
                Type = Type.Elf;
            else if (name.Contains("Vampire", StringComparison.OrdinalIgnoreCase)) 
                Type = Type.Vampire;
            else if (name.Contains("Fairy", StringComparison.OrdinalIgnoreCase)) 
                Type = Type.Fairy;
            else 
                Type = Type.Spell;
        }
        
        public double CompareElement(Element enemyElement)
        {
            double damageAdj;

            switch (Element)
            {
                case Element.Water when enemyElement == Element.Fire:
                case Element.Fire when enemyElement == Element.Normal:
                case Element.Normal when enemyElement == Element.Water:
                case Element.Dark when enemyElement == Element.Light:
                case Element.Light when enemyElement == Element.Dark:
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
                case Element.Fire when enemyElement == Element.Fire:
                case Element.Water when enemyElement == Element.Water:
                case Element.Dark when enemyElement == Element.Dark:
                case Element.Light when enemyElement == Element.Light:
                case Element.Normal when enemyElement == Element.Normal:
                {
                    damageAdj = Damage * 0;
                    break;
                }
                default:
                {
                    damageAdj = Damage;
                    break;
                }
            }

            return damageAdj;
        }

        public string CheckEffect(ICard enemyCard)
        {
            string result;
            switch (Type)
            {
                case Type.Goblin when enemyCard.Type == Type.Dragon:
                    result = $"{Name} is afraid of {enemyCard.Name}!";
                    break;
                case Type.Ork when enemyCard.Type == Type.Wizard:
                    result = $"{enemyCard.Name} is putting {Name} under control!";
                    break;
                case Type.Dragon when enemyCard.Type == Type.Elf && enemyCard.Element == Element.Fire:
                    result = $"{enemyCard.Name} is able to evade {Name}'s attack!";
                    break;
                case Type.Knight when enemyCard.Type == Type.Spell && enemyCard.Element == Element.Water:
                    result = $"{Name} drowned in the {enemyCard.Name}!";
                    break;
                case Type.Spell when enemyCard.Type == Type.Kraken:
                    result = $"{enemyCard.Name} is immune against spells!";
                    break;
                case Type.Vampire when enemyCard.Type == Type.Fairy && enemyCard.Element == Element.Light:
                    result = $"{Name} is going blind due to the brightness of {enemyCard.Name}!";
                    break;
                default:
                    result = null;
                    break;
            }

            if(string.IsNullOrEmpty(result))
                Console.WriteLine(result);
            
            return result;
        }
    }
}
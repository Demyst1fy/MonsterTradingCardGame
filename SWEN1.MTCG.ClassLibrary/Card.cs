using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN1.MTCG.ClassLibrary
{
    public interface ICard
    {
        
    }

    public abstract class Card : ICard
    {
        public string name { get; private set; }
        public double damage { get; private set; }
        public Element element { get; private set; }

        protected Card(string name, double damage, Element element)
        {
            this.name = name;
            this.damage = damage;
            this.element = element;
        }
    }
}
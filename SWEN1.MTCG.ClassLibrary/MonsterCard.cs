using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN1.MTCG.ClassLibrary
{
    public class MonsterCard : Card
    {
        public Monster monsterType { get; private set; }
        public MonsterCard(string name, int damage, Element element, Monster monsterType) : base(name, damage, element)
        {
            this.monsterType = monsterType;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN1.MTCG.ClassLibrary
{
    public class SpellCard : Card
    {
        public SpellCard(string name_, int damage_, Element element_) : base(name_, damage_, element_) { }
    }
}
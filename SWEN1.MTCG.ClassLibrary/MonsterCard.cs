namespace SWEN1.MTCG.ClassLibrary
{
    public class MonsterCard : Card
    {
        public Monster MonsterType { get; }
        public MonsterCard(string id, string name, int damage) : base(id, name, damage) 
        {
            if(name.Contains("Goblin")) { MonsterType = Monster.Goblin; }
            else if(name.Contains("Dragon")) { MonsterType = Monster.Dragon; }
            else if(name.Contains("Wizard")) { MonsterType = Monster.Wizard; }
            else if(name.Contains("Ork")) { MonsterType = Monster.Ork; }
            else if(name.Contains("Knight")) { MonsterType = Monster.Knight; }
            else if(name.Contains("Kraken")) { MonsterType = Monster.Kraken; }
            else { MonsterType = Monster.Elf; }
        }
    }
}
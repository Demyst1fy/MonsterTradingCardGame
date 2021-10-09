namespace SWEN1.MTCG.ClassLibrary
{
    public class MonsterCard : Card
    {
        public Monster MonsterType { get; private set; }
        public MonsterCard(string name, int damage, Element element, Monster monsterType) : base(name, damage, element)
        {
            MonsterType = monsterType;
        }
    }
}
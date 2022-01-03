namespace SWEN1.MTCG.Game.Interfaces
{
    public interface ICard
    {
        string Id { get; }
        string Name { get; }
        double Damage { get; }
        Element Element { get; }
        Type Type { get; }
        double CompareElement(Element enemyCard);
        string CheckEffect(ICard enemyCard);
    }
}
namespace SWEN1.MTCG.GameClasses.Interfaces
{
    public interface ICard
    {
        string Id { get; }
        string Name { get; }
        double Damage { get; }
        Element Element { get; }
        Type Type { get; }
        double CompareElement(Element enemyCard);
        bool CheckEffect(ICard enemyCard);
    }
}
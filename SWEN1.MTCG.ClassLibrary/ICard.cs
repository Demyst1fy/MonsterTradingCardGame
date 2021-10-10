namespace SWEN1.MTCG.ClassLibrary
{
    public interface ICard
    {
        string Id { get; }
        string Name { get; }
        double Damage { get; }
        Element Element { get; }
    }
}
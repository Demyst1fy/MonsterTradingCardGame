namespace SWEN1.MTCG.Server.JsonClasses
{
    public class PackageJson
    {
        public string PackId { get; }
        public object[] Cards { get; }

        public PackageJson(string packId, object[] cards)
        {
            PackId = packId;
            Cards = cards;
        }
    }
}
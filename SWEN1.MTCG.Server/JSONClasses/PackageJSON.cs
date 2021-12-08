namespace SWEN1.MTCG.Server.JSONClasses
{
    public class PackageJSON
    {
        public string PackId { get; }
        public object[] Cards { get; }

        public PackageJSON(string packId, object[] cards)
        {
            PackId = packId;
            Cards = cards;
        }
    }
}
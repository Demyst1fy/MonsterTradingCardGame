namespace SWEN1.MTCG.Server.DatabaseClasses
{
    public class TransactionTable
    {
        public string PackId { get; }
        public int Cost { get; }
        public string DateTime { get; }

        public TransactionTable(string packId, int cost, string dateTime)
        {
            PackId = packId;
            Cost = cost;
            DateTime = dateTime;
        }
    }
}
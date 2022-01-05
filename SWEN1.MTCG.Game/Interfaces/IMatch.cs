namespace SWEN1.MTCG.Game.Interfaces
{
    public interface IMatch
    {
        ILogging Logger { get; set; }
        IUser Player1 { get; }
        IUser Player2 { get; set; }
        void AddUser(IUser player2);
        void BattleAction(ILogging logger);
        void ProcessRunningGame();
    }
}
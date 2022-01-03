using System.Text;

namespace SWEN1.MTCG.Game.Interfaces
{
    public interface ILogging
    {
        IUser Player1 { get; }
        IUser Player2 { get; }
        StringBuilder Log { get; }
        
        void AppendLogWithLine(string text);
        string LogString();
    }
}
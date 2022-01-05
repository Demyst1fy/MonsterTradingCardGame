using System.Collections.Concurrent;
using SWEN1.MTCG.Game;

namespace SWEN1.MTCG.Server.Interfaces
{
    public interface IServiceHandler
    {
        IRequest ParseRequest(string data);
        IResponse HandleRequest(IRequest parsedRequest, ref ConcurrentQueue<Match> allBattles);
    }
}
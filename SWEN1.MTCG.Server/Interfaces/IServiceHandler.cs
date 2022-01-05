using System.Collections.Concurrent;
using SWEN1.MTCG.Game.Interfaces;

namespace SWEN1.MTCG.Server.Interfaces
{
    public interface IServiceHandler
    {
        IRequest ParseRequest(string data);
        IResponse HandleRequest(IRequest parsedRequest, ref ConcurrentQueue<IMatch> allBattles);
    }
}
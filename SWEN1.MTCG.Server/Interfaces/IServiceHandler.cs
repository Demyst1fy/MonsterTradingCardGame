using System.Collections.Concurrent;
using SWEN1.MTCG.Game;

namespace SWEN1.MTCG.Server.Interfaces
{
    public interface IServiceHandler
    {
        Response HandleRequest(string request, ref ConcurrentBag<Match> allBattles);
    }
}
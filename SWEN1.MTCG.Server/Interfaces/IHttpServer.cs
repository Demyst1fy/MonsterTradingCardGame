using System.Collections.Concurrent;
using SWEN1.MTCG.Game;

namespace SWEN1.MTCG.Server.Interfaces
{
    public interface IHttpServer
    {
        void Start(int port);
        void Stop();
    }
}
namespace SWEN1.MTCG.Server.Interfaces
{
    public interface IServiceHandler
    {
        Response HandleRequest(string request);
    }
}
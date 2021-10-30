namespace SWEN1.MTCG.Server.Interfaces
{
    public interface IRequest
    {
        string Method { get; }
        string Query { get; }
        string Content { get; }
        string AuthToken { get; }
    }
}
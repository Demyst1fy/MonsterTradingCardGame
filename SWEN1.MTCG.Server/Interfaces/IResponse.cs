namespace SWEN1.MTCG.Server.Interfaces
{
    public interface IResponse
    {
        int Status { get; }
        string Message { get; }
        string MimeType { get; }
        int ContentLength { get; }
        string Body { get; }
    }
}
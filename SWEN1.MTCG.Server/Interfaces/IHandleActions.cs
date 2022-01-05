using System.Collections.Concurrent;
using SWEN1.MTCG.Game.Interfaces;

namespace SWEN1.MTCG.Server.Interfaces
{
    public interface IHandleActions
    {
        string GetUsernameFromAuthKey(string authToken);
        IResponse HandleRegistration(string requestContent);
        IResponse HandleLogin(string requestContent);
        IResponse HandleCreatePackage(string requestContent, string username);
        IResponse HandleAcquirePackage(string requestContent, string username);
        IResponse HandleShowTransactions(string username);
        IResponse HandleShowStack(string username);
        IResponse HandleShowDeck(string username);
        IResponse HandleShowDeckInPlain(string username);
        IResponse HandleConfigureDeck(string requestContent, string username);
        IResponse HandleEditUserData(string subQuery, string requestContent, string username);
        IResponse HandleGetUserData(string subQuery, string username);
        IResponse HandleShowStats(string username);
        IResponse HandleShowScoreboard(string username);
        IResponse HandleBattle(string username, ref ConcurrentQueue<IMatch> allMatches);
        IResponse HandleCreateTradingDeal(string requestContent, string username);
        IResponse HandleShowTradingDeals(string username);
        IResponse HandleDeleteTradingDeal(string subQuery, string username);
        IResponse HandleProcessTradingDeal(string subQuery, string requestContent, string username);
    }
}
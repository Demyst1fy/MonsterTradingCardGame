using System.Collections.Generic;
using SWEN1.MTCG.GameClasses.Interfaces;
using SWEN1.MTCG.Server.DatabaseClasses;

namespace SWEN1.MTCG.Server.Interfaces
{
    public interface IDatabase
    {
        RegisterStatus RegisterUser(string username, string password);
        LoginStatus LoginUser(string username, string password);
        Usertable GetUserData(string username);
        EditUserDataStatus EditUserData(string username, string fullname, string bio, string image);
        ConfigDeckStatus ConfigureDeck(string[] chosenCardIDs, string username);
        List<ICard> GetUserStack(string username);
        List<ICard> GetUserDeck(string username);
        Statstable GetUserStats(string username);
        List<Statstable> GetScoreBoard();
        List<Tradetable> GetTradingDeals();
        CreateTradingDealStatus CreateTradingDeal(string username, string tradeId, string cardId, string searchType, string minimumDamageString);
        DeleteTradingDealStatus DeleteTradingDeal(string tradeId, string username);
        ProcessTradingDealStatus ProcessTradingDeal(string tradeId, string offeredCardId, string username);
        string GetUsernameFromAuthKey(string authToken);
    }
}
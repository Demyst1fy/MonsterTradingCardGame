using System.Collections.Generic;
using SWEN1.MTCG.Game;
using SWEN1.MTCG.Game.Interfaces;
using SWEN1.MTCG.Server.DatabaseClasses;

namespace SWEN1.MTCG.Server.Interfaces
{
    public interface IDatabase
    {
        RegisterStatus RegisterUser(string username, string password);
        LoginStatus LoginUser(string username, string password);
        CreatePackageStatus CreatePackage(string packId, string cardId, string cardName, double cardDamage);
        AcquirePackageStatus AcquirePackage(string packId, string username);
        UserTable GetUserData(string username);
        EditUserDataStatus EditUserData(string username, string fullname, string bio, string image);
        ConfigDeckStatus ConfigureDeck(string[] chosenCardIDs, string username);
        List<ICard> GetUserStack(string username);
        List<ICard> GetUserDeck(string username);
        StatsTable GetUserStats(string username);
        Stats GetUserStatsWithoutUserName(string username);
        List<StatsTable> GetScoreBoard();
        List<TradeTable> GetTradingDeals();
        CreateTradingDealStatus CreateTradingDeal(string username, string tradeId, string cardId, string searchType, double? minimumDamage);
        DeleteTradingDealStatus DeleteTradingDeal(string tradeId, string username);
        ProcessTradingDealStatus ProcessTradingDeal(string tradeId, string offeredCardId, string username);
        string GetUsernameFromAuthKey(string authToken);
        bool CheckPackageExist(string packId);
        void UpdateStatsAfterMatch(string winner, string loser);
        void UpdateStatsAfterMatchDraw(string player1, string player2);
    }
}
namespace SWEN1.MTCG.Server
{
    public enum RegisterStatus { FieldEmpty, AlreadyExist, Success }
    public enum LoginStatus { FieldEmpty, IncorrectData, Success }
    public enum CreatePackageStatus { FieldEmpty, AlreadyExist, Success }
    public enum AcquirePackageStatus { NotExist, NoCoins, AlreadyOwn, Success }
    public enum ConfigDeckStatus { NotFourCards, NoMatchCards, Success }
    public enum EditUserDataStatus { FieldEmpty, Success }
    public enum CreateTradingDealStatus { FieldEmpty, CardInDeck, Success }
    public enum DeleteTradingDealStatus { FromOtherUser, Success }
    public enum ProcessTradingDealStatus { NotExist, SameUser, RequestNotExist, NotWanted, Success }
}
using System.Collections.Generic;

namespace SWEN1.MTCG.GameClasses.Interfaces
{
    public interface IUser
    {
        int ID { get; }
        string Username { get; }
        List<ICard> Deck { get; set; }
    }
}
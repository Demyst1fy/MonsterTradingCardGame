using System.Collections.Generic;

namespace SWEN1.MTCG.ClassLibrary
{
    public interface IUser
    {
        int ID { get; }
        string Username { get; }
        List<ICard> Deck { get; set; }
    }
}
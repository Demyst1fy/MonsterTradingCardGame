using System.Collections.Generic;

namespace SWEN1.MTCG.ClassLibrary
{
    public interface IUser
    {
        string Username { get; }
        string Password { get; }
        int Coins { get; }
        List<ICard> StackCollection { get; set; }
        List<ICard> DeckCollection { get; set; }
    }
}
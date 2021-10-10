using System.Collections.Generic;

namespace SWEN1.MTCG.ClassLibrary
{
    public interface IUser
    {
        string Username { get; }
        string Password { get; }
        int Coins { get; }
        List<Card> StackCollection { get; }
        List<Card> DeckCollection { get; }
    }
}
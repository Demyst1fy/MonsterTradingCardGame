using System.Collections.Generic;

namespace SWEN1.MTCG.Game.Interfaces
{
    public interface IUser
    {
        string Username { get; }
        List<ICard> Deck { get; }
        
        Stats Stats { get; }
    }
}
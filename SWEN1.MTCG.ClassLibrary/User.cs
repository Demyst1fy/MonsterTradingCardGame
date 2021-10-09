using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWEN1.MTCG.ClassLibrary;

namespace SWEN1.MTCG.ClassLibrary
{
    public interface IUser
    {
        
    }

    public class User : IUser
    {
        public string username { get; private set; }
        public string password { get; private set; }
        public int coins { get; private set; }
        
        public List<Card> stackCollection { get; private set; }
        public List<Card> deckCollection { get; private set; }

        public User(string username, string password, int coins)
        {
            this.username = username;
            this.password = password;
            this.coins = coins;
        }

        public void ChooseDeckCards(List<Card> deckCollection_)
        {
            deckCollection = deckCollection_;
        }
    }
}
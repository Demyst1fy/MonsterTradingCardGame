using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN1.MTCG.ClassLibrary
{
    public class Game
    {
        public int round { get; private set; }
        
        private readonly User player1;
        private readonly User player2;

        public Game(User player1, User player2)
        {
            this.player1 = player1;
            this.player2 = player2;
        }
        
        public double compareElement(Card playerCard, Card enemyCard)
        {
            double damageAdj;
            
            if ((playerCard.element == Element.water && enemyCard.element == Element.fire) ||
                (playerCard.element == Element.fire && enemyCard.element == Element.normal) ||
                (playerCard.element == Element.normal && enemyCard.element == Element.water))
            {
                damageAdj = playerCard.damage * 2;
            } 
            else if ((playerCard.element == Element.fire && enemyCard.element == Element.water) ||
                     (playerCard.element == Element.water && enemyCard.element == Element.normal) ||
                     (playerCard.element == Element.normal && enemyCard.element == Element.fire))
            {
                damageAdj = playerCard.damage * 0.5;
            }
            else
            {
                damageAdj = playerCard.damage * 0;
            }

            return damageAdj;
        }
        public void battleAction()
        {
            Random rd = new Random();
            int rdPlayer1 = rd.Next(player1.deckCollection.Count);
            int rdPlayer2 = rd.Next(player2.deckCollection.Count);
            
            Card playerCard1 = player1.deckCollection[rdPlayer1];
            Card playerCard2 = player2.deckCollection[rdPlayer2];
            
            Console.WriteLine($"{player1.username}: {playerCard1.name} ({playerCard1.damage} Damage) " +
                              $"VS {player2.username}: {playerCard2.name} ({playerCard2.damage} Damage)\n");

            double damageAdj1, damageAdj2;

            if (playerCard1 is SpellCard || playerCard2 is SpellCard)
            {
                damageAdj1 = compareElement(playerCard1, playerCard2);
                damageAdj2 = compareElement(playerCard2, playerCard1);
                Console.WriteLine($"=> {playerCard1.damage} VS {playerCard2.damage} -> {damageAdj1} VS {damageAdj2}\n");
            }
            else
            {
                damageAdj1 = playerCard1.damage;
                damageAdj2 = playerCard2.damage;
            }
            
            if (damageAdj1 > damageAdj2)
            {
                Console.WriteLine($"=> {playerCard1.name} wins.");
            }
            else if (damageAdj1 < damageAdj2)
            {
                Console.WriteLine($"=> {playerCard2.name} wins.");
            }
            else
            {
                Console.WriteLine("Draw (no action)");
            }
        }
    }
}
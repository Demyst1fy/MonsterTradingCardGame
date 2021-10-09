using System;

namespace SWEN1.MTCG.ClassLibrary
{
    public class Game
    {
        public int Round { get; private set; }
        
        private readonly User _player1;
        private readonly User _player2;

        public Game(User player1, User player2)
        {
            _player1 = player1;
            _player2 = player2;
        }

        private static double CompareElement(Card playerCard, Card enemyCard)
        {
            double damageAdj;
            
            if ((playerCard.Element == Element.Water && enemyCard.Element == Element.Fire) ||
                (playerCard.Element == Element.Fire && enemyCard.Element == Element.Normal) ||
                (playerCard.Element == Element.Normal && enemyCard.Element == Element.Water))
            {
                damageAdj = playerCard.Damage * 2;
            } 
            else if ((playerCard.Element == Element.Fire && enemyCard.Element == Element.Water) ||
                     (playerCard.Element == Element.Water && enemyCard.Element == Element.Normal) ||
                     (playerCard.Element == Element.Normal && enemyCard.Element == Element.Fire))
            {
                damageAdj = playerCard.Damage * 0.5;
            }
            else
            {
                damageAdj = playerCard.Damage * 0;
            }

            return damageAdj;
        }
        public void BattleAction()
        {
            var rd = new Random();
            var rdPlayer1 = rd.Next(_player1.DeckCollection.Count);
            var rdPlayer2 = rd.Next(_player2.DeckCollection.Count);
            
            var playerCard1 = _player1.DeckCollection[rdPlayer1];
            var playerCard2 = _player2.DeckCollection[rdPlayer2];
            
            Console.WriteLine($"{_player1.Username}: {playerCard1.Name} ({playerCard1.Damage} Damage) " +
                              $"VS {_player2.Username}: {playerCard2.Name} ({playerCard2.Damage} Damage)\n");

            double damageAdj1, damageAdj2;

            if (playerCard1 is SpellCard || playerCard2 is SpellCard)
            {
                damageAdj1 = CompareElement(playerCard1, playerCard2);
                damageAdj2 = CompareElement(playerCard2, playerCard1);
                Console.WriteLine($"=> {playerCard1.Damage} VS {playerCard2.Damage} -> {damageAdj1} VS {damageAdj2}\n");
            }
            else
            {
                damageAdj1 = playerCard1.Damage;
                damageAdj2 = playerCard2.Damage;
            }
            
            if (damageAdj1 > damageAdj2)
            {
                Console.WriteLine($"=> {playerCard1.Name} wins.");
            }
            else if (damageAdj1 < damageAdj2)
            {
                Console.WriteLine($"=> {playerCard2.Name} wins.");
            }
            else
            {
                Console.WriteLine("Draw (no action)");
            }
        }
    }
}
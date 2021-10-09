using System;
using System.Collections.Generic;

namespace SWEN1.MTCG.ClassLibrary
{
    public class Game
    {
        public int Round { get; private set; }
        
        private User _player1;
        private User _player2;

        public Game(User player1, User player2)
        {
            _player1 = player1;
            _player2 = player2;
            Round = 1;
        }

        public static double CompareElement(Card playerCard, Card enemyCard)
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
            Random rd = new Random();
            List<Card> player1Cards = _player1.DeckCollection;
            List<Card> player2Cards = _player2.DeckCollection;
            
            int rdPlayer1 = rd.Next(player1Cards.Count);
            int rdPlayer2 = rd.Next(player2Cards.Count);

            var player1ChosenCard1 = player1Cards[rdPlayer1];
            var player2ChosenCard2 = player2Cards[rdPlayer2];

            Console.WriteLine($"{_player1.Username}'s DeckList:");
            for (int i = 0; i < player1Cards.Count; i++)
            {
                Console.WriteLine($"{i+1}: {player1Cards[i].Name} ({player1Cards[i].Damage} Damage)");
            }
            
            Console.WriteLine($"\n{_player2.Username}'s DeckList:");
            for (int i = 0; i < player2Cards.Count; i++)
            {
                Console.WriteLine($"{i+1}: {player2Cards[i].Name} ({player2Cards[i].Damage} Damage)");
            }
            
            Console.WriteLine($"\nRound {Round}");
            Console.WriteLine($"{_player1.Username}: {player1ChosenCard1.Name} ({player1ChosenCard1.Damage} Damage) " +
                              $"VS {_player2.Username}: {player2ChosenCard2.Name} ({player2ChosenCard2.Damage} Damage)\n");

            double damageAdj1, damageAdj2;

            if (player1ChosenCard1 is SpellCard || player2ChosenCard2 is SpellCard)
            {
                damageAdj1 = CompareElement(player1ChosenCard1, player2ChosenCard2);
                damageAdj2 = CompareElement(player2ChosenCard2, player1ChosenCard1);
                Console.WriteLine($"=> {player1ChosenCard1.Damage} VS {player2ChosenCard2.Damage} -> {damageAdj1} VS {damageAdj2}\n");
            }
            else
            {
                damageAdj1 = player1ChosenCard1.Damage;
                damageAdj2 = player2ChosenCard2.Damage;
            }
            
            if (damageAdj1 > damageAdj2)
            {
                Console.WriteLine($"=> {player1ChosenCard1.Name} wins.\n");
                _player1.DeckCollection.Add(player2ChosenCard2);
                _player2.DeckCollection.Remove(player2ChosenCard2);
            }
            else if (damageAdj1 < damageAdj2)
            {
                Console.WriteLine($"=> {player2ChosenCard2.Name} wins.\n");
                _player2.DeckCollection.Add(player1ChosenCard1);
                _player1.DeckCollection.Remove(player1ChosenCard1);
            }
            else
            {
                Console.WriteLine("Draw (no action)\n");
            }

            Round++;
        }
    }
}
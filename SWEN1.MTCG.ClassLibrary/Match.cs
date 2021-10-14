using System;
using System.Collections.Generic;

namespace SWEN1.MTCG.ClassLibrary
{
    public class Match
    {
        public int Round { get; set; } = 1;

        public IUser Player1 { get; set; }
        public IUser Player2 { get; set; }

        public int Player1RoundWon { get; set; }
        public int Player2RoundWon { get; set; }
        public Match(IUser player1, IUser player2)
        {
            Player1 = player1;
            Player2 = player2;
        }

        public void BattleAction()
        {
            var rd = new Random();

            var myCards = Player1.DeckCollection;
            var enemyCards = Player2.DeckCollection;

            var rd1 = rd.Next(myCards.Count);
            var rd2 = rd.Next(enemyCards.Count);

            var myCard = myCards[rd1];
            var enemyCard = enemyCards[rd2];

            Console.WriteLine($"{Player1.Username}'s DeckList:");
            foreach (var player1Card in myCards)
            {
                Console.WriteLine($"- {player1Card.Name} ({player1Card.Damage} Damage)");
            }
            
            Console.WriteLine($"\n{Player2.Username}'s DeckList:");
            foreach (var player2Card in enemyCards)
            {
                Console.WriteLine($"- {player2Card.Name} ({player2Card.Damage} Damage)");
            }
            
            //System.Threading.Thread.Sleep(1000);
            
            Console.WriteLine($"\nRound {Round}");
            Console.WriteLine($"{Player1.Username}: {myCard.Name} ({myCard.Damage} Damage) " +
                              $"VS {Player2.Username}: {enemyCard.Name} ({enemyCard.Damage} Damage)\n");

            //System.Threading.Thread.Sleep(1000);
            
            double damageAdj1 = 0;
            double damageAdj2 = 0;

            var monsterStatus1 = myCard.CheckEffect(enemyCard);
            var monsterStatus2 = enemyCard.CheckEffect(myCard);

            if (monsterStatus1 == false && monsterStatus2 == false)
            {
                if (myCard.Type != Type.Spell && enemyCard.Type != Type.Spell)
                {
                    damageAdj1 = myCard.Damage;
                    damageAdj2 = enemyCard.Damage;
                }
            
                else
                {
                    damageAdj1 = myCard.CompareElement(enemyCard.Element);
                    damageAdj2 = enemyCard.CompareElement(myCard.Element);
                    Console.WriteLine($"=> {myCard.Damage} VS {enemyCard.Damage} -> {damageAdj1} VS {damageAdj2}\n");
                }
            }

            if (damageAdj1 > damageAdj2 || monsterStatus2)
            {
                Console.WriteLine($"=> {myCard.Name} wins.\n");
                myCards.Add(enemyCard);
                enemyCards.Remove(enemyCard);
                Player1RoundWon++;
            }
            else if (damageAdj1 < damageAdj2 || monsterStatus1)
            {
                Console.WriteLine($"=> {enemyCard.Name} wins.\n");
                enemyCards.Add(myCard);
                myCards.Remove(myCard);
                Player2RoundWon++;
            }
            
            else
            {
                Console.WriteLine("Draw (no action)\n");
            }
            
            //System.Threading.Thread.Sleep(2000);
            Round++;
        }
    }
}
using System;
using System.Collections.Generic;
using SWEN1.MTCG.ClassLibrary;

namespace SWEN1.MTCG
{
    class Program
    {
        static void Main(string[] args)
        {
            var player1 = new User("Jay", "12345", 50);
            var player2 = new User("Marc", "54321", 60);
            
            var player1Cards = new List<Card>();
            var player2Cards = new List<Card>();
            
            player1Cards.Add(DeclareCard("FireElf", 12));
            player1Cards.Add(DeclareCard("WaterGoblin", 14));
            player1Cards.Add(DeclareCard("Kraken", 16));
            player1Cards.Add(DeclareCard("WaterOrk", 19));
            player2Cards.Add(DeclareCard("Wizard", 45));
            player2Cards.Add(DeclareCard("WaterDragon", 45));
            player2Cards.Add(DeclareCard("FireOrk", 40));
            player2Cards.Add(DeclareCard("FireGoblin", 35));
            
            player1.ChooseDeckCards(player1Cards);
            player2.ChooseDeckCards(player2Cards);
            
            var game = new Game(player1, player2);
            
            while (game.Round < 100 && player1Cards.Count > 0 && player2Cards.Count > 0)
            {
                game.BattleAction();
            }

            if (player1Cards.Count <= 0)
            {
                Console.WriteLine($"{player2.Username} won the game!");
            }
            else if (player2Cards.Count <= 0)
            {
                Console.WriteLine($"{player1.Username} won the game!");
            }
            else
            {
                Console.WriteLine($"Over 100 Rounds were player, let's decide it to a draw!");
            }
            
        }

        public static Card DeclareCard(string name, int damage)
        {
            if (name.Contains("Spell"))
            {
                return new SpellCard(name, damage);
            }
            return new MonsterCard(name, damage);
        }
    }
}
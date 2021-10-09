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

            player1Cards.Add(new MonsterCard("FireOrk", 15, Element.Fire, Monster.Ork));
            player1Cards.Add(new SpellCard("WaterSpell", 75, Element.Water));
            player1Cards.Add(new MonsterCard("Knight", 10, Element.Normal, Monster.Knight));
            player1Cards.Add(new SpellCard("RegularSpell", 50, Element.Water));
            player2Cards.Add(new MonsterCard("Knight", 15, Element.Normal, Monster.Knight));
            player2Cards.Add(new SpellCard("FireSpell", 45, Element.Fire));
            player2Cards.Add(new SpellCard("WaterSpell", 40, Element.Fire));
            player2Cards.Add(new SpellCard("RegularSpell", 35, Element.Fire));
            
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
    }
}
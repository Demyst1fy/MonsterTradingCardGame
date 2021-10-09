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
            var game = new Game(player1, player2);
   
            var player1Cards = new List<Card>();
            var player2Cards = new List<Card>();

            player1Cards.Add(new MonsterCard("FireOrk", 10, Element.Fire, Monster.Ork));
            player1Cards.Add(new SpellCard("WaterSpell", 75, Element.Water));
            player2Cards.Add(new MonsterCard("Knight", 15, Element.Normal, Monster.Knight));
            player2Cards.Add(new SpellCard("FireSpell", 90, Element.Fire));

            player1.ChooseDeckCards(player1Cards);
            player2.ChooseDeckCards(player2Cards);

            game.BattleAction();
        }
    }
}
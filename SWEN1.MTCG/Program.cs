using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using Microsoft.VisualBasic.FileIO;
using SWEN1.MTCG.ClassLibrary;

namespace SWEN1.MTCG
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to your Monster Trading Card Game!");
            Console.WriteLine("---------------------------------------");
            string username = "";
            string password = "";

            while (username != "Jay" || password != "12345")
            {
                username = EnterCredentials("Username: ");
                password = EnterCredentials("Password: ");
            }

            List<Card> player1Cards = new List<Card>();
            player1Cards.Add(Shop.DeclareCard("dasdacx","FireElf", 12));
            player1Cards.Add(Shop.DeclareCard("fdsgfdh","WaterGoblin", 14));
            player1Cards.Add(Shop.DeclareCard("hfghffvgn","Kraken", 16));
            player1Cards.Add(Shop.DeclareCard("gdfgdfgdb","WaterOrk", 19));

            List<Card> player2Cards = new List<Card>();
            player2Cards.Add(Shop.DeclareCard("gfjgfmjg","Wizard", 45));
            player2Cards.Add(Shop.DeclareCard("jfgjgfjrt","WaterDragon", 45));
            player2Cards.Add(Shop.DeclareCard("hfghgfn","FireOrk", 40));
            player2Cards.Add(Shop.DeclareCard("hfghfhf","FireGoblin", 35));

            var user = new User(username, password, 20, player1Cards);
            var bot = new User("Marc", "54321", 60, player2Cards);

            var game = new Match(user, bot);

            while (game.Round < 100 && user.DeckCollection.Count > 0 && bot.DeckCollection.Count > 0)
            {
                game.BattleAction();
            }

            if (user.DeckCollection.Count <= 0)
            {
                Console.WriteLine($"{bot.Username} won the game!");
                bot.increWins();
                user.increLosses();
            }
            else if (bot.DeckCollection.Count <= 0)
            {
                Console.WriteLine($"{user.Username} won the game!");
                user.increWins();
                bot.increLosses();
            }
            else
            {
                Console.WriteLine($"Over 100 Rounds were player, let's decide it to a draw!");
            }

            Console.WriteLine("\nWon rounds:");
            Console.WriteLine($"{user.Username}: {game.Player1RoundWon}");
            Console.WriteLine($"{bot.Username}: {game.Player2RoundWon}");
            
        }
        

        private static string EnterCredentials(string message)
        {
            string input = "";
            Console.Write($"{message}");
            
            input = Console.ReadLine();
            return input;
        }
        
        public static int UserInput()
        {
            int input = 9;
            Console.Write("\n1. Play one ranked match\n" +
                          "2. Buy Packages (5 Cards) for 5 coins\n" +
                          "3. Manage your cards\n" +
                          "4. Deal your Trade\n" +
                          "5. Quit\n" +
                          "Choose one menu point: ");
            
            while (input < 1 || input > 5)
            {
                input = Convert.ToInt32(Console.ReadLine());
                if (input < 1 || input > 5) {
                    Console.Write("Unknown entry! Try again: ");
                }
            }

            return input;
        }
    }
}
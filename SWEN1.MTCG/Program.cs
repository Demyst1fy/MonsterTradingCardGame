using System;
using System.Collections.Generic;
using SWEN1.MTCG.ClassLibrary;

namespace SWEN1.MTCG
{
    class Program
    {
        static void Main(string[] args)
        {
            var player1Cards = new List<Card>();
            var player2Cards = new List<Card>();
            
            player1Cards.Add(DeclareCard(GenerateId(),"FireElf", 12));
            player1Cards.Add(DeclareCard(GenerateId(),"WaterGoblin", 14));
            player1Cards.Add(DeclareCard(GenerateId(),"Kraken", 16));
            player1Cards.Add(DeclareCard(GenerateId(),"WaterOrk", 19));
            player1Cards.Add(DeclareCard(GenerateId(),"Wizard", 45));
            player1Cards.Add(DeclareCard(GenerateId(),"WaterDragon", 45));
            player1Cards.Add(DeclareCard(GenerateId(),"FireOrk", 40));
            player1Cards.Add(DeclareCard(GenerateId(),"FireGoblin", 35));
            
            player2Cards.Add(DeclareCard(GenerateId(),"Wizard", 45));
            player2Cards.Add(DeclareCard(GenerateId(),"WaterDragon", 45));
            player2Cards.Add(DeclareCard(GenerateId(),"FireOrk", 40));
            player2Cards.Add(DeclareCard(GenerateId(),"FireGoblin", 35));
            
            var player1 = new User("Jay", "12345", 50, player1Cards);
            var player2 = new User("Marc", "54321", 60, player2Cards);
            
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

        private static Card DeclareCard(string id, string name, int damage)
        {
            if (name.Contains("Spell"))
            {
                return new SpellCard(id, name, damage);
            }
            return new MonsterCard(id, name, damage);
        }

        private static string GenerateId()
        {
            const int idLength = 36;
            
            var random = new Random();
            const string chars = "abcdefghijklmnpqrstuvwxyz0123456789";

            var buffer = new char[idLength];
            
            for(var i = 0; i < idLength; ++i)
            {
                if (i is 8 or 13 or 18 or 23)
                {
                    buffer[i] = '-';
                }
                else
                {
                    buffer[i] = chars[random.Next(chars.Length)];
                }
            }

            return new string(buffer);
        }
    }
}
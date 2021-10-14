using System;
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

            var user = new User(username, password, 20);
            
            user.DeckCollection.Add(new Card("dasdacx","FireSpell", 40));
            user.DeckCollection.Add(new Card("fdsgfdh","WaterGoblin", 45));
            user.DeckCollection.Add(new Card("hfghffvgn","Kraken", 38));
            user.DeckCollection.Add(new Card("gdfgdfgdb","WaterOrk", 49));
            
            var bot = new User("Marc", "54321", 60);
            bot.DeckCollection.Add(new Card("gfjgfmjg","Wizard", 45));
            bot.DeckCollection.Add(new Card("jfgjgfjrt","WaterDragon", 45));
            bot.DeckCollection.Add(new Card("hfghgfn","FireOrk", 40));
            bot.DeckCollection.Add(new Card("hfghfhf","WaterSpell", 35));


            int input = 9;
            
            while (input != 5)
            {
                input = UserInput();
                switch (input)
                {
                    case 1:
                    {
                        var userTmp = new User(user);
                        var enemyTmp = new User(bot);
                        
                        var game = new Match(userTmp, enemyTmp);
                        
                        while (game.Round <= 100 && userTmp.DeckCollection.Count > 0 && enemyTmp.DeckCollection.Count > 0)
                        {
                            game.BattleAction();
                        }
        
                        if (userTmp.DeckCollection.Count <= 0)
                        {
                            Console.WriteLine($"{bot.Username} won the game!");
                            bot.IncreWins();
                            user.IncreLosses();
                        }
                        else if (enemyTmp.DeckCollection.Count <= 0)
                        {
                            Console.WriteLine($"{user.Username} won the game!");
                            user.IncreWins();
                            bot.IncreLosses();
                        }
                        else
                        {
                            Console.WriteLine($"Over 100 Rounds were player, let's decide it to a draw!");
                            user.IncreDraws();
                            bot.IncreDraws();
                        }
        
                        Console.WriteLine("\nWon rounds:");
                        Console.WriteLine($"{user.Username}: {game.Player1RoundWon}");
                        Console.WriteLine($"{bot.Username}: {game.Player2RoundWon}");
                        break;
                    }
                    case 2:
                        Shop.BuyPackage(user);
                        break;
                    case 3:
                        user.OutPutWinRate();
                        break;
                }
                
            }
        
        }
        

        private static string EnterCredentials(string message)
        {
            Console.Write($"{message}");
            
            var input = Console.ReadLine();
            return input;
        }
        
        public static int UserInput()
        {
            var input = 9;
            Console.Write("\n1. Play one ranked match\n" +
                          "2. Buy Packages (5 Cards) for 5 coins\n" +
                          "3. Manage your cards\n" +
                          "4. Deal your Trade\n" +
                          "5. Quit\n" +
                          "Choose one menu point: ");
            
            while (input is < 1 or > 5)
            {
                if (!int.TryParse(Console.ReadLine(), out input) && input is < 1 or > 5) {
                    Console.Write("Unknown entry! Try again: ");
                }
            }

            return input;
        }
    }
}
using System;

namespace SWEN1.MTCG.GameClasses
{
    public class Shop
    {
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

        private static string GenerateCardName()
        {
            string[] cardTypes = {"Spell", "Goblin", "Dragon", "Wizard", "Ork", "Knight", "Kraken", "Elf"};
            string[] elements = {"Fire", "Water", "Regular"};
            
            var rd = new Random();
            int rdCardType = rd.Next(cardTypes.Length);
            int rdElement = rd.Next(elements.Length);

            string chosenCardType = cardTypes[rdCardType];
            string chosenElement = elements[rdElement];
            
            if(chosenCardType != "Spell" && chosenElement == "Regular") { chosenElement = ""; }

            return chosenElement + chosenCardType;
        }

        // temporarily add on deck
        /*public static void BuyPackage(User user)
        {
            Database database = new Database();
            
            Console.WriteLine("You gained following cards: ");
            for (int i = 0; i < 5; i++)
            {
                var rd = new Random();
                
                string rdID = GenerateId();
                string rdCardName = GenerateCardName();
                int rdDmg = rd.Next(50, 90);
                
                database.InsertNewCardStack(user.ID, rdID, rdCardName, rdDmg);
                Console.WriteLine($"- {rdCardName} ({rdDmg})");
            }
        }*/
    }
}
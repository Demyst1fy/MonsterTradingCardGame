using System;
using System.Net.Http.Headers;

namespace SWEN1.MTCG.ClassLibrary
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
        public static void BuyPackage(User user)
        {

            Console.WriteLine("You gained following cards: ");
            for (int i = 0; i < 5; i++)
            {
                var rd = new Random();
                int dmg = rd.Next(50, 90);
                user.DeckCollection.Add(new Card(GenerateId(), GenerateCardName(), dmg));
                Console.WriteLine($"- {user.DeckCollection[i].Name} ({user.DeckCollection[i].Damage})");
            }
        }
    }
}
using System.Collections.Generic;
using NUnit.Framework;
using SWEN1.MTCG.Game;
using SWEN1.MTCG.Game.Interfaces;

namespace SWEN1.MTCG.Test.GameClasses.Test
{
    public class CardTest
    {
        [Test]
        public void Test_AssignMonsterCard()
        {
            var card = new Card("845f0dc7-37d0-426e-994e-43fc3ac83c08","Knight", 15);
            Assert.AreEqual("845f0dc7-37d0-426e-994e-43fc3ac83c08", card.Id);
            Assert.AreEqual("Knight", card.Name);
            Assert.AreEqual(15, card.Damage);
            Assert.AreEqual(Element.Normal, card.Element);
            Assert.AreEqual(Type.Knight, card.Type);
        }
        
        [Test]
        public void Test_AssignSpellCard()
        {
            var card = new Card("99f8f8dc-e25e-4a95-aa2c-782823f36e2a","WaterSpell", 75);
            Assert.AreEqual("99f8f8dc-e25e-4a95-aa2c-782823f36e2a", card.Id);
            Assert.AreEqual("WaterSpell", card.Name);
            Assert.AreEqual(75, card.Damage);
            Assert.AreEqual(Element.Water, card.Element);
            Assert.AreEqual(Type.Spell, card.Type);
        }

        [Test]
        public void Test_AssignUserWithStackAndDeck()
        {
            User user1 = new User("Jay", new List<ICard>(), new Stats(2, 0, 1, 106));
            user1.Deck.Add(new Card("67f9048f-99b8-4ae4-b866-d8008d00c53d", "FireOrk", 10));
            user1.Deck.Add(new Card("70962948-2bf7-44a9-9ded-8c68eeac7793", "FireSpell", 90));

            foreach (var card in user1.Deck)
            {
                if (card.Type != Type.Spell)
                {
                    Assert.AreEqual("67f9048f-99b8-4ae4-b866-d8008d00c53d", card.Id);
                    Assert.AreEqual("FireOrk", card.Name);
                    Assert.AreEqual(10, card.Damage);
                    Assert.AreEqual(Element.Fire, card.Element);
                    Assert.AreEqual(Type.Ork, card.Type);
                }
                else
                {
                    Assert.AreEqual("70962948-2bf7-44a9-9ded-8c68eeac7793", card.Id);
                    Assert.AreEqual("FireSpell", card.Name);
                    Assert.AreEqual(90, card.Damage);
                    Assert.AreEqual(Element.Fire, card.Element);
                    Assert.AreEqual(Type.Spell, card.Type);
                }
            }
        }

        [Test]
        public void Test_DamageAdjustment_IncreaseAndDecrease()
        {
            var card1 = new Card("67f9048f-99b8-4ae4-b866-d8008d00c53d", "FireSpell", 60);
            var card2 = new Card("70962948-2bf7-44a9-9ded-8c68eeac7793","WaterWizard", 70);

            Assert.AreEqual(30, card1.CompareElement(card2.Element));
            Assert.AreEqual(140, card2.CompareElement(card1.Element));
        }
        
        [Test]
        public void Test_DamageAdjustment_NoDamages()
        {
            var card1 = new Card("67f9048f-99b8-4ae4-b866-d8008d00c53d", "FireSpell", 60);
            var card2 = new Card("70962948-2bf7-44a9-9ded-8c68eeac7793","FireElf", 70);

            Assert.AreEqual(0, card1.CompareElement(card2.Element));
            Assert.AreEqual(0, card2.CompareElement(card1.Element));
        }
        
        [Test]
        public void Test_DamageAdjustment_NormalDamage()
        {
            var card1 = new Card("67f9048f-99b8-4ae4-b866-d8008d00c53d", "DarkSpell", 60);
            var card2 = new Card("70962948-2bf7-44a9-9ded-8c68eeac7793","FireElf", 70);

            Assert.AreEqual(60, card1.CompareElement(card2.Element));
            Assert.AreEqual(70, card2.CompareElement(card1.Element));
        }
        
        [Test]
        public void Test_GoblinVSDragon()
        {
            var card1 = new Card("67f9048f-99b8-4ae4-b866-d8008d00c53d", "WaterGoblin", 40);
            var card2 = new Card("70962948-2bf7-44a9-9ded-8c68eeac7793","FireDragon", 70);

            Assert.AreEqual("WaterGoblin is afraid of FireDragon!", card1.CheckEffect(card2));
        }
        
        [Test]
        public void Test_OrkVSWizard()
        {
            var card1 = new Card("67f9048f-99b8-4ae4-b866-d8008d00c53d", "Ork", 60);
            var card2 = new Card("70962948-2bf7-44a9-9ded-8c68eeac7793","Wizard", 50);

            Assert.AreEqual("Wizard is putting Ork under control!", card1.CheckEffect(card2));
        }
        
        [Test]
        public void Test_DragonVSFireElf()
        {
            var card1 = new Card("67f9048f-99b8-4ae4-b866-d8008d00c53d", "WaterDragon", 50);
            var card2 = new Card("70962948-2bf7-44a9-9ded-8c68eeac7793","FireElf", 45);

            Assert.AreEqual(Element.Fire, card2.Element);
            Assert.AreEqual("FireElf is able to evade WaterDragon's attack!", card1.CheckEffect(card2));
        }
        
        [Test]
        public void Test_KnightVSWaterSpell()
        {
            var card1 = new Card("67f9048f-99b8-4ae4-b866-d8008d00c53d", "FireKnight", 55);
            var card2 = new Card("70962948-2bf7-44a9-9ded-8c68eeac7793","WaterSpell", 65);

            Assert.AreEqual(Element.Water, card2.Element);
            Assert.AreEqual("FireKnight drowned in the WaterSpell!", card1.CheckEffect(card2));
        }
        
        [Test]
        public void Test_SpellVSKraken()
        {
            var card1 = new Card("67f9048f-99b8-4ae4-b866-d8008d00c53d", "RegularSpell", 60);
            var card2 = new Card("70962948-2bf7-44a9-9ded-8c68eeac7793","FireKraken", 70);

            Assert.AreEqual("FireKraken is immune against spells!", card1.CheckEffect(card2));
        }
        
        [Test]
        public void Test_VampireVSLightFairy()
        {
            var card1 = new Card("67f9048f-99b8-4ae4-b866-d8008d00c53d", "DarkVampire", 50);
            var card2 = new Card("70962948-2bf7-44a9-9ded-8c68eeac7793","LightFairy", 30);

            Assert.AreEqual("DarkVampire is going blind due to the brightness of LightFairy!", card1.CheckEffect(card2));
        }
    }
}
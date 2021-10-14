using System.Collections.Generic;
using SWEN1.MTCG.ClassLibrary;
using NUnit.Framework;

namespace SWEN1.MTCG.Test
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
            User user1 = new User("Jay", "12345", 50);
            user1.DeckCollection.Add(new Card("67f9048f-99b8-4ae4-b866-d8008d00c53d", "FireOrk", 10));
            user1.DeckCollection.Add(new Card("70962948-2bf7-44a9-9ded-8c68eeac7793","FireSpell", 90));

            foreach (var card in user1.DeckCollection)
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
        public void Test_MonsterEffect()
        {
            var card1 = new Card("67f9048f-99b8-4ae4-b866-d8008d00c53d", "FireOrk", 45);
            var card2 = new Card("70962948-2bf7-44a9-9ded-8c68eeac7793","WaterWizard", 70);

            Assert.True(card1.CheckEffect(card2));
        }
        
        [Test]
        public void Test_DamageAdjustment()
        {
            var card1 = new Card("67f9048f-99b8-4ae4-b866-d8008d00c53d", "FireSpell", 60);
            var card2 = new Card("70962948-2bf7-44a9-9ded-8c68eeac7793","WaterWizard", 70);

            Assert.AreEqual(30, card1.CompareElement(card2.Element));
            Assert.AreEqual(140, card2.CompareElement(card1.Element));
        }
    }
}
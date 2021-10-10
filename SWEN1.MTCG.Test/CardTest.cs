using System.Collections.Generic;
using Moq;
using SWEN1.MTCG.ClassLibrary;
using NUnit.Framework;

namespace SWEN1.MTCG.Test
{
    public class CardTest
    {
        [Test]
        public void Test_AssignMonsterCard()
        {
            MonsterCard card = new MonsterCard("845f0dc7-37d0-426e-994e-43fc3ac83c08","Knight", 15);
            Assert.AreEqual("Knight", card.Name);
            Assert.AreEqual(15, card.Damage);
            Assert.AreEqual(Element.Normal, card.Element);
            Assert.AreEqual(Monster.Knight, card.MonsterType);
        }
        
        [Test]
        public void Test_AssignSpellCard()
        {
            SpellCard card = new SpellCard("99f8f8dc-e25e-4a95-aa2c-782823f36e2a","WaterSpell", 75);
            Assert.AreEqual("WaterSpell", card.Name);
            Assert.AreEqual(75, card.Damage);
            Assert.AreEqual(Element.Water, card.Element);
        }
        
        [Test]
        public void Test_AssignUserWithStackAndDeck()
        {
            List<Card> cards = new List<Card>();

            cards.Add(new MonsterCard("67f9048f-99b8-4ae4-b866-d8008d00c53d", "FireOrk", 10));
            cards.Add(new SpellCard("70962948-2bf7-44a9-9ded-8c68eeac7793","FireSpell", 90));

            User user1 = new User("Jay", "12345", 50, cards);

            foreach (var card in user1.DeckCollection)
            {
                if (card is MonsterCard)
                {
                    var cardTmp = card as MonsterCard;
                    Assert.AreEqual("67f9048f-99b8-4ae4-b866-d8008d00c53d", cardTmp?.Id);
                    Assert.AreEqual("FireOrk", cardTmp?.Name);
                    Assert.AreEqual(10, cardTmp?.Damage);
                    Assert.AreEqual(Element.Fire, cardTmp?.Element);
                    Assert.AreEqual(Monster.Ork, cardTmp?.MonsterType);
                }
                else
                {
                    var cardTmp = card as SpellCard;
                    Assert.AreEqual("70962948-2bf7-44a9-9ded-8c68eeac7793", cardTmp?.Id);
                    Assert.AreEqual("FireSpell", cardTmp?.Name);
                    Assert.AreEqual(90, cardTmp?.Damage);
                    Assert.AreEqual(Element.Fire, cardTmp?.Element);
                }
            }
        }
    }
}
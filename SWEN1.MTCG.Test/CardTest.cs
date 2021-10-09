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
            MonsterCard card = new MonsterCard("Knight", 15, Element.Normal, Monster.Knight);
            Assert.AreEqual("Knight", card.Name);
            Assert.AreEqual(15, card.Damage);
            Assert.AreEqual(Element.Normal, card.Element);
            Assert.AreEqual(Monster.Knight, card.MonsterType);
        }
        
        [Test]
        public void Test_AssignSpellCard()
        {
            SpellCard card = new SpellCard("WaterSpell", 75, Element.Water);
            Assert.AreEqual("WaterSpell", card.Name);
            Assert.AreEqual(75, card.Damage);
            Assert.AreEqual(Element.Water, card.Element);
        }
        
        [Test]
        public void Test_AssignUserWithCard()
        {
            List<Card> cards = new List<Card>();

            cards.Add(new MonsterCard("FireOrk", 10, Element.Fire, Monster.Ork));
            cards.Add(new SpellCard("FireSpell", 90, Element.Fire));

            User user1 = new User("Jay", "12345", 50);
            user1.ChooseDeckCards(cards);

            for (int i = 0; i < cards.Count; i++)
            {
                if (user1.DeckCollection[i] is MonsterCard)
                {
                    var cardTmp = user1.DeckCollection[i] as MonsterCard;
                    Assert.AreEqual("FireOrk", cardTmp?.Name);
                    Assert.AreEqual(10, cardTmp?.Damage);
                    Assert.AreEqual(Element.Fire, cardTmp?.Element);
                    Assert.AreEqual(Monster.Ork, cardTmp?.MonsterType);
                }
                else
                {
                    var cardTmp = user1.DeckCollection[i] as SpellCard;
                    Assert.AreEqual("FireSpell", cardTmp?.Name);
                    Assert.AreEqual(90, cardTmp?.Damage);
                    Assert.AreEqual(Element.Fire, cardTmp?.Element);
                }
            }
        }
    }
}
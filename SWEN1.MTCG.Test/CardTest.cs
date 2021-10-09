using System.Collections.Generic;
using Moq;
using SWEN1.MTCG.ClassLibrary;
using Xunit;

namespace SWEN1.MTCG.Tests
{
    public class CardTest
    {
        [Fact]
        public void Test_AssignMonsterCard()
        {
            MonsterCard card = new MonsterCard("Knight", 15, Element.normal, Monster.Knight);
            Assert.Equal("Knight", card.name);
            Assert.Equal(15, card.damage);
            Assert.Equal(Element.normal, card.element);
            Assert.Equal(Monster.Knight, card.monsterType);
        }
        [Fact]
        public void Test_AssignSpellCard()
        {
            SpellCard card = new SpellCard("WaterSpell", 75, Element.water);
            Assert.Equal("WaterSpell", card.name);
            Assert.Equal(75, card.damage);
            Assert.Equal(Element.water, card.element);
        }

        [Fact]
        public void Test_AssignUserWithCard()
        {
            var cards = new List<Card>();

            cards.Add(new MonsterCard("FireOrk", 10, Element.fire, Monster.Ork));
            cards.Add(new SpellCard("FireSpell", 90, Element.fire));

            User user1 = new User("Jay", "12345", 50);
            user1.ChooseDeckCards(cards);

            for (int i = 0; i < cards.Count; i++)
            {
                if (user1.deckCollection[i] is MonsterCard)
                {
                    var cardTmp = user1.deckCollection[i] as MonsterCard;
                    Assert.Equal("FireOrk", cardTmp.name);
                    Assert.Equal(10, cardTmp.damage);
                    Assert.Equal(Element.fire, cardTmp.element);
                    Assert.Equal(Monster.Ork, cardTmp.monsterType);
                }
                else
                {
                    var cardTmp = user1.deckCollection[i] as SpellCard;
                    Assert.Equal("FireSpell", cardTmp.name);
                    Assert.Equal(90, cardTmp.damage);
                    Assert.Equal(Element.fire, cardTmp.element);
                }
            }
        }
    }
}
using System.Collections.Generic;
using SWEN1.MTCG.ClassLibrary;
using Xunit;

namespace SWEN1.MTCG.Test
{
    public class CardTest
    {
        [Fact]
        public void Test_AssignMonsterCard()
        {
            MonsterCard card = new MonsterCard("Knight", 15, Element.Normal, Monster.Knight);
            Assert.Equal("Knight", card.Name);
            Assert.Equal(15, card.Damage);
            Assert.Equal(Element.Normal, card.Element);
            Assert.Equal(Monster.Knight, card.MonsterType);
        }
        [Fact]
        public void Test_AssignSpellCard()
        {
            SpellCard card = new SpellCard("WaterSpell", 75, Element.Water);
            Assert.Equal("WaterSpell", card.Name);
            Assert.Equal(75, card.Damage);
            Assert.Equal(Element.Water, card.Element);
        }

        [Fact]
        public void Test_AssignUserWithCard()
        {
            var cards = new List<Card>();

            cards.Add(new MonsterCard("FireOrk", 10, Element.Fire, Monster.Ork));
            cards.Add(new SpellCard("FireSpell", 90, Element.Fire));

            var user1 = new User("Jay", "12345", 50);
            user1.ChooseDeckCards(cards);

            for (var i = 0; i < cards.Count; i++)
            {
                if (user1.DeckCollection[i] is MonsterCard)
                {
                    var cardTmp = user1.DeckCollection[i] as MonsterCard;
                    Assert.Equal("FireOrk", cardTmp?.Name);
                    Assert.Equal(10, cardTmp?.Damage);
                    Assert.Equal(Element.Fire, cardTmp?.Element);
                    Assert.Equal(Monster.Ork, cardTmp?.MonsterType);
                }
                else
                {
                    var cardTmp = user1.DeckCollection[i] as SpellCard;
                    Assert.Equal("FireSpell", cardTmp?.Name);
                    Assert.Equal(90, cardTmp?.Damage);
                    Assert.Equal(Element.Fire, cardTmp?.Element);
                }
            }
        }
    }
}
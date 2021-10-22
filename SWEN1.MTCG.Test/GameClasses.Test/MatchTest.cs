using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SWEN1.MTCG.ClassLibrary;
using Match = SWEN1.MTCG.ClassLibrary.Match;

namespace SWEN1.MTCG.Test
{
    public class MatchTest
    {
        [Test]
        public void Test_MonsterVsMonster()
        {
            var user1 = new Mock<IUser>();
            var user2 = new Mock<IUser>();
            
            var card1 = new Mock<ICard>();
            var card2 = new Mock<ICard>();

            var deck1 = new List<ICard>()
            {
                card1.Object
            };
            var deck2 = new List<ICard>()
            {
                card2.Object
            };

            card1.Setup(mock => mock.Type).Returns(Type.Ork);
            card2.Setup(mock => mock.Type).Returns(Type.Wizard);

            user1.Setup(mock => mock.Deck).Returns(deck1);
            user2.Setup(mock => mock.Deck).Returns(deck2);

            var game = new Mock<Match>(user1.Object, user2.Object);
            
            game.Object.BattleAction();

            card1.Verify(x => x.CompareElement(card2.Object.Element), Times.Never);
            card2.Verify(x => x.CompareElement(card1.Object.Element), Times.Never);
        }
        [Test]
        public void Test_SpellVsMonster()
        {
            var user1 = new Mock<IUser>();
            var user2 = new Mock<IUser>();
            
            var card1 = new Mock<ICard>();
            var card2 = new Mock<ICard>();

            var deck1 = new List<ICard>()
            {
                card1.Object
            };
            var deck2 = new List<ICard>()
            {
                card2.Object
            };

            card1.Setup(mock => mock.Type).Returns(Type.Spell);
            card2.Setup(mock => mock.Type).Returns(Type.Wizard);

            user1.Setup(mock => mock.Deck).Returns(deck1);
            user2.Setup(mock => mock.Deck).Returns(deck2);

            var game = new Mock<Match>(user1.Object, user2.Object);
            
            game.Object.BattleAction();

            card1.Verify(x => x.CompareElement(card2.Object.Element), Times.Once);
            card2.Verify(x => x.CompareElement(card1.Object.Element), Times.Once);
        }
    }
}
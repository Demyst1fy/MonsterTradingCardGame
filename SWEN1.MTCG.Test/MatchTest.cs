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
        public void Test_BattleAction()
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

            user1.Setup(mock => mock.DeckCollection).Returns(deck1);
            user2.Setup(mock => mock.DeckCollection).Returns(deck2);
           
            var game = new Match(user1.Object, user2.Object);
            
            game.BattleAction();
            
            card1.Verify(x => x.CompareCard(card2.Object), Times.AtLeastOnce);
            card2.Verify(x => x.CompareCard(card1.Object), Times.AtLeastOnce);
            
            card1.Verify(x => x.CompareElement(card2.Object.Element), Times.AtLeastOnce);
            card2.Verify(x => x.CompareElement(card1.Object.Element), Times.AtLeastOnce);
        }
    }
}
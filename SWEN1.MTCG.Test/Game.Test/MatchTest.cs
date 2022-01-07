using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SWEN1.MTCG.Game;
using SWEN1.MTCG.Game.Interfaces;
using Match = SWEN1.MTCG.Game.Match;

namespace SWEN1.MTCG.Test.Game.Test
{
    public class MatchTest
    {
        private Mock<IUser> _user1;
        private Mock<IUser> _user2;
        
        private Mock<ICard> _card1;
        private Mock<ICard> _card2;
        
        private List<ICard> _deck1;
        private List<ICard> _deck2;
        
        private Mock<ILogging> _logging;

        [SetUp]
        public void Init()
        {
            _user1 = new Mock<IUser>();
            _user2 = new Mock<IUser>();
            
            _card1 = new Mock<ICard>();
            _card2 = new Mock<ICard>();

            _deck1 = new List<ICard>() { _card1.Object };
            _deck2 = new List<ICard>() { _card2.Object };
            
            _logging = new Mock<ILogging>();
        }
        
        [Test]
        public void Test_MonsterVsMonster()
        {
            _card1.Setup(mock => mock.Type).Returns(Type.Ork);
            _card2.Setup(mock => mock.Type).Returns(Type.Wizard);

            _user1.Setup(mock => mock.Deck).Returns(_deck1);
            _user2.Setup(mock => mock.Deck).Returns(_deck2);
            
            IMatch game = new Match(_user1.Object, 1);
            game.AddUser(_user2.Object);
            game.BattleAction(_logging.Object);

            _card1.Verify(x => x.CompareElement(_card2.Object.Element), Times.Never);
            _card2.Verify(x => x.CompareElement(_card1.Object.Element), Times.Never);
        }
        
        [Test]
        public void Test_SpellVsMonster()
        {
            _card1.Setup(mock => mock.Type).Returns(Type.Spell);
            _card2.Setup(mock => mock.Type).Returns(Type.Wizard);

            _user1.Setup(mock => mock.Deck).Returns(_deck1);
            _user2.Setup(mock => mock.Deck).Returns(_deck2);

            int maxRounds = 100;
            
            IMatch game = new Match(_user1.Object, maxRounds);
            game.AddUser(_user2.Object);
            
            game.BattleAction(_logging.Object);

            _card1.Verify(x => x.CompareElement(_card2.Object.Element), Times.Exactly(maxRounds));
            _card2.Verify(x => x.CompareElement(_card1.Object.Element), Times.Exactly(maxRounds));
        }
    }
}
using System.Collections.Generic;
using Moq;
using SWEN1.MTCG.ClassLibrary;
using Xunit;

namespace SWEN1.MTCG.Test
{
    public class GameTest
    {
        [Fact]
        public void Test_battleAction()
        {
            var mockedPlayer1 = new Mock<User>("Jay", "12345", 50);
            var mockedPlayer2 = new Mock<User>("Marc", "54321", 60);
   
            var player1Cards = new List<Card>();
            var player2Cards = new List<Card>();

            player1Cards.Add(new MonsterCard("FireOrk", 10, Element.Fire, Monster.Ork));
            player1Cards.Add(new SpellCard("WaterSpell", 75, Element.Water));
            player2Cards.Add(new MonsterCard("Knight", 15, Element.Normal, Monster.Knight));
            player2Cards.Add(new SpellCard("FireSpell", 90, Element.Fire));

            mockedPlayer1.Verify(mock => mock.ChooseDeckCards(player1Cards), Times.Once);
            mockedPlayer2.Verify(mock => mock.ChooseDeckCards(player2Cards), Times.Once);
            
            var mockedGame = new Mock<Game>(mockedPlayer1.Object, mockedPlayer2.Object);
            
            mockedGame.Verify(mock => mock.BattleAction(), Times.AtLeastOnce);
        }
    }
}
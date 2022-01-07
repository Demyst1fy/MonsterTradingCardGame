using System.Collections.Generic;
using NUnit.Framework;
using SWEN1.MTCG.Game;
using SWEN1.MTCG.Game.Interfaces;

namespace SWEN1.MTCG.Test.Game.Test
{
    public class UserTest
    {
        [Test]
        public void Test_GetWinRate()
        {
            var user1 = new User("Jay", new List<ICard>(), new Stats(3, 1, 1, 109));
            Assert.AreEqual("Jay", user1.Username);
            Assert.AreEqual(3, user1.Stats.Wins);
            Assert.AreEqual(1, user1.Stats.Losses);
            Assert.AreEqual(1, user1.Stats.Draws);
            Assert.AreEqual(109, user1.Stats.Elo);
            Assert.AreEqual(60, user1.Stats.WinRate);
        }
    }
}
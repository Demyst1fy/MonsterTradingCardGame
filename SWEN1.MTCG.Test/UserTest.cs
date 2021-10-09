using NUnit.Framework;
using SWEN1.MTCG.ClassLibrary;

namespace SWEN1.MTCG.Test
{
    public class UserTest
    {
        [Test]
        public void Test_CallUser()
        {
            var user1 = new User("Jay", "12345", 50);
            Assert.AreEqual("Jay", user1.Username);
            Assert.AreEqual("12345", user1.Password);
            Assert.AreEqual(50, user1.Coins);
        }
    }
}
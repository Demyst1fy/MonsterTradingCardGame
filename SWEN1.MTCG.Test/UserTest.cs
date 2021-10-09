using SWEN1.MTCG.ClassLibrary;
using Xunit;

namespace SWEN1.MTCG.Test
{
    public class UserTest
    {
        [Fact]
        public void Test_CallUser()
        {
            var user1 = new User("Jay", "12345", 50);
            Assert.Equal("Jay", user1.Username);
            Assert.Equal("12345", user1.Password);
            Assert.Equal(50, user1.Coins);
        }
    }
}
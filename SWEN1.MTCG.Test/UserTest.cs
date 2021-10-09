using SWEN1.MTCG.ClassLibrary;
using Xunit;

namespace SWEN1.MTCG.Tests
{
    public class UserTest
    {
        [Fact]
        public void Test_CallUser()
        {
            User user1 = new User("Jay", "12345", 50);
            Assert.Equal("Jay", user1.username);
            Assert.Equal("12345", user1.password);
            Assert.Equal(50, user1.coins);
        }
    }
}
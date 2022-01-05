using NUnit.Framework;
using SWEN1.MTCG.Game;
using SWEN1.MTCG.Server;
using SWEN1.MTCG.Server.Interfaces;

namespace SWEN1.MTCG.Test.Server.Test
{
    public class ResponseTest
    {
        [Test]
        public void Test_ResponseMessage()
        {
            string responseBodyMessage = "You are now registered!";
            IResponse response = new Response(201, responseBodyMessage);
            
            Assert.AreEqual(201, response.Status);
            Assert.AreEqual("Created", response.Message);
            Assert.AreEqual("text/plain", response.MimeType);
            Assert.AreEqual(responseBodyMessage.Length, response.ContentLength);
            Assert.AreEqual("You are now registered!", response.Body);
        }
    }
}
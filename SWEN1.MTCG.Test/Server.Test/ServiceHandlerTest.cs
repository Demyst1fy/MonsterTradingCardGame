using NUnit.Framework;
using SWEN1.MTCG.Server;
using SWEN1.MTCG.Server.Interfaces;

namespace SWEN1.MTCG.Test.Server.Test
{
    public class ServiceHandlerTest
    {
        
        [Test]
        public void Test_ParseRequest()
        {
            IServiceHandler serviceHandler = new ServiceHandler();
            var data = "POST /users HTTP/1.1\r\n" +
                            "Host: localhost:10001\r\n" +
                            "User-Agent: curl/7.55.1\r\n" +
                            "Accept: */*\r\n" +
                            "Content-Type: application/json\r\n" +
                            "Content-Length: 44\r\n\r\n" +
                            "{\"Username\":\"kienboec\", \"Password\":\"daniel\"}";
            
            IRequest request = serviceHandler.ParseRequest(data);
            
            Assert.AreEqual("POST", request.Method);
            Assert.AreEqual("/users", request.Query);
            Assert.AreEqual("{\"Username\":\"kienboec\", \"Password\":\"daniel\"}", request.Content);
        }
    }
}
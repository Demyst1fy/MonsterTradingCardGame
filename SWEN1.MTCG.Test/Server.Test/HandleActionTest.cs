using System.Collections.Concurrent;
using Moq;
using NUnit.Framework;
using SWEN1.MTCG.Game.Interfaces;
using SWEN1.MTCG.Server;
using SWEN1.MTCG.Server.Interfaces;

namespace SWEN1.MTCG.Test.Server.Test
{
    public class HandleActionTest
    {
        private Mock<IHandleActions> _action;
        private IServiceHandler _serviceHandler;
        private ConcurrentQueue<IMatch> _allMatches;
        
        [SetUp]
        public void Init()
        {
            _action = new Mock<IHandleActions>();
            _serviceHandler = new ServiceHandler(_action.Object);
            _allMatches = new ConcurrentQueue<IMatch>();
        }
        
        [Test]
        public void Test_HandleRegistration()
        {
            IRequest request = new Request("POST", "/users", It.IsAny<string>());
            _serviceHandler.HandleRequest(request, ref _allMatches);
            _action.Verify(mock => mock.HandleRegistration(It.IsAny<string>()), Times.Once); // this should pass
            _action.Verify(mock => mock.HandleLogin(It.IsAny<string>()), Times.Never);
        }
        
        [Test]
        public void Test_HandleLogin()
        {
            IRequest request = new Request("POST", "/sessions", It.IsAny<string>());
            _serviceHandler.HandleRequest(request, ref _allMatches);
            _action.Verify(mock => mock.HandleRegistration(It.IsAny<string>()), Times.Never);
            _action.Verify(mock => mock.HandleLogin(It.IsAny<string>()), Times.Once); // this should pass
        }
        
        [Test]
        public void Test_HandleCreatePackage()
        {
            IRequest request = new Request("POST", "/packages", It.IsAny<string>());
            _serviceHandler.HandleRequest(request, ref _allMatches);
            _action.Verify(mock => mock.HandleCreatePackage(It.IsAny<string>(), It.IsAny<string>()), Times.Once); // this should pass
            _action.Verify(mock => mock.HandleShowTransactions(It.IsAny<string>()), Times.Never);
        }
        
        [Test]
        public void Test_HandleShowTransaction()
        {
            IRequest request = new Request("GET", "/transactions", It.IsAny<string>());
            _serviceHandler.HandleRequest(request, ref _allMatches);
            _action.Verify(mock => mock.HandleCreatePackage(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _action.Verify(mock => mock.HandleShowTransactions(It.IsAny<string>()), Times.Once); // this should pass
        }
        
        [Test]
        public void Test_HandleShowDeck()
        {
            IRequest request = new Request("GET", "/deck", It.IsAny<string>());
            _serviceHandler.HandleRequest(request, ref _allMatches);
            _action.Verify(mock => mock.HandleShowDeck(It.IsAny<string>()), Times.Once); // this should pass
            _action.Verify(mock => mock.HandleConfigureDeck(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
        
        [Test]
        public void Test_HandleConfigureDeck()
        {
            IRequest request = new Request("PUT", "/deck", It.IsAny<string>());
            _serviceHandler.HandleRequest(request, ref _allMatches);
            _action.Verify(mock => mock.HandleShowDeck(It.IsAny<string>()), Times.Never);
            _action.Verify(mock => mock.HandleConfigureDeck(It.IsAny<string>(), It.IsAny<string>()), Times.Once); // this should pass
        }
    }
}
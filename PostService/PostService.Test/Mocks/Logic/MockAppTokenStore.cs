using Moq;
using PostService.Logic.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PostService.Test.Mocks.Logic
{
    public class MockAppTokenStore : Mock<IAppTokenStore>
    {
        public void MockGetAppToken(string token)
        {
            Setup(c => c.GetAppTokenAsync()).Returns(Task.FromResult(token));
        }
    }
}

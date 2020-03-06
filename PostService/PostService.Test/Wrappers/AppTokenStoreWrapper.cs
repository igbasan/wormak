using PostService.Logic.Implementations;
using PostService.Logic.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PostService.Test.Wrappers
{
    public class AppTokenStoreWrapper : AppTokenStore
    {
        public AppTokenStoreWrapper(IHttpHandler httpHandler, string authUrl, string appID) : base(httpHandler, authUrl, appID)
        {
        }

        public void UpdateExpirationTime(DateTime time)
        {
            ExpirationTime = time;
        }
    }
}

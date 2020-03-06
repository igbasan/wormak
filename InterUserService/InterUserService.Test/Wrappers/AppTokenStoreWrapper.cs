using InterUserService.Logic.Implementation;
using InterUserService.Logic.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace InterUserService.Test.Wrappers
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

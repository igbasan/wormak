using AuthenticationService.WebAPI.Controllers;
using AuthenticationService.WebAPI.Logic.Interfaces;
using AuthenticationService.WebAPI.Models.Implementations;
using AuthenticationService.WebAPI.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.Test.Wrappers
{
    public class AccountControllerWrapper : AccountController
    {
        public AccountControllerWrapper(IConfiguration configuration, IUserLogic userLogic, IUserSessionLogic userSessionLogic, IHttpHandler client, ILogger<AccountController> logger)
            :base(configuration, userLogic, userSessionLogic, client, logger)
        {
        }
        public async override Task<IActionResult> HandleExternalLogin(User userDetails, string token)
        {
            return new JsonResult(new AuthToken { TokenType = "Bearer", AccessToken = ShaHashTool.Hash($"{userDetails.LoginServiceProvider}-{token}"), ExpiresIn = 24 * 3600 });
        }
    }
}

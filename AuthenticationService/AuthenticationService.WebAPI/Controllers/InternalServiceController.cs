using System;
using System.Threading.Tasks;
using AuthenticationService.WebAPI.Logic.Interfaces;
using AuthenticationService.WebAPI.Models.Implementations;
using AuthenticationService.WebAPI.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
//using static AuthenticationService.Startup;

namespace AuthenticationService.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class InternalServiceController : ControllerBase
    {
        private readonly IInternalServiceKeys internalServiceKeys;
        private readonly IInternalServiceSessionLogic internalServiceSessionLogic;

        public InternalServiceController(IInternalServiceKeys internalServiceKeys, IInternalServiceSessionLogic internalServiceSessionLogic)
        {
            this.internalServiceKeys = internalServiceKeys ?? throw new ArgumentNullException("internalServiceKeys");
            this.internalServiceSessionLogic = internalServiceSessionLogic ?? throw new ArgumentNullException("internalServiceSessionLogic");
        }

        [HttpGet]
        [Route("SignIn")]
        public async virtual Task<IActionResult> HandleExternalLogin(string appKey)
        {
            if (string.IsNullOrWhiteSpace(appKey)) return Unauthorized("Invalid appkey");

            string appName = string.Empty;
            if (!internalServiceKeys?.ServiceKeys?.TryGetValue(appKey, out appName) ?? true) return Unauthorized("Invalid appkey");
            

            InternalServiceSession session = await internalServiceSessionLogic.SetUpServiceSessionAsync(appName, appKey);

            return new JsonResult(new AuthToken { TokenType = "IntServ", AccessToken = session.AuthToken, ExpiresIn = 24 * 3600 });
        }

        [HttpGet]
        [Route("VaidateServiceToken")]
        public async Task<IActionResult> VaidateServiceToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return Unauthorized("Invalid Token");
            }

            InternalServiceSession session = await internalServiceSessionLogic.GetServiceSessionByTokenAsync(token);
            if (session == null)
            {
                return Unauthorized("Invalid Token");
            }
            if (session.ExpirationDate < DateTime.Now)
            {
                return Unauthorized("Session Expired, please login again.");
            }

            return new JsonResult(session.AppName);
        }
    }
}

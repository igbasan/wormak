using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using ProfileService.Logic.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProfileService
{
    public class TokenHandler : AuthorizationHandler<TokenRequirement>
    {
        private readonly ITokenValidator tokenValidator;
        private readonly IHttpContextAccessor httpContextAccessor;

        public TokenHandler(ITokenValidator tokenValidator, IHttpContextAccessor httpContextAccessor)
        {
            this.tokenValidator = tokenValidator;
            this.httpContextAccessor = httpContextAccessor;
        }
        protected async override Task HandleRequirementAsync(AuthorizationHandlerContext context, TokenRequirement requirement)
        {

            bool userIsAuthenticated = false;
            httpContextAccessor.HttpContext?.Request?.Headers?.TryGetValue("Authorization", out StringValues authTokenString);
            string authToken = string.Empty;
            bool isUserToken = authTokenString.ToString().StartsWith("Bearer ");
            bool isAppToken = authTokenString.ToString().StartsWith("IntApp ");

            if(isUserToken) authToken = authTokenString.ToString()?.Replace("Bearer ", "");
            if(isAppToken) authToken = authTokenString.ToString()?.Replace("IntApp ", "");

            if (!string.IsNullOrWhiteSpace(authToken))
            {
                if (isUserToken)
                {
                    var user = await tokenValidator.ValidateTokenAsync(authToken);
                    //user is authenticated if user is not null
                    userIsAuthenticated = user != null;

                    if (user != null)
                    {
                        var identity = new ClaimsIdentity();
                        identity.AddClaim(new Claim(ClaimTypes.GivenName, user.FirstName));
                        identity.AddClaim(new Claim(ClaimTypes.Surname, user.LastName));
                        identity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/id", user.Id));
                        identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
                        httpContextAccessor.HttpContext.User.AddIdentity(identity);
                    }
                }
                if (isAppToken)
                {
                    string appName = await tokenValidator.ValidateAppTokenAsync(authToken);
                    //user is authenticated if user is not null
                    userIsAuthenticated = !string.IsNullOrWhiteSpace(appName);
                }
            }

            if (userIsAuthenticated == false)
            {
                context.Fail();
            }
            if (userIsAuthenticated == true)
            {
                context.Succeed(requirement);
            }

        }
    }
}

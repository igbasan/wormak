using InterUserService.Logic.Interfaces;
using InterUserService.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InterUserService.Test.Wrappers
{
    public class TokenHandlerWrapper : TokenHandler
    {
        public TokenHandlerWrapper(ITokenValidator tokenValidator, IHttpContextAccessor httpContextAccessor) : base(tokenValidator, httpContextAccessor)
        {
        }

        public async Task HandleRequirementAsyncPublic(AuthorizationHandlerContext context, TokenRequirement requirement)
        {
            await HandleRequirementAsync(context, requirement);
        }
    }
}

using AuthenticationService.WebAPI.Models.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.WebAPI.Data.Interfaces
{
    public interface ILoginAttemptDAO
    {
        Task<LoginAttempt> SaveLoginAttemptAsync(LoginAttempt attempt);
    }
}

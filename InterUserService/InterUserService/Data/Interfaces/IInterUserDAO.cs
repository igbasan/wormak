using InterUserService.Models;
using InterUserService.Models.Implemetations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterUserService.Data.Interfaces
{
    public interface IInterUserDAO<T> where T : InterUser
    {

        Task<CountList<T>> GetAllByActiveProfileIDAsync(string profileID, int skip, int take);
        Task<CountList<T>> GetAllByPassiveProfileIDAsync(string profileID, int skip, int take);
        Task<T> CreateAsync(T interUser);
        Task<T> UpdateAsync(T interUser);
        Task<T> GetByActiveProfileIDandPassiveProfileIDAsync(string activeProfileID, string passiveProfileID);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using InterUserService.Logic.Interfaces;
using InterUserService.Models;
using InterUserService.Models.Exceptions;
using InterUserService.Models.Implemetations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InterUserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Token")]
    public class ClientController : ControllerBase
    {
        readonly IClientLogic clientLogic;
        public ClientController(IClientLogic clientLogic)
        {
            this.clientLogic = clientLogic ?? throw new ArgumentNullException("clientLogic");
        }

        [Route("GetClients")]
        public async Task<IActionResult> GetClients(int skip, int take)
        {
            ProfileType profileType;
            //get current profile details
            string profileId = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/profileId").Value;
            string profileTypeString = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/profileType").Value;
            Enum.TryParse(profileTypeString, out profileType);

            CountList<Profile> profiles = await clientLogic.GetAllByPassiveProfileIDAsync(profileId, profileType, skip, take);

            InterUserListResponse<Profile> response = new InterUserListResponse<Profile>
            {
                IsSuccessful = true,
                Message = "Successful",
                Result = profiles,
                TotalCount = profiles.TotalCount
            };
            return new JsonResult(response);
        }

        [Route("AddClient")]
        public async Task<IActionResult> AddClient(string profileID, string profileType)
        {
            return await ProcessClient(profileID, profileType, false);
        }


        [Route("RemoveClient")]
        public async Task<IActionResult> RemoveClient(string profileID, string profileType)
        {
            return await ProcessClient(profileID, profileType, true);
        }

        private async Task<IActionResult> ProcessClient(string profileID, string profileType, bool deactivate)
        {
            try
            {
                //validations
                if (string.IsNullOrWhiteSpace(profileID) || !Regex.IsMatch(profileID, @"^[0-9a-fA-F]+$"))
                {
                    ModelState.AddModelError("Result", "Invalid Profile Id Provided");
                    return BadRequest(ModelState);
                }
                ProfileType theProfileType, activeProfileType;
                if (string.IsNullOrWhiteSpace(profileType) || !Enum.TryParse(profileType, out theProfileType))
                {
                    ModelState.AddModelError("Result", "Invalid Profile Type Provided");
                    return BadRequest(ModelState);
                }

                //get current profile details
                string activeProfileId = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/profileId").Value;
                string activeProfileTypeString = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/profileType").Value;
                Enum.TryParse(activeProfileTypeString, out activeProfileType);

                Client theClient = new Client
                {
                    PassiveProfileID = profileID,
                    PassiveProfileType = theProfileType,
                    ActiveProfileID = activeProfileId,
                    ActiveProfileType = activeProfileType
                };
                //set up relationship
                theClient = await clientLogic.SetUpRelationshipAsync(theClient, deactivate);

                InterUserResponse response = new InterUserResponse
                {
                    IsSuccessful = true,
                    Message = "Successful"
                };
                return new JsonResult(response);
            }
            catch (InterUserException ex)
            {
                ModelState.AddModelError("Result", ex.Message);
                return BadRequest(ModelState);
            }
        }
    }
}
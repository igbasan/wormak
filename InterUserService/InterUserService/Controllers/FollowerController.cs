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
    public class FollowerController : ControllerBase
    {
        readonly IFollowerLogic followerLogic;
        public FollowerController(IFollowerLogic followerLogic)
        {
            this.followerLogic = followerLogic ?? throw new ArgumentNullException("followerLogic");
        }

        [Route("GetFollowers")]
        public async Task<IActionResult> GetFollowers(int skip, int take)
        {
            ProfileType profileType;
            //get current profile details
            string profileId = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/profileId").Value;
            string profileTypeString = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/profileType").Value;
            Enum.TryParse(profileTypeString, out profileType);

            CountList<Profile> profiles = await followerLogic.GetAllByPassiveProfileIDAsync(profileId, profileType, skip, take);

            InterUserListResponse<Profile> response = new InterUserListResponse<Profile>
            {
                IsSuccessful = true,
                Message = "Successful",
                Result = profiles,
                TotalCount = profiles.TotalCount
            };
            return new JsonResult(response);
        }
        
        [Route("GetAllFollowers")]
        public async Task<IActionResult> GetAllFollowers(string profileId, string profileType)
        {
            ProfileType theProfileType;
            if (string.IsNullOrWhiteSpace(profileId) || !Regex.IsMatch(profileId, @"^[0-9a-fA-F]+$"))
            {
                ModelState.AddModelError("Result", "Invalid Profile Id Provided");
                return BadRequest(ModelState);
            }
            if (string.IsNullOrWhiteSpace(profileType) || !Enum.TryParse(profileType, out theProfileType))
            {
                ModelState.AddModelError("Result", "Invalid Profile Type Provided");
                return BadRequest(ModelState);
            }
            //get current profile details
            Enum.TryParse(profileType, out theProfileType);

            //get all followers
            CountList<Profile> profiles = await followerLogic.GetAllByPassiveProfileIDAsync(profileId, theProfileType, 0, -1);

            InterUserListResponse<Profile> response = new InterUserListResponse<Profile>
            {
                IsSuccessful = true,
                Message = "Successful",
                Result = profiles,
                TotalCount = profiles.TotalCount
            };
            return new JsonResult(response);
        }

        [Route("GetFollowing")]
        public async Task<IActionResult> GetFollowing(int skip, int take)
        {
            ProfileType profileType;
            //get current profile details
            string profileId = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/profileId").Value;
            string profileTypeString = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/profileType").Value;
            Enum.TryParse(profileTypeString, out profileType);

            CountList<Profile> profiles = await followerLogic.GetAllByActiveProfileIDAsync(profileId, profileType, skip, take);

            InterUserListResponse<Profile> response = new InterUserListResponse<Profile>
            {
                IsSuccessful = true,
                Message = "Successful",
                Result = profiles,
                TotalCount = profiles.TotalCount
            };
            return new JsonResult(response);
        }
        [Route("GetAllFollowing")]
        public async Task<IActionResult> GetAllFollowing(string profileId, string profileType)
        {
            ProfileType theProfileType;
            if (string.IsNullOrWhiteSpace(profileId) || !Regex.IsMatch(profileId, @"^[0-9a-fA-F]+$"))
            {
                ModelState.AddModelError("Result", "Invalid Profile Id Provided");
                return BadRequest(ModelState);
            }
            if (string.IsNullOrWhiteSpace(profileType) || !Enum.TryParse(profileType, out theProfileType))
            {
                ModelState.AddModelError("Result", "Invalid Profile Type Provided");
                return BadRequest(ModelState);
            }
            //get current profile details
            Enum.TryParse(profileType, out theProfileType);

            //get all followers
            CountList<Profile> profiles = await followerLogic.GetAllByActiveProfileIDAsync(profileId, theProfileType, 0, -1);

            InterUserListResponse<Profile> response = new InterUserListResponse<Profile>
            {
                IsSuccessful = true,
                Message = "Successful",
                Result = profiles,
                TotalCount = profiles.TotalCount
            };
            return new JsonResult(response);
        }
        [Route("Follow")]
        public async Task<IActionResult> Follow(string profileID, string profileType)
        {
            return await ProcessFollowStatus(profileID, profileType, false);
        }

        [Route("Unfollow")]
        public async Task<IActionResult> Unfollow(string profileID, string profileType)
        {
            return await ProcessFollowStatus(profileID, profileType, true);
        }

        private async Task<IActionResult> ProcessFollowStatus(string profileID, string profileType, bool deactivate)
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

                Follower theFollower = new Follower
                {
                    PassiveProfileID = profileID,
                    PassiveProfileType = theProfileType,
                    ActiveProfileID = activeProfileId,
                    ActiveProfileType = activeProfileType
                };
                //set up relationship
                theFollower = await followerLogic.SetUpRelationshipAsync(theFollower, deactivate);

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

        [Route("CheckFollowed")]
        public async Task<IActionResult> CheckFollowed(string profileID, string profileType)
        {
            //validations
            if (string.IsNullOrWhiteSpace(profileID) || !Regex.IsMatch(profileID, @"^[0-9a-fA-F]+$"))
            {
                ModelState.AddModelError("Result", "Invalid Profile Id Provided");
                return BadRequest(ModelState);
            }
            ProfileType passiveProfileType, theProfileType;
            if (string.IsNullOrWhiteSpace(profileType) || !Enum.TryParse(profileType, out theProfileType))
            {
                ModelState.AddModelError("Result", "Invalid Profile Type Provided");
                return BadRequest(ModelState);
            }

            //get current profile details
            string passiveProfileId = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/profileId").Value;
            string passiveProfileTypeString = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/profileType").Value;
            Enum.TryParse(passiveProfileTypeString, out passiveProfileType);

            Follower follower = await followerLogic.GetByActiveProfileIDandPassiveProfileIDAsync(profileID, theProfileType, passiveProfileId, passiveProfileType);

            bool isFollowed = follower?.IsActive ?? false;

            InterUserBoolResponse response = new InterUserBoolResponse
            {
                IsSuccessful = true,
                Message = "Successful",
                Result = isFollowed
            };
            return new JsonResult(response);
        }

        [Route("CheckFollowing")]
        public async Task<IActionResult> CheckFollowing(string profileID, string profileType)
        {
            //validations
            if (string.IsNullOrWhiteSpace(profileID) || !Regex.IsMatch(profileID, @"^[0-9a-fA-F]+$"))
            {
                ModelState.AddModelError("Result", "Invalid Profile Id Provided");
                return BadRequest(ModelState);
            }
            ProfileType activeProfileType, theProfileType;
            if (string.IsNullOrWhiteSpace(profileType) || !Enum.TryParse(profileType, out theProfileType))
            {
                ModelState.AddModelError("Result", "Invalid Profile Type Provided");
                return BadRequest(ModelState);
            }

            //get current profile details
            string activeProfileId = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/profileId").Value;
            string activeProfileTypeString = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/profileType").Value;
            Enum.TryParse(activeProfileTypeString, out activeProfileType);

            Follower follower = await followerLogic.GetByActiveProfileIDandPassiveProfileIDAsync(activeProfileId, activeProfileType, profileID, theProfileType);

            bool isFollowing = follower?.IsActive ?? false;

            InterUserBoolResponse response = new InterUserBoolResponse
            {
                IsSuccessful = true,
                Message = "Successful",
                Result = isFollowing
            };
            return new JsonResult(response);
        }
    }
}
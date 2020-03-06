using ProfileService.Logic.Interfaces;
using ProfileService.Models.Implementations;
using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ProfileService.Models.Exceptions;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace ProfileService.Controllers
{
    [Authorize(Policy = "Token")]
    [Route("api/[controller]")]
    [ApiController]
    public class GeneralUserController : ControllerBase
    {
        readonly IGeneralUserLogic generalUserLogic;
        public GeneralUserController(IGeneralUserLogic generalUserLogic)
        {
            this.generalUserLogic = generalUserLogic ?? throw new ArgumentNullException("generalUserLogic");
        }

        [HttpPost]
        [Route("Setup")]
        public async Task<IActionResult> SetupGeneralUser([FromBody]GeneralUser profile)
        {
            try
            {
                if (profile == null || !ModelState.IsValid)
                {
                    if (profile == null) ModelState.AddModelError("Result", "No Payload was sent");
                    return BadRequest(ModelState);
                }

                profile.UserId = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/id").Value;
                profile = await generalUserLogic.SetupProfileAsync(profile);

                ProfileResponse response = new ProfileResponse
                {
                    IsSuccessful = true,
                    Message = "Successful"
                };
                return new JsonResult(response);
            }
            catch (ProfileServiceException ex)
            {
                ModelState.AddModelError("Result", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpGet]
        [Route("GetByID")]
        public async Task<IActionResult> GetGeneralUserDetails(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || !Regex.IsMatch(id, @"^[0-9a-fA-F]+$"))
            {
                ModelState.AddModelError("Result", "Invalid Id Provided");
                return BadRequest(ModelState);
            }

            var profile = await generalUserLogic.GetProfileByIDAsync(id);
            ProfileObjectResponse<GeneralUser> response = new ProfileObjectResponse<GeneralUser>
            {
                IsSuccessful = true,
                Message = "Successful",
                Result = profile
            };
            return new JsonResult(response); throw new NotImplementedException();
        }

        [HttpPost]
        [Route("Update")]
        public async Task<IActionResult> UpdateGeneralUser([FromBody]GeneralUser profile)
        {
            try
            {
                if (profile == null || !ModelState.IsValid)
                {
                    if (profile == null) ModelState.AddModelError("Result", "No Payload was sent");
                    return BadRequest(ModelState);
                }

                profile.UserId = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/id").Value;
                profile = await generalUserLogic.UpdateProfileAsync(profile);

                ProfileResponse response = new ProfileResponse
                {
                    IsSuccessful = true,
                    Message = "Successful"
                };
                return new JsonResult(response);
            }
            catch (ProfileServiceException ex)
            {
                ModelState.AddModelError("Result", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpGet]
        [Route("GetUserProfile")]
        public async Task<IActionResult> GetGeneralUserDetails()
        {
            string userID = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/id").Value;

            var profiles = await generalUserLogic.GetAllProfilesAsync(userID);
            ProfileObjectResponse<GeneralUser> response = new ProfileObjectResponse<GeneralUser>
            {
                IsSuccessful = true,
                Message = "Successful",
                Result = profiles.FirstOrDefault()
            };
            return new JsonResult(response);
        }

        [HttpGet]
        [Route("GetAllByIDs")]
        public async Task<IActionResult> GetAllGeneralUsersByProfileIDs([FromBody]List<string> profileIDs)
        {
            if (profileIDs == null || profileIDs.Count == 0 || profileIDs.Any(v => string.IsNullOrWhiteSpace(v) || !Regex.IsMatch(v, @"^[0-9a-fA-F]+$")))
            {
                ModelState.AddModelError("Result", "Invalid Id Provided");
                return BadRequest(ModelState);
            }
            var profiles = await generalUserLogic.GetAllProfilesByIDsAsync(profileIDs);
            ProfileListResponse<GeneralUser> response = new ProfileListResponse<GeneralUser>
            {
                IsSuccessful = true,
                Message = "Successful",
                Result = profiles
            };
            return new JsonResult(response);
        }
    }
}

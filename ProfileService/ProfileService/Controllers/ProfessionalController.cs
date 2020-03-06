using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProfileService.Logic.Interfaces;
using ProfileService.Models.Exceptions;
using ProfileService.Models.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProfileService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Token")]
    public class ProfessionalController : ControllerBase
    {
        readonly IProfessionalLogic professionalLogic;
        public ProfessionalController(IProfessionalLogic professionalLogic)
        {
            this.professionalLogic = professionalLogic ?? throw new ArgumentNullException("professionalLogic");
        }

        [HttpPost]
        [Route("Setup")]
        public async Task<IActionResult> SetupProfessional([FromBody]Professional profile)
        {
            try
            {
                if (profile == null || !ModelState.IsValid)
                {
                    if (profile == null) ModelState.AddModelError("Result", "No Payload was sent");
                    return BadRequest(ModelState);
                }

                profile.UserId = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/id").Value;
                profile = await professionalLogic.SetupProfileAsync(profile);

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
        public async Task<IActionResult> GetProfessionalDetails(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || !Regex.IsMatch(id, @"^[0-9a-fA-F]+$"))
            {
                ModelState.AddModelError("Result", "Invalid Id Provided");
                return BadRequest(ModelState);
            }

            var profile = await professionalLogic.GetProfileByIDAsync(id);
            ProfileObjectResponse<Professional> response = new ProfileObjectResponse<Professional>
            {
                IsSuccessful = true,
                Message = "Successful",
                Result = profile
            };
            return new JsonResult(response);
        }

        [HttpPost]
        [Route("Update")]
        public async Task<IActionResult> UpdateProfessional([FromBody]Professional profile)
        {
            try
            {
                if (profile == null || !ModelState.IsValid)
                {
                    if (profile == null) ModelState.AddModelError("Result", "No Payload was sent");
                    return BadRequest(ModelState);
                }

                profile.UserId = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/id").Value;
                profile = await professionalLogic.UpdateProfileAsync(profile);

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
        [Route("GetAll")]
        public async Task<IActionResult> GetAllProfessionalsByUser()
        {
            string userID = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/id").Value;

            var profiles = await professionalLogic.GetAllProfilesAsync(userID);
            ProfileListResponse<Professional> response = new ProfileListResponse<Professional>
            {
                IsSuccessful = true,
                Message = "Successful",
                Result = profiles
            };
            return new JsonResult(response);
        }

        [HttpGet]
        [Route("GetAllByIDs")]
        public async Task<IActionResult> GetAllProfessionalsByProfileIDs([FromBody]List<string> profileIDs)
        {
            if (profileIDs == null || profileIDs.Count == 0 || profileIDs.Any(v => string.IsNullOrWhiteSpace(v) || !Regex.IsMatch(v, @"^[0-9a-fA-F]+$")))
            {
                ModelState.AddModelError("Result", "Invalid Id Provided");
                return BadRequest(ModelState);
            }
            var profiles = await professionalLogic.GetAllProfilesByIDsAsync(profileIDs);
            ProfileListResponse<Professional> response = new ProfileListResponse<Professional>
            {
                IsSuccessful = true,
                Message = "Successful",
                Result = profiles
            };
            return new JsonResult(response);
        }
    }
}

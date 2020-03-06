using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProfileService.Logic.Interfaces;
using ProfileService.Models;
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
    public class ProfileController : ControllerBase
    {
        readonly ICompanyLogic companyLogic;
        readonly IGeneralUserLogic generalUserLogic;
        readonly IProfessionalLogic professionalLogic;
        readonly ICurrentProfileLogic currentProfileLogic;
        public ProfileController(ICompanyLogic companyLogic, IGeneralUserLogic generalUserLogic, IProfessionalLogic professionalLogic, ICurrentProfileLogic currentProfileLogic)
        {
            this.companyLogic = companyLogic ?? throw new ArgumentNullException("companyLogic");
            this.generalUserLogic = generalUserLogic ?? throw new ArgumentNullException("generalUserLogic");
            this.professionalLogic = professionalLogic ?? throw new ArgumentNullException("professionalLogic");
            this.currentProfileLogic = currentProfileLogic ?? throw new ArgumentNullException("currentProfileLogic");
        }

        [HttpPost]
        [Route("SetCurrentProfile")]
        public async Task<IActionResult> SetCurrentProfile([FromBody]CurrentProfile currentProfile)
        {
            try
            {
                if (currentProfile == null || !ModelState.IsValid)
                {
                    if (currentProfile == null) ModelState.AddModelError("Result", "No Payload was sent");
                    return BadRequest(ModelState);
                }

                currentProfile.UserID = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/id").Value;
                currentProfile = await currentProfileLogic.SetCurrentProfileAsync(currentProfile.UserID, currentProfile.ProfileID, currentProfile.ProfileType);

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

        [Route("GetCurrentProfile")]
        public async Task<IActionResult> GetCurrentProfile()
        {
            try
            {
                string userID = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/id").Value;

                var currentProfile = await currentProfileLogic.GetCurrentProfileAsync(userID);
                ProfileObjectResponse<ProfileLite> response = new ProfileObjectResponse<ProfileLite>
                {
                    IsSuccessful = true,
                    Message = "Successful",
                    Result = new ProfileLite { Id = currentProfile.ProfileID, Name = currentProfile.Name, ProfileType = currentProfile.ProfileType }
                };
                return new JsonResult(response);
            }
            catch (ProfileServiceException ex)
            {
                ModelState.AddModelError("Result", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [Route("GetCurrentInterestProfile")]
        public async Task<IActionResult> GetCurrentInterestProfile()
        {
            try
            {
                string userID = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/id").Value;

                var currentProfile = await currentProfileLogic.GetCurrentProfileAsync(userID);
                ProfileObjectResponse<InterestProfileLite> response = new ProfileObjectResponse<InterestProfileLite>
                {
                    IsSuccessful = true,
                    Message = "Successful",
                    Result = new InterestProfileLite
                    {
                        Id = currentProfile.ProfileID,
                        Name = currentProfile.Name,
                        ProfileType = currentProfile.ProfileType,
                        Interests = currentProfile.Interests ?? new List<Interest>()
                    }
                };
                return new JsonResult(response);
            }
            catch (ProfileServiceException ex)
            {
                ModelState.AddModelError("Result", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPost]
        [Route("GetProfiles")]
        public async Task<IActionResult> GetProfiles([FromBody] List<ProfileLite> profiles)
        {
            if (profiles == null || profiles.Count == 0 || profiles.Any(v => string.IsNullOrWhiteSpace(v.Id) || !Regex.IsMatch(v.Id, @"^[0-9a-fA-F]+$")))
            {
                ModelState.AddModelError("Result", "Invalid Id(s) Provided");
                return BadRequest(ModelState);
            }
            List<Profile> profileList = new List<Profile>();
            //get companies
            var companyProfileIds = profiles.Where(c => c.ProfileType == Models.ProfileType.Company).Select(c => c.Id).ToList();
            if (companyProfileIds.Count > 0)
            {
                var companies = await companyLogic.GetAllProfilesByIDsAsync(companyProfileIds);
                if (companies?.Count > 0) profileList.AddRange(companies);
            }

            //get general Users
            var generalUserProfileIds = profiles.Where(c => c.ProfileType == Models.ProfileType.GeneralUser).Select(c => c.Id).ToList();
            if (generalUserProfileIds.Count > 0)
            {
                var generalUsers = await generalUserLogic.GetAllProfilesByIDsAsync(generalUserProfileIds);
                if (generalUsers?.Count > 0) profileList.AddRange(generalUsers);
            }

            //get professionals
            var professionalProfileIds = profiles.Where(c => c.ProfileType == Models.ProfileType.Professional).Select(c => c.Id).ToList();
            if (professionalProfileIds.Count > 0)
            {
                var professionals = await professionalLogic.GetAllProfilesByIDsAsync(professionalProfileIds);
                if (professionals?.Count > 0) profileList.AddRange(professionals);
            }

            List<ProfileLite> result = profileList.Select(v => new ProfileLite { Id = v.Id, Name = v.Name, ProfileType = v.ProfileType }).ToList();
            ProfileListResponse<ProfileLite> response = new ProfileListResponse<ProfileLite>
            {
                IsSuccessful = true,
                Message = "Successful",
                Result = result
            };
            return new JsonResult(response);
        }

        [HttpPost]
        [Route("GetInterests")]
        public async Task<IActionResult> GetInterests([FromBody] List<Interest> interests)
        {
            if (interests == null || interests.Count == 0)
            {
                ModelState.AddModelError("Result", "Invalid Interest(s) Provided");
                return BadRequest(ModelState);
            }
            List<Profile> profileList = new List<Profile>();
            //get companies
            var companies = await companyLogic.GetAllProfilesByInterestsAsync(interests);
            if (companies?.Count > 0) profileList.AddRange(companies);

            //get general Users
            var generalUsers = await generalUserLogic.GetAllProfilesByInterestsAsync(interests);
            if (generalUsers?.Count > 0) profileList.AddRange(generalUsers);


            //get professionals
            var professionals = await professionalLogic.GetAllProfilesByInterestsAsync(interests);
            if (professionals?.Count > 0) profileList.AddRange(professionals);

            List<InterestProfileLite> result = profileList.Select(v => new InterestProfileLite
            {
                Id = v.Id,
                Name = v.Name,
                ProfileType = v.ProfileType,
                Interests = v.Interests
            }).ToList();

            ProfileListResponse<InterestProfileLite> response = new ProfileListResponse<InterestProfileLite>
            {
                IsSuccessful = true,
                Message = "Successful",
                Result = result
            };
            return new JsonResult(response);
        }
    }
}

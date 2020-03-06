using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProfileService.Logic.Interfaces;
using ProfileService.Models.Exceptions;
using ProfileService.Models.Implementations;

namespace ProfileService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Token")]
    public class CompanyController : ControllerBase
    {
        readonly ICompanyLogic companyLogic;
        public CompanyController(ICompanyLogic companyLogic)
        {
            this.companyLogic = companyLogic ?? throw new ArgumentNullException("companyLogic");
        }

        [HttpPost]
        [Route("Setup")]
        public async Task<IActionResult> SetupCompany([FromBody]Company profile)
        {
            try
            {
                if (profile == null || !ModelState.IsValid)
                {
                    if (profile == null) ModelState.AddModelError("Result", "No Payload was sent");
                    return BadRequest(ModelState);
                }

                profile.UserId = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/id").Value;
                profile = await companyLogic.SetupProfileAsync(profile);

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
        public async Task<IActionResult> GetCompanyDetails(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || !Regex.IsMatch(id, @"^[0-9a-fA-F]+$"))
            {
                ModelState.AddModelError("Result", "Invalid Id Provided");
                return BadRequest(ModelState);
            }

            var profile = await companyLogic.GetProfileByIDAsync(id);
            ProfileObjectResponse<Company> response = new ProfileObjectResponse<Company>
            {
                IsSuccessful = true,
                Message = "Successful",
                Result = profile
            };
            return new JsonResult(response);
        }

        [HttpPost]
        [Route("Update")]
        public async Task<IActionResult> UpdateCompany([FromBody]Company profile)
        {
            try
            {
                if (profile == null || !ModelState.IsValid)
                {
                    if (profile == null) ModelState.AddModelError("Result", "No Payload was sent");
                    return BadRequest(ModelState);
                }

                profile.UserId = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/id").Value;
                profile = await companyLogic.UpdateProfileAsync(profile);

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
        public async Task<IActionResult> GetAllCompaniesByUser()
        {
            string userID = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/id").Value;

            var profiles = await companyLogic.GetAllProfilesAsync(userID);
            ProfileListResponse<Company> response = new ProfileListResponse<Company>
            {
                IsSuccessful = true,
                Message = "Successful",
                Result = profiles
            };
            return new JsonResult(response);
        }

        [HttpGet]
        [Route("GetAllByIDs")]
        public async Task<IActionResult> GetAllCompaniesByProfileIDs([FromBody]List<string> profileIDs)
        {
            if (profileIDs == null || profileIDs.Count == 0 || profileIDs.Any(v => string.IsNullOrWhiteSpace(v) || !Regex.IsMatch(v, @"^[0-9a-fA-F]+$")))
            {
                ModelState.AddModelError("Result", "Invalid Id Provided");
                return BadRequest(ModelState);
            }
            var profiles = await companyLogic.GetAllProfilesByIDsAsync(profileIDs);
            ProfileListResponse<Company> response = new ProfileListResponse<Company>
            {
                IsSuccessful = true,
                Message = "Successful",
                Result = profiles
            };
            return new JsonResult(response);
        }
    }
}
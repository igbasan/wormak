using InterUserService.Logic.Interfaces;
using InterUserService.Models;
using InterUserService.Models.Exceptions;
using InterUserService.Models.Implemetations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace InterUserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Token")]
    public class TestimonialController : ControllerBase
    {
        readonly ITestimonialLogic testimonialLogic;
        public TestimonialController(ITestimonialLogic testimonialLogic)
        {
            this.testimonialLogic = testimonialLogic ?? throw new ArgumentNullException("testimonialLogic");
        }

        [Route("GetTestimonials")]
        public async Task<IActionResult> GetTestimonials(int skip, int take)
        {
            ProfileType profileType;
            //get current profile details
            string profileId = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/profileId").Value;
            string profileTypeString = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/profileType").Value;
            Enum.TryParse(profileTypeString, out profileType);

            CountList<Profile> profiles = await testimonialLogic.GetAllByPassiveProfileIDAsync(profileId, profileType, skip, take);

            InterUserListWithRatingResponse<Profile> response = new InterUserListWithRatingResponse<Profile>
            {
                IsSuccessful = true,
                Message = "Successful",
                Result = profiles,
                TotalCount = profiles.TotalCount,
                AverageRating = profiles.AverageRating
            };
            return new JsonResult(response);
        }

        [HttpPost]
        [Route("AddTestimonial")]
        public async Task<IActionResult> AddTestimonial([FromBody]TestimonialRequest testimonial)
        {
            try
            {
                if (testimonial == null || !ModelState.IsValid)
                {
                    if (testimonial == null) ModelState.AddModelError("Result", "No Payload was sent");
                    return BadRequest(ModelState);
                }

                //get current profile details
                ProfileType theProfileType;
                string profileId = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/profileId").Value;
                string profileTypeString = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/profileType").Value;
                Enum.TryParse(profileTypeString, out theProfileType);

                Testimonial theTestimonial = new Testimonial
                {
                    PassiveProfileID = testimonial.ProfileId,
                    PassiveProfileType = testimonial.ProfileType,
                    ActiveProfileID = profileId,
                    ActiveProfileType = theProfileType,
                    Comment = testimonial.Comment,
                    Rating = testimonial.Rating
                };
                //set up relationship
                theTestimonial = await testimonialLogic.SetUpRelationshipAsync(theTestimonial, false);

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

        [Route("RemoveTestimonial")]
        public async Task<IActionResult> RemoveTestimonial(string profileID, string profileType)
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

                Testimonial theTestimonial = new Testimonial
                {
                    PassiveProfileID = profileID,
                    PassiveProfileType = theProfileType,
                    ActiveProfileID = activeProfileId,
                    ActiveProfileType = activeProfileType
                };
                //set up relationship
                theTestimonial = await testimonialLogic.SetUpRelationshipAsync(theTestimonial, true);

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

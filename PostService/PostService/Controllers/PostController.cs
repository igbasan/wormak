using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostService.Logic.Interfaces;
using PostService.Models;
using PostService.Models.Exceptions;
using PostService.Models.Implementations;
using PostService.Models.Implementations.Requests;
using PostService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PostService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Token")]
    public class PostController : ControllerBase
    {
        readonly IPostLogic postLogic;
        public PostController(IPostLogic postLogic)
        {
            this.postLogic = postLogic ?? throw new ArgumentNullException("postLogic");
        }

        [Route("DoPost")]
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody]PostNewRequest post)
        {
            try
            {
                if (post == null || !ModelState.IsValid)
                {
                    if (post == null) ModelState.AddModelError("Result", "No Payload was sent");
                    return BadRequest(ModelState);
                }
                //get current user details
                string profileId = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/profileId").Value;
                string profileTypeString = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/profileType").Value;
                Enum.TryParse(profileTypeString, out ProfileType profileType);

                post.ProfileID = profileId;
                post.ProfileType = profileType;

                Post postItem = await postLogic.PostAsync(post);
                PostResponse response = new PostResponse
                {
                    IsSuccessful = true,
                    Message = "Successful"
                };
                return new JsonResult(response);
            }
            catch (PostServiceException ex)
            {
                ModelState.AddModelError("Result", ex.Message);
                return BadRequest(ModelState);
            }

        }
        [Route("Update")]
        [HttpPost]
        public async Task<IActionResult> UpdatePostAsync([FromBody]PostRequest post)
        {
            try
            {
                if (post == null || !ModelState.IsValid)
                {
                    if (post == null) ModelState.AddModelError("Result", "No Payload was sent");
                    return BadRequest(ModelState);
                }
                //get current user details
                string profileId = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/profileId").Value;
                string profileTypeString = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/profileType").Value;
                Enum.TryParse(profileTypeString, out ProfileType profileType);

                post.ProfileID = profileId;
                post.ProfileType = profileType;

                Post postItem = await postLogic.UpdatePostAsync(post);
                PostResponse response = new PostResponse
                {
                    IsSuccessful = true,
                    Message = "Successful"
                };
                return new JsonResult(response);
            }
            catch (PostServiceException ex)
            {
                ModelState.AddModelError("Result", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [Route("Get")]
        public async Task<IActionResult> GetDetailsByIDAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || !Regex.IsMatch(id, @"^[0-9a-fA-F]+$"))
            {
                ModelState.AddModelError("Result", "Invalid Post Id Provided");
                return BadRequest(ModelState);
            }
            Post postItem = await postLogic.GetByIDAsync(id);
            PostObjectResponse<PostResult> response = new PostObjectResponse<PostResult>
            {
                IsSuccessful = true,
                Message = "Successful",
                Result = postItem?.ToResult()
            };
            return new JsonResult(response);
        }

        [Route("Comment")]
        [HttpPost]
        public async Task<IActionResult> CommentAsync([FromBody]CommentRequest request)
        {
            try
            {
                if (request == null || !ModelState.IsValid)
                {
                    if (request == null) ModelState.AddModelError("Result", "No Payload was sent");
                    return BadRequest(ModelState);
                }
                //get current user details
                string profileId = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/profileId").Value;
                string profileTypeString = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/profileType").Value;
                Enum.TryParse(profileTypeString, out ProfileType profileType);

                request.ProfileID = profileId;
                request.ProfileType = profileType;

                Comment commentItem = await postLogic.CommentAsync(request);
                PostResponse response = new PostResponse
                {
                    IsSuccessful = true,
                    Message = "Successful"
                };
                return new JsonResult(response);
            }
            catch (PostServiceException ex)
            {
                ModelState.AddModelError("Result", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [Route("Like")]
        [HttpPost]
        public async Task<IActionResult> LikeAsync([FromBody]Request request)
        {
            try
            {
                if (request == null || !ModelState.IsValid)
                {
                    if (request == null) ModelState.AddModelError("Result", "No Payload was sent");
                    return BadRequest(ModelState);
                }
                //get current user details
                string profileId = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/profileId").Value;
                string profileTypeString = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/profileType").Value;
                Enum.TryParse(profileTypeString, out ProfileType profileType);

                request.ProfileID = profileId;
                request.ProfileType = profileType;

                Like likeItem = await postLogic.LikeAsync(request);
                PostResponse response = new PostResponse
                {
                    IsSuccessful = true,
                    Message = "Successful"
                };
                return new JsonResult(response);
            }
            catch (PostServiceException ex)
            {
                ModelState.AddModelError("Result", ex.Message);
                return BadRequest(ModelState);
            }
        }
        [Route("GetFeed")]
        public async Task<IActionResult> GetPostFeedAsync(int skip, int take)
        {
            //get current user details
            string profileId = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/profileId").Value;
            string profileTypeString = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/profileType").Value;
            string interestsString = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/interests").Value;

            Enum.TryParse(profileTypeString, out ProfileType profileType);
            List<Interest> interests = JsonSerializer.Deserialize<List<Interest>>(interestsString);

            CountList<Post> posts = await postLogic.GetPostFeedAsync(profileId, profileType, interests, skip, take);
            PostListResponse<PostResult> response = new PostListResponse<PostResult>
            {
                IsSuccessful = true,
                Message = "Successful",
                Result = posts?.Select(v => v.ToResult()).ToList() ?? new List<PostResult>(),
                TotalCount = posts?.TotalCount ?? 0
            };
            return new JsonResult(response);
        }
    }
}

using Moq;
using PostService.Data.Interfaces;
using PostService.Models;
using PostService.Models.Implementations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PostService.Test.Mocks.Data
{
    public class MockPostDAO : Mock<IPostDAO>
    {
        public bool PostCreated { get; set; }
        public bool PostUpdated { get; set; }
        public void MockSave()
        {
            Setup(x => x.SaveAsync(
                It.IsAny<Post>(),
                It.Is<List<string>>(v => v != null && v.Count > 0)
                )).Callback<Post, List<string>>((c,list) => PostCreated = true)
                .Returns<Post, List<string>>((s, list) => Task.FromResult<Post>(s));
        }
        public void MockUpdate(bool allowNullIds = false)
        {
            Setup(x => x.UpdateAsync(
                It.IsAny<Post>(),
                It.Is<List<Interest>>(v => allowNullIds || (v != null))
                )).Callback<Post, List<Interest>>((c, list) => PostUpdated = true)
                .Returns<Post, List<Interest>>((s, list) => Task.FromResult<Post>(s));
        }


        public void MockGetByID(string id, string knownId, string profileId = null, List<Comment> comments = null, List<Like> likes = null)
        {
            Post output = null;
            if (!string.IsNullOrWhiteSpace(id) && id == knownId)
            {
                output = new Post
                {
                    Id = id,
                    ProfileID = profileId,
                    Comments = comments, 
                    Likes = likes
                };
            }

            Setup(x => x.GetByIDAsync(
                It.Is<string>(c => c == id)
                )).Returns(Task.FromResult<Post>(output));
        }

        public void MockGetByIDs(List<string> ids, List<string> knownIds)
        {
            List<Post> output = new List<Post>();
            if (ids != null && knownIds != null)
            {
                foreach (var id in ids)
                {
                    if (knownIds.Contains(id))
                        output.Add(new Post { Id = id });
                }
            }

            Setup(x => x.GetByIDsAsync(
                It.Is<List<string>>(c => c == ids)
                )).Returns(Task.FromResult(output));
        }

        public void MockGetPostFeed(bool valid, string profileId = null, List<Comment> comments = null)
        {
            CountList<Post> output = new CountList<Post>();
            if (valid)
            {
                output = new CountList<Post> {
                    new Post() { Id = "Result", ProfileID = profileId, Comments = comments },
                    new Post() { Id = "Result", ProfileID = profileId, Comments = comments }
                };
                output.TotalCount = 2;
            }

            Setup(x => x.GetPostFeedAsync(
                It.Is<string>(c => !string.IsNullOrWhiteSpace(c)),
                It.Is<List<string>>(c => c != null),
                It.Is<List<Interest>>(c => c != null),
                It.Is<int>(c => c == 0),
                It.Is<int>(c => c > 0)
                )).Returns(Task.FromResult(output));
        }

    }
}

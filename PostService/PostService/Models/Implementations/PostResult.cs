using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostService.Models.Implementations
{
    public class PostResult
    {
        public string PostId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime DatePosted { get; set; }
        public List<CommentResult> Comments { get; set; }
        public int Likes { get; set; }
        public List<Interest> Tags { get; set; }
        public bool IsEdited { get; set; }
        public string ProfileID { get; set; }
        public ProfileType ProfileType { get; set; }
        public string ProfileName { get; set; }
 
    }
}

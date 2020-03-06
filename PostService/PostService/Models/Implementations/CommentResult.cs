using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostService.Models.Implementations
{
    public class CommentResult
    {
        public string Message { get; set; }
        public DateTime DateCommented { get; set; }
        public string ProfileID { get; set; }
        public ProfileType ProfileType { get; set; }
        public string ProfileName { get; set; }
    }
}

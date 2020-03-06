using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PostService.Models.Implementations.Requests
{
    public class CommentRequest : Request
    {
        [Required]
        [MaxLength(500, ErrorMessage = "Message should not exceed 500 characters")]
        public string Message { get; set; }
    }
}

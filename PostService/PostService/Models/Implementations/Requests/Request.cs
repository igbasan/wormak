using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PostService.Models.Implementations.Requests
{
    public class Request : BaseRequest
    {
        [Required]
        [RegularExpression(@"^[0-9a-fA-F]+$", ErrorMessage = "Invalid Post Id")]
        public string PostID { get; set; }
    }
    public class BaseRequest
    {

        public string ProfileID { get; set; }
        
        public ProfileType ProfileType { get; set; }
    }
}

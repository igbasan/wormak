using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InterUserService.Models.Implemetations
{
    public class TestimonialRequest
    {
        [Required]
        [RegularExpression(@"^[0-9a-fA-F]+$", ErrorMessage = "Invalid Id")]
        public string ProfileId { get; set; }
        [Required]
        public ProfileType ProfileType { get; set; }
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        [StringLength(500)]
        public string Comment { get; set; }
    }
}

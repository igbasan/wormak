using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProfileService.Models.Implementations
{
    public class ProfileLite
    {
        [RegularExpression(@"^[0-9a-fA-F]+$", ErrorMessage = "Invalid Id")]
        [Required]
        public string Id { get; set; }
        public string Name { get; set; }
        [Required]
        public ProfileType ProfileType { get; set; }
    }

    public class InterestProfileLite : ProfileLite
    {
        public List<Interest> Interests { get; set; }
    }
}

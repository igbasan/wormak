using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProfileService.Models.Implementations
{
    public class Company : Profile
    {
        [Required]
        public string Description { get; set; }
        [Required]
        public string Industry { get; set; }
        public string VerificationStatus { get; set; }
        public string SizeRange { get; set; }

        public Company()
        {
            ProfileType = ProfileType.Company;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProfileService.Models.Implementations
{
    public class Professional : Profile
    {
        [Required]
        [StringLength(100)]
        public string Type { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        public Professional()
        {
            ProfileType = ProfileType.Professional;
        }
    }
}

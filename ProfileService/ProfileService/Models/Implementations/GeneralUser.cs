using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProfileService.Models.Implementations
{
    public class GeneralUser : Profile
    {
        [Required]
        [StringLength(250)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(250)]
        public string LastName { get; set; }

        [DataType(DataType.DateTime, ErrorMessage = "Please enter a valid date in the format dd/mm/yyyy")]
        [Range(typeof(DateTime), "1/1/1800", "1/1/2516")]
        public DateTime DateOfBirth { get; set; }
        [Required]
        [StringLength(500)]
        public string Bio { get; set; }
        [StringLength(100)]
        public string Occupation { get; set; }

        public override string Name { get { return $"{FirstName} {LastName}"; } }

        public GeneralUser()
        {
            ProfileType = ProfileType.GeneralUser;
        }
    }
}

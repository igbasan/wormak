using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InterUserService.Models.Implemetations
{
    public class Testimonial : InterUser
    {
        /// <summary>
        /// Rating from 1 to 5
        /// </summary>
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        [StringLength(500)]
        public string Comment { get; set; }
    }
}

using InterUserService.Data.Interfaces;
using InterUserService.Models.Implemetations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterUserService.Data.Redis
{
    public class TestimonialRDAO : InterUserRDAO<Testimonial>, ITestimonialDAO
    {
        public TestimonialRDAO(ITestimonialDAO testimonialDAO, IRedisConnection connection) : base(testimonialDAO, connection, "TESTIMONIAL")
        {
        }
    }
}

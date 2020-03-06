using InterUserService.Data.Interfaces;
using InterUserService.Logic.Interfaces;
using InterUserService.Models.Implemetations;

namespace InterUserService.Logic.Implementation
{
    public class TestimonialLogic: InterUserLogic<Testimonial>, ITestimonialLogic
    {
        public TestimonialLogic(ITestimonialDAO testimonialDAO, IProfileLogic profileLogic) : base(testimonialDAO, profileLogic)
        {
        }
    }
}

using InterUserService.Logic.Interfaces;
using InterUserService.Models;
using InterUserService.Models.Exceptions;
using InterUserService.Models.Implemetations;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InterUserService.Test.Mocks.Logic
{
    public class MockTestimonialLogic : Mock<ITestimonialLogic>
    {
        public bool TestimonialSetUp { get; set; }
        public bool TestimonialSetUpToDeactivate { get; set; }

        public void MockSetUpRelationship(Testimonial testimonial)
        {
            Setup(x => x.SetUpRelationshipAsync(
                It.Is<Testimonial>(c => c.ActiveProfileID == testimonial.ActiveProfileID && c.PassiveProfileID == testimonial.PassiveProfileID),
                It.IsAny<bool>()
                )).Callback<Testimonial, bool>((client, deactivate) =>
                {
                    TestimonialSetUp = true;
                    TestimonialSetUpToDeactivate = deactivate;
                })
                .Returns<Testimonial, bool>((s,deact) => Task.FromResult(s));
        }
        public void MockSetUpRelationshipWithException(Testimonial testimonial)
        {
            Setup(x => x.SetUpRelationshipAsync(
                It.Is<Testimonial>(c => c.ActiveProfileID == testimonial.ActiveProfileID && c.PassiveProfileID == testimonial.PassiveProfileID),
                It.IsAny<bool>()
                )).Callback<Testimonial, bool>((client, deactivate) =>
                {
                    TestimonialSetUpToDeactivate = deactivate;
                })
                .Throws(new InterUserException("Test Exception"));
        }

        public void MockGetAllByPassiveProfileID(string profileID, string knownProfileID, ProfileType profileType, ProfileType knownProfileType)
        {
            CountList<Profile> outputList = new CountList<Profile>();
            if (!string.IsNullOrWhiteSpace(profileID) && profileID == knownProfileID && profileType == knownProfileType)
            {
                outputList = new CountList<Profile> { new Profile(), new Profile() };
                outputList.TotalCount = 2;
                outputList.AverageRating = 1.5;

                outputList.ForEach(c =>
                {
                    c.Id = "Result";
                    c.ProfileType = ProfileType.Professional;
                });
            }

            Setup(x => x.GetAllByPassiveProfileIDAsync(
                It.Is<string>(c => c == profileID),
                It.Is<ProfileType>(v => v == profileType),
                It.Is<int>(c => c == 0),
                It.Is<int>(c => c == 10)
                )).Returns(Task.FromResult(outputList));
        }
    }
}


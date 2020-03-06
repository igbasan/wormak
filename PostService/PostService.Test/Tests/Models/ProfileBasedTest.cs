using PostService.Models;
using PostService.Models.Implementations;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace PostService.Test.Tests.Models
{
    public class ProfileBasedTest
    {
        [Fact]
        public void ProfileID_SetProfileID()
        {
            ProfileBased profileBased = new ProfileBased();
            profileBased.ProfileID = "ProfileID";
            Assert.Equal($"GeneralUser_ProfileID", profileBased.ProfileID);
            Assert.Equal($"ProfileID", profileBased.ProfileIDRaw);
        }

        [Fact]
        public void ProfileID_SetProfileIDAndProfileType()
        {
            ProfileBased profileBased = new ProfileBased();
            profileBased.ProfileID = "ProfileID";
            profileBased.ProfileType = ProfileType.Company;
            Assert.Equal($"Company_ProfileID", profileBased.ProfileID);
            Assert.Equal($"ProfileID", profileBased.ProfileIDRaw);
        }

        [Fact]
        public void ProfileID_SetProfileIDAndProfileType2()
        {
            ProfileBased profileBased = new ProfileBased();
            profileBased.ProfileID = "Company_ProfileID";
            Assert.Equal($"Company_ProfileID", profileBased.ProfileID);
            Assert.Equal(ProfileType.Company, profileBased.ProfileType);
            Assert.Equal($"ProfileID", profileBased.ProfileIDRaw);
        }
    }
}

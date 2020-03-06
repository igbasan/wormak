using InterUserService.Models.Implemetations;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace InterUserService.Test.Tests.Models
{
    public class InterUserTest
    {
        [Fact]
        public void ActiveProfileID_SetProfileID()
        {
            InterUser interUser = new InterUser();
            interUser.ActiveProfileID = "ProfileID";
            Assert.Equal($"GeneralUser_ProfileID", interUser.ActiveProfileID);
            Assert.Equal($"ProfileID", interUser.ActiveProfileIDRaw);
        }

        [Fact]
        public void ActiveProfileID_SetProfileIDAndProfileType()
        {
            InterUser interUser = new InterUser();
            interUser.ActiveProfileID = "ProfileID";
            interUser.ActiveProfileType = InterUserService.Models.ProfileType.Company;
            Assert.Equal($"Company_ProfileID", interUser.ActiveProfileID);
            Assert.Equal($"ProfileID", interUser.ActiveProfileIDRaw);
        }

        [Fact]
        public void ActiveProfileID_SetProfileIDAndProfileType2()
        {
            InterUser interUser = new InterUser();
            interUser.ActiveProfileID = "Company_ProfileID";
            Assert.Equal($"Company_ProfileID", interUser.ActiveProfileID);
            Assert.Equal(InterUserService.Models.ProfileType.Company, interUser.ActiveProfileType);
            Assert.Equal($"ProfileID", interUser.ActiveProfileIDRaw);
        }

        [Fact]
        public void PassiveProfileID_SetProfileID()
        {
            InterUser interUser = new InterUser();
            interUser.PassiveProfileID = "ProfileID";
            Assert.Equal($"GeneralUser_ProfileID", interUser.PassiveProfileID);
            Assert.Equal($"ProfileID", interUser.PassiveProfileIDRaw);
        }

        [Fact]
        public void PassiveProfileID_SetProfileIDAndProfileType()
        {
            InterUser interUser = new InterUser();
            interUser.PassiveProfileID = "ProfileID";
            interUser.PassiveProfileType = InterUserService.Models.ProfileType.Company;
            Assert.Equal($"Company_ProfileID", interUser.PassiveProfileID);
            Assert.Equal($"ProfileID", interUser.PassiveProfileIDRaw);
        }

        [Fact]
        public void PassiveProfileID_SetProfileIDAndProfileType2()
        {
            InterUser interUser = new InterUser();
            interUser.PassiveProfileID = "Company_ProfileID";
            Assert.Equal($"Company_ProfileID", interUser.PassiveProfileID);
            Assert.Equal(InterUserService.Models.ProfileType.Company, interUser.PassiveProfileType);
            Assert.Equal($"ProfileID", interUser.PassiveProfileIDRaw);
        }
    }
}

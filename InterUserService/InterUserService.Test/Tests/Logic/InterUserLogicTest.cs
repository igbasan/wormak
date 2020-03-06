using InterUserService.Models;
using InterUserService.Models.Implemetations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace InterUserService.Test.Tests.Logic
{
    public class InterUserLogicTest : InterUserLogicGenericTest<InterUser>
    {
        [Fact]
        public async Task SetupProfileAsync_NullTest()
        {
            await SetupProfileAsync_Null();
        }
        [Fact]
        public async Task SetupProfileAsync_invalidProfileIdTest()
        {
            await SetupProfileAsync_invalidProfileId();
        }

        [Fact]
        public async Task SetupProfileAsync_SameProfileTest()
        {
            await SetupProfileAsync_SameProfile();
        }

        [Theory]
        [InlineData("KnownActiveProfileID", "KnownPassiveProfileID")]
        [InlineData("ActiveProfileID", "KnownPassiveProfileID")]
        [InlineData("ActiveProfileID", "PassiveProfileID")]
        public async Task SetupProfileAsync_ValidProfileIdTest(string activeProfileId, string passiveProfileId)
        {
            await SetupProfileAsync_ValidProfileId(activeProfileId, passiveProfileId);
        }

        [Theory]
        [InlineData(false, true)]
        [InlineData(false, false)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public async Task SetupProfileAsync_ValidProfileId_ChangeActiveStateTest(bool activate, bool currrentState)
        {
            await SetupProfileAsync_ValidProfileId_ChangeActiveState(activate, currrentState);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public async Task GetByActiveProfileIDandPassiveProfileIDAsync_NullOrEmptyActiveIDTest(string profileId)
        {
            await GetByActiveProfileIDandPassiveProfileIDAsync_NullOrEmptyActiveID(profileId);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public async Task GetByActiveProfileIDandPassiveProfileIDAsync_NullOrEmptyPassiveIDTest(string profileId)
        {
            await GetByActiveProfileIDandPassiveProfileIDAsync_NullOrEmptyPassiveID(profileId);
        }

        [Theory]
        [InlineData("KnownActiveProfileID", "KnownPassiveProfileID", ProfileType.Company )]
        [InlineData("ActiveProfileID", "KnownPassiveProfileID", ProfileType.Professional)]
        [InlineData("ActiveProfileID", "PassiveProfileID", ProfileType.GeneralUser)]
        public async Task GetByActiveProfileIDandPassiveProfileIDAsync_NotNullTest(string activeProfileId, string passiveProfileId, ProfileType profileType)
        {
            await GetByActiveProfileIDandPassiveProfileIDAsync_NotNull(activeProfileId, passiveProfileId, profileType);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public async Task GetAllByActiveProfileIDAsync_NullOrEmptyTest(string profileId)
        {
            await GetAllByActiveProfileIDAsync_NullOrEmpty(profileId);
        }

        [Theory]
        [InlineData("KnownProfileID", ProfileType.Company)]
        [InlineData("ProfileID", ProfileType.Professional)]
        public async Task GetAllByActiveProfileIDAsync_NotNullTest(string profileId, ProfileType profileType)
        {
            await GetAllByActiveProfileIDAsync_NotNull(profileId, profileType);
        }


        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public async Task GetAllByPassiveProfileIDAsync_NullOrEmptyTest(string profileId)
        {
            await GetAllByPassiveProfileIDAsync_NullOrEmpty(profileId);
        }

        [Theory]
        [InlineData("KnownProfileID", ProfileType.Company)]
        [InlineData("ProfileID", ProfileType.Professional)]
        public async Task GetAllByPassiveProfileIDAsync_NotNullTest(string profileId, ProfileType profileType)
        {
            await GetAllByPassiveProfileIDAsync_NotNull(profileId, profileType);
        }
    }
}

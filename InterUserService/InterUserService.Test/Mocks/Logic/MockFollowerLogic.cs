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
    public class MockFollowerLogic : Mock<IFollowerLogic>
    {
        public bool FollowerSetUp { get; set; }
        public bool FollowerSetUpToDeactivate { get; set; }

        public void MockSetUpRelationship(Follower follower)
        {
            Setup(x => x.SetUpRelationshipAsync(
                It.Is<Follower>(c => c.ActiveProfileID == follower.ActiveProfileID && c.PassiveProfileID == follower.PassiveProfileID),
                It.IsAny<bool>()
                )).Callback<Follower, bool>((client, deactivate) =>
                {
                    FollowerSetUp = true;
                    FollowerSetUpToDeactivate = deactivate;
                })
                .Returns<Follower, bool>((s,deact) => Task.FromResult(s));
        }
        public void MockSetUpRelationshipWithException(Follower follower)
        {
            Setup(x => x.SetUpRelationshipAsync(
                It.Is<Follower>(c => c.ActiveProfileID == follower.ActiveProfileID && c.PassiveProfileID == follower.PassiveProfileID),
                It.IsAny<bool>()
                )).Callback<Follower, bool>((client, deactivate) =>
                {
                    FollowerSetUpToDeactivate = deactivate;
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

                outputList.ForEach(c =>
                {
                    c.Id = "Result";
                    c.ProfileType = ProfileType.Professional;
                });
            }

            Setup(x => x.GetAllByPassiveProfileIDAsync(
                It.Is<string>(c => c == profileID),
                It.Is<ProfileType>(c => c == profileType),
                It.Is<int>(c => c == 0),
                It.Is<int>(c => c == 10 || c == -1)
                )).Returns(Task.FromResult(outputList));
        }

        public void MockGetAllByActiveProfileID(string profileID, string knownProfileID, ProfileType profileType, ProfileType knownProfileType)
        {
            CountList<Profile> outputList = new CountList<Profile>();
            if (!string.IsNullOrWhiteSpace(profileID) && profileID == knownProfileID && profileType == knownProfileType)
            {
                outputList = new CountList<Profile> { new Profile(), new Profile() };
                outputList.TotalCount = 2;

                outputList.ForEach(c =>
                {
                    c.Id = "Result";
                    c.ProfileType = ProfileType.Professional;
                });
            }

            Setup(x => x.GetAllByActiveProfileIDAsync(
                It.Is<string>(c => c == profileID),
                It.Is<ProfileType>(c => c == profileType),
                It.Is<int>(c => c == 0),
                It.Is<int>(c => c == 10 || c == -1)
                )).Returns(Task.FromResult(outputList));
        }

        public void MockGetByActiveProfileIDandPassiveProfileID(string activeProfileID, string knownActiveProfileID, string passiveProfileID, string knownPassiveProfileID, ProfileType profileType, bool isActive = true)
        {
            Follower output = null;
            if (!string.IsNullOrWhiteSpace(activeProfileID) && !string.IsNullOrWhiteSpace(passiveProfileID) && activeProfileID == knownActiveProfileID && passiveProfileID == knownPassiveProfileID)
            {
                output = new Follower
                {
                    ActiveProfileID = activeProfileID,
                    PassiveProfileID = passiveProfileID,
                    ActiveProfileType = profileType,
                    PassiveProfileType = profileType,
                    IsActive = isActive
                };
            }

            Setup(x => x.GetByActiveProfileIDandPassiveProfileIDAsync(
                It.Is<string>(c => c == activeProfileID),
                It.Is<ProfileType>(c => c == profileType),
                It.Is<string>(c => c == passiveProfileID),
                It.Is<ProfileType>(c => c == profileType)
                )).Returns(Task.FromResult(output));
        }
    }
}

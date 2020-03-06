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
    public class MockClientLogic : Mock<IClientLogic>
    {
        public bool ClientSetUp { get; set; }
        public bool ClientSetUpToDeactivate { get; set; }

        public void MockSetUpRelationship(Client client)
        {
            Setup(x => x.SetUpRelationshipAsync(
                It.Is<Client>(c => c.PassiveProfileID == client.PassiveProfileID && c.ActiveProfileID == client.ActiveProfileID),
                It.IsAny<bool>()
                )).Callback<Client, bool>((client, deactivate) =>
                {
                    ClientSetUp = true;
                    ClientSetUpToDeactivate = deactivate;
                })
                .Returns<Client, bool>((s,deact) => Task.FromResult(s));
        }
        public void MockSetUpRelationshipWithException(Client client)
        {
            Setup(x => x.SetUpRelationshipAsync(
                It.Is<Client>(c => c.PassiveProfileID == client.PassiveProfileID && c.ActiveProfileID == client.ActiveProfileID),
                It.IsAny<bool>()
                )).Callback<Client, bool>((client, deactivate) =>
                {
                    ClientSetUpToDeactivate = deactivate;
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
                It.Is<ProfileType>(v => v == profileType),
                It.Is<int>(c => c == 0),
                It.Is<int>(c => c == 10)
                )).Returns(Task.FromResult(outputList));
        }
    }
}

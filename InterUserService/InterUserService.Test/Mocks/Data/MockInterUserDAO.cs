using InterUserService.Data.Interfaces;
using InterUserService.Models.Implemetations;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InterUserService.Test.Mocks.Data
{
    public class MockInterUserDAO<T> : Mock<IInterUserDAO<T>> where T : InterUser, new()
    {
        public bool InterUserCreated { get; set; }
        public bool InterUserUpdated { get; set; }
        public void MockCreate()
        {
            Setup(x => x.CreateAsync(
                It.IsAny<T>()
                )).Callback<T>(c => InterUserCreated = true)
                .Returns<T>(s => Task.FromResult<T>(s));
        }
        public void MockUpdate()
        {
            Setup(x => x.UpdateAsync(
                It.IsAny<T>()
                )).Callback<T>(c => InterUserUpdated = true)
                .Returns<T>(s => Task.FromResult<T>(s));
        }
        //default for test 0 for skip, 10 for take
        public void MockGetAllByProfileID(string profileID, string knownProfileID, bool isActiveUser)
        {
            CountList<T> outputList = new CountList<T>();
            if (!string.IsNullOrWhiteSpace(profileID) && profileID == knownProfileID)
            {
                outputList = new CountList<T> { new T(), new T() };
                outputList.TotalCount = 2;

                int count = 1;
                if (isActiveUser) outputList.ForEach(c => c.PassiveProfileID = $"Result{count++}");
                else outputList.ForEach(c => c.ActiveProfileID = $"Result{count++}");
            }

            if (isActiveUser)
            {
                Setup(x => x.GetAllByActiveProfileIDAsync(
                It.Is<string>(c => c == profileID),
                It.Is<int>(c => c == 0),
                It.Is<int>(c => c == 10)
                )).Returns(Task.FromResult(outputList));
            }
            else
            {
                Setup(x => x.GetAllByPassiveProfileIDAsync(
                It.Is<string>(c => c == profileID),
                It.Is<int>(c => c == 0),
                It.Is<int>(c => c == 10)
                )).Returns(Task.FromResult(outputList));
            }
        }

        public void MockGetByActiveProfileIDandPassiveProfileID(string activeProfileID, string knownActiveProfileID, string passiveProfileID, string knownPassiveProfileID, bool isActive = true)
        {
            T output = null;
            if (!string.IsNullOrWhiteSpace(activeProfileID) && !string.IsNullOrWhiteSpace(passiveProfileID) && activeProfileID == knownActiveProfileID && passiveProfileID == knownPassiveProfileID)
            {
                output = new T
                {
                    ActiveProfileID = activeProfileID,
                    PassiveProfileID = passiveProfileID,
                    IsActive = isActive
                };
            }

            Setup(x => x.GetByActiveProfileIDandPassiveProfileIDAsync(
                It.Is<string>(c => c == activeProfileID),
                It.Is<string>(c => c == passiveProfileID)
                )).Returns(Task.FromResult<T>(output));
        }
    }
}

using Microsoft.AspNetCore.Http;
using Moq;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Data;
using StargateAPI.Business.Repositories;
using StargateAPI.Business.Services;

namespace StargateUnitTests.Services
{
    public class AstronautDutyServiceTests
    {
        private readonly Mock<IPersonRepository> _personRepositoryMock;
        private readonly Mock<ILogService> _logServiceMock;
        private readonly IAstronautDutyService _astronautDutyService;

        public AstronautDutyServiceTests()
        {
            _personRepositoryMock = new Mock<IPersonRepository>();
            _logServiceMock = new Mock<ILogService>();
            _astronautDutyService = new AstronautDutyService(_personRepositoryMock.Object, _logServiceMock.Object);
        }

        [Fact]
        public async Task CreateDutyAsync_ReturnsSuccess_WhenDutyIsCreated()
        {
            var person = new Person
            {
                Id = 1,
                Name = "John Doe",
                AstronautDuties = new List<AstronautDuty>(),
                AstronautDetail = null
            };

            _personRepositoryMock.Setup(repo => repo.GetByNameAsync("John Doe", It.IsAny<CancellationToken>()))
                .ReturnsAsync(person);

            _personRepositoryMock.Setup(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Callback(() =>
            {
                var duty = person.AstronautDuties.First();
                duty.Id = 1;
            });

            var request = new CreateAstronautDuty
            {
                Name = "John Doe",
                Rank = "Captain",
                DutyTitle = "Commander",
                DutyStartDate = new DateTime(2024, 1, 1)
            };

            var result = await _astronautDutyService.CreateDutyAsync(request, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);

            _logServiceMock.Verify(log => log.LogSuccessAsync("Create Astronaut Duty: Add Astronaut Duty", $"Name: {request.Name}, Title: {request.DutyTitle}, Rank: {request.Rank}, StartDate: {request.DutyStartDate.Date}", It.IsAny<CancellationToken>()), Times.Once);
            _personRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateDutyAsync_ThrowsException_WhenPersonNotFound()
        {
            _personRepositoryMock.Setup(repo => repo.GetByNameAsync("Nonexistent", It.IsAny<CancellationToken>()))
                .ReturnsAsync((Person?)null);

            var request = new CreateAstronautDuty
            {
                Name = "Nonexistent",
                Rank = "Captain",
                DutyTitle = "Commander",
                DutyStartDate = new DateTime(2023, 1, 1)
            };

            await Assert.ThrowsAsync<BadHttpRequestException>(() => _astronautDutyService.CreateDutyAsync(request, CancellationToken.None));
        }

        [Fact]
        public async Task CreateDutyAsync_ThrowsException_WhenDutyAlreadyExists()
        {
            var person = new Person
            {
                Id = 1,
                Name = "John Doe",
                AstronautDuties = new List<AstronautDuty>
            {
                new AstronautDuty
                {
                    DutyTitle = "Commander",
                    DutyStartDate = new DateTime(2023, 1, 1)
                }
            },
                AstronautDetail = null
            };

            _personRepositoryMock.Setup(repo => repo.GetByNameAsync("John Doe", It.IsAny<CancellationToken>()))
                .ReturnsAsync(person);

            var request = new CreateAstronautDuty
            {
                Name = "John Doe",
                Rank = "Captain",
                DutyTitle = "Commander",
                DutyStartDate = new DateTime(2023, 1, 1)
            };

            await Assert.ThrowsAsync<BadHttpRequestException>(() => _astronautDutyService.CreateDutyAsync(request, CancellationToken.None));

            _logServiceMock.Verify(log => log.LogExceptionAsync("Create Astronaut Duty", $"{person.Name} already has a duty of {request.DutyTitle} with a start date of {request.DutyStartDate}", It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

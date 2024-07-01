using Microsoft.AspNetCore.Http;
using Moq;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Data;
using StargateAPI.Business.Repositories;
using StargateAPI.Business.Services;

namespace StargateUnitTests.Services
{
    public class PersonServiceTests
    {
        private readonly Mock<IPersonRepository> _personRepositoryMock;
        private readonly Mock<ILogService> _logServiceMock;
        private readonly IPersonService _personService;

        public PersonServiceTests()
        {
            _personRepositoryMock = new Mock<IPersonRepository>();
            _logServiceMock = new Mock<ILogService>();
            _personService = new PersonService(_personRepositoryMock.Object, _logServiceMock.Object);
        }

        [Fact]
        public async Task UpdatePerson_ReturnsSuccess_WhenPersonIsUpdated()
        {
            var person = new Person { Name = "John Doe" };

            _personRepositoryMock.Setup(repo => repo.GetByNameAsync("John Doe", It.IsAny<CancellationToken>()))
                .ReturnsAsync(person);

            var request = new UpdatePerson { Name = "John Doe", NewName = "Jane Doe" };
            var response = await _personService.UpdatePersonAsync(request, CancellationToken.None);

            Assert.True(response.Success);
            Assert.Equal("Person updated successfully", response.Message);
            _logServiceMock.Verify(log => log.LogSuccessAsync("Update Person", $"Name: {request.Name}, New Name: {request.NewName}", It.IsAny<CancellationToken>()), Times.Once);
            _personRepositoryMock.Verify(repo => repo.UpdatePersonAsync(person, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdatePerson_ThrowsException_WhenPersonNotFound()
        {
            _personRepositoryMock.Setup(repo => repo.GetByNameAsync("Nonexistent", It.IsAny<CancellationToken>()))
                .ReturnsAsync((Person?)null);

            var request = new UpdatePerson { Name = "Nonexistent", NewName = "Jane Doe" };

            await Assert.ThrowsAsync<BadHttpRequestException>(() => _personService.UpdatePersonAsync(request, CancellationToken.None));
            _logServiceMock.Verify(log => log.LogExceptionAsync("Update Person", "Nonexistent not found", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreatePerson_ReturnsSuccess_WhenPersonIsCreated()
        {
            _personRepositoryMock.Setup(repo => repo.GetByNameAsync("John Doe", It.IsAny<CancellationToken>()))
                .ReturnsAsync((Person?)null);

            var request = new CreatePerson { Name = "John Doe" };
            var response = await _personService.CreatePersonAsync(request, CancellationToken.None);

            Assert.NotNull(response);
            _logServiceMock.Verify(log => log.LogSuccessAsync("Create Person", $"Name: {request.Name}", It.IsAny<CancellationToken>()), Times.Once);
            _personRepositoryMock.Verify(repo => repo.AddAsync(It.Is<Person>(p => p.Name == "John Doe"), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreatePerson_ThrowsException_WhenPersonAlreadyExists()
        {
            var person = new Person { Name = "John Doe" };

            _personRepositoryMock.Setup(repo => repo.GetByNameAsync("John Doe", It.IsAny<CancellationToken>()))
                .ReturnsAsync(person);

            var request = new CreatePerson { Name = "John Doe" };

            await Assert.ThrowsAsync<BadHttpRequestException>(() => _personService.CreatePersonAsync(request, CancellationToken.None));
            _logServiceMock.Verify(log => log.LogExceptionAsync("Create Person", "John Doe already exists", It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

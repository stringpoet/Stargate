using Moq;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using StargateAPI.Controllers;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Queries;
using System.Net;

namespace StargateUnitTests.Controllers
{
    public class PersonControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly PersonController _controller;

        public PersonControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new PersonController(_mediatorMock.Object);
        }

        [Fact]
        public async Task GetPeople_ReturnsOkResult_WhenSuccessful()
        {
            var response = new GetPeopleResult { Success = true, ResponseCode = (int)HttpStatusCode.OK };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPeople>(), default))
                .ReturnsAsync(response);

            var result = await _controller.GetPeople();

            var okResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
        }

        [Fact]
        public async Task GetPersonByName_ReturnsOkResult_WhenSuccessful()
        {
            var response = new GetPersonByNameResult { Success = true, ResponseCode = (int)HttpStatusCode.OK };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPersonByName>(), default))
                .ReturnsAsync(response);

            var result = await _controller.GetPersonByName("John");

            var okResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
        }

        [Fact]
        public async Task CreatePerson_ReturnsOkResult_WhenSuccessful()
        {
            var response = new CreatePersonResult { Success = true, ResponseCode = (int)HttpStatusCode.OK };
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreatePerson>(), default))
                .ReturnsAsync(response);

            var result = await _controller.CreatePerson("John Doe");

            var okResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
        }

        [Fact]
        public async Task UpdatePerson_ReturnsOkResult_WhenSuccessful()
        {
            var response = new BaseResponse { Success = true, ResponseCode = (int)HttpStatusCode.OK };
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdatePerson>(), default))
                .ReturnsAsync(response);

            var result = await _controller.UpdatePerson("John Doe", "John Smith");

            var okResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
        }

        [Fact]
        public async Task GetPeople_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPeople>(), default))
                .ThrowsAsync(new Exception("Test exception"));

            var result = await _controller.GetPeople();

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.InternalServerError, objectResult.StatusCode);
        }
    }
}
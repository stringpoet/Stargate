using Moq;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using StargateAPI.Controllers;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Queries;
using System.Net;

namespace StargateUnitTests.Controllers
{
    public class AstronautDutyControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly AstronautDutyController _controller;

        public AstronautDutyControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new AstronautDutyController(_mediatorMock.Object);
        }

        [Fact]
        public async Task GetAstronautDutiesByName_ReturnsOkResult_WhenSuccessful()
        {
            var response = new GetAstronautDutiesByNameResult { Success = true, ResponseCode = (int)HttpStatusCode.OK };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAstronautDutiesByName>(), default))
                .ReturnsAsync(response);

            var result = await _controller.GetAstronautDutiesByName("John");

            var okResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
        }

        [Fact]
        public async Task CreateAstronautDuty_ReturnsOkResult_WhenSuccessful()
        {
            var response = new CreateAstronautDutyResult { Success = true, ResponseCode = (int)HttpStatusCode.OK };
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateAstronautDuty>(), default))
                .ReturnsAsync(response);

            var request = new CreateAstronautDuty
            {
                Name = "John",
                Rank = "LT",
                DutyTitle = "Commander",
                DutyStartDate = DateTime.Now
            };
            var result = await _controller.CreateAstronautDuty(request);

            var okResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
        }

        [Fact]
        public async Task GetAstronautDutiesByName_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAstronautDutiesByName>(), default))
                .ThrowsAsync(new Exception("Test exception"));

            var result = await _controller.GetAstronautDutiesByName("John");

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.InternalServerError, objectResult.StatusCode);
        }
    }
}
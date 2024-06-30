using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Data;

namespace StargateUnitTests.Commands
{
    public class CreateAstronautDutyTests
    {
        private readonly StargateContext _context;
        private readonly CreateAstronautDutyHandler _handler;
        private readonly CreateAstronautDutyPreProcessor _preProcessor;

        public CreateAstronautDutyTests()
        {
            var options = new DbContextOptionsBuilder<StargateContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new StargateContext(options);
            _handler = new CreateAstronautDutyHandler(_context);
            _preProcessor = new CreateAstronautDutyPreProcessor(_context);
        }

        [Fact]
        public async Task CreateAstronautDutyHandler_ReturnsSuccess_WhenDutyIsCreated()
        {
            var person = new Person { Name = "John Doe" };
            await _context.People.AddAsync(person);
            await _context.SaveChangesAsync();

            var request = new CreateAstronautDuty
            {
                Name = "John Doe",
                Rank = "Captain",
                DutyTitle = "Commander",
                DutyStartDate = DateTime.UtcNow
            };

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.Success);
            Assert.Equal("Commander", (await _context.AstronautDetails.FirstAsync()).CurrentDutyTitle);
            Assert.Equal("Commander", (await _context.AstronautDuties.FirstAsync()).DutyTitle);

            var successLog = await _context.SuccessLog.FirstOrDefaultAsync(s => s.Process == "Create Astronaut Duty: Add Astronaut Duty" && s.Message.Contains("John Doe"));
            Assert.NotNull(successLog);
        }

        [Fact]
        public async Task CreateAstronautDutyPreProcessor_ThrowsException_WhenPersonNotFound()
        {
            var request = new CreateAstronautDuty
            {
                Name = "Nonexistent",
                Rank = "Captain",
                DutyTitle = "Commander",
                DutyStartDate = DateTime.UtcNow
            };

            await Assert.ThrowsAsync<BadHttpRequestException>(() => _preProcessor.Process(request, CancellationToken.None));
            Assert.True(await _context.ExceptionLog.AnyAsync(e => e.Process == "Create Astronaut Duty" && e.Message == "Person Not Found: Nonexistent"));
        }

        [Fact]
        public async Task CreateAstronautDutyPreProcessor_ThrowsException_WhenDutyAlreadyExists()
        {
            var now = DateTime.UtcNow;
            var person = new Person { Name = "John Doe" };
            await _context.People.AddAsync(person);
            await _context.AstronautDuties.AddAsync(new AstronautDuty
            {
                PersonId = person.Id,
                DutyTitle = "Commander",
                DutyStartDate = now
            });
            await _context.SaveChangesAsync();

            var request = new CreateAstronautDuty
            {
                Name = "John Doe",
                Rank = "Captain",
                DutyTitle = "Commander",
                DutyStartDate = now
            };

            await Assert.ThrowsAsync<BadHttpRequestException>(() => _preProcessor.Process(request, CancellationToken.None));
            Assert.True(await _context.ExceptionLog.AnyAsync(e => e.Process == "Create Astronaut Duty" && e.Message.Contains("John Doe already has a duty")));
        }
    }
}
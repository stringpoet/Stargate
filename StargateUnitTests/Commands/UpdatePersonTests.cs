using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Data;

namespace StargateUnitTests.Commands
{
    public class UpdatePersonTests
    {
        private readonly StargateContext _context;
        private readonly UpdatePersonHandler _handler;
        private readonly UpdatePersonPreProcessor _preProcessor;

        public UpdatePersonTests()
        {
            var options = new DbContextOptionsBuilder<StargateContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new StargateContext(options);
            _handler = new UpdatePersonHandler(_context);
            _preProcessor = new UpdatePersonPreProcessor(_context);
        }

        [Fact]
        public async Task UpdatePersonHandler_ReturnsSuccess_WhenPersonIsUpdated()
        {
            var existingPerson = new Person { Name = "John Doe" };
            await _context.People.AddAsync(existingPerson);
            await _context.SaveChangesAsync();

            var request = new UpdatePerson { Name = "John Doe", NewName = "Jane Doe" };
            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.Success);
            Assert.Equal("Person updated successfully", result.Message);
            Assert.Equal(200, result.ResponseCode);
            Assert.Equal("Jane Doe", (await _context.People.FirstAsync()).Name);

            var successLog = await _context.SuccessLog.FirstOrDefaultAsync(s => s.Process == "Update Person" && s.Message.Contains("Name: John Doe, New Name: Jane Doe"));
            Assert.NotNull(successLog);
        }

        [Fact]
        public async Task UpdatePersonPreProcessor_ThrowsException_WhenPersonNotFound()
        {
            var request = new UpdatePerson { Name = "Nonexistent", NewName = "New Name" };

            await Assert.ThrowsAsync<BadHttpRequestException>(() => _preProcessor.Process(request, CancellationToken.None));

            var exceptionLog = await _context.ExceptionLog.FirstOrDefaultAsync(e => e.Process == "Update Person" && e.Message == "Nonexistent not found");
            Assert.NotNull(exceptionLog);
        }
    }
}
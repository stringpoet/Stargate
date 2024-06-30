using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Data;

namespace StargateUnitTests.Commands
{
    public class CreatePersonTests
    {
        private readonly StargateContext _context;
        private readonly CreatePersonHandler _handler;
        private readonly CreatePersonPreProcessor _preProcessor;

        public CreatePersonTests()
        {
            var options = new DbContextOptionsBuilder<StargateContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new StargateContext(options);
            _handler = new CreatePersonHandler(_context);
            _preProcessor = new CreatePersonPreProcessor(_context);
        }

        [Fact]
        public async Task CreatePersonHandler_ReturnsCorrectResult_WhenPersonIsCreated()
        {
            var request = new CreatePerson { Name = "John Doe" };
            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.True(result.Id > 0);
            Assert.True(await _context.SuccessLog.AnyAsync(s => s.Process == "Create Person" && s.Message == "Name: John Doe"));
         }

        [Fact]
        public async Task CreatePersonPreProcessor_ThrowsException_WhenPersonAlreadyExists()
        {
            var existingPerson = new Person { Name = "John Doe" };
            await _context.People.AddAsync(existingPerson);
            await _context.SaveChangesAsync();

            var request = new CreatePerson { Name = "John Doe" };

            await Assert.ThrowsAsync<BadHttpRequestException>(() => _preProcessor.Process(request, CancellationToken.None));
            Assert.True(await _context.ExceptionLog.AnyAsync(e => e.Process == "Create Person" && e.Message == "John Doe already exists"));
        }
    }
}
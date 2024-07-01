using StargateAPI.Controllers;
using MediatR;
using StargateAPI.Business.Data;
using Microsoft.EntityFrameworkCore;
using MediatR.Pipeline;

namespace StargateAPI.Business.Commands
{
    public class UpdatePerson : IRequest<BaseResponse>
    {
        public required string Name { get; set; }
        public required string NewName { get; set; }
    }

    public class UpdatePersonPreProcessor : IRequestPreProcessor<UpdatePerson>
    {
        private readonly StargateContext _context;

        public UpdatePersonPreProcessor(StargateContext context) => _context = context;

        public Task Process(UpdatePerson request, CancellationToken cancellationToken)
        {
            var person = _context.People.AsNoTracking().FirstOrDefault(z => z.Name == request.Name);

            if (person is null)
            {
                _context.ExceptionLog.Add(new ExceptionLog { Process = "Update Person", Message = $"{request.Name} not found" });
                _context.SaveChanges(); 
                throw new BadHttpRequestException("Person Not Found"); 
            }

            return Task.CompletedTask;
        }
    }

    public class UpdatePersonHandler : IRequestHandler<UpdatePerson, BaseResponse>
    {
        private readonly StargateContext _context;

        public UpdatePersonHandler(StargateContext context) => _context = context;

        public async Task<BaseResponse> Handle(UpdatePerson request, CancellationToken cancellationToken)
        {
            var person = await _context.People.FirstAsync(p => p.Name == request.Name, cancellationToken);

            person.Name = request.NewName;

            await _context.SuccessLog.AddAsync(new SuccessLog { Process = "Update Person", Message = $"Name: {request.Name}, New Name: {request.NewName}" }, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new BaseResponse
            {
                Success = true,
                Message = "Person updated successfully",
                ResponseCode = 200
            };
        }
    }
}

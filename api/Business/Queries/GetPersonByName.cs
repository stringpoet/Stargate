using MediatR;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
using StargateAPI.Business.Services;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Queries
{
    public class GetPersonByName : IRequest<GetPersonByNameResult>
    {
        public required string Name { get; set; } = string.Empty;
    }

    public class GetPersonByNameHandler : IRequestHandler<GetPersonByName, GetPersonByNameResult>
    {
        private readonly StargateContext _context;
        public GetPersonByNameHandler(StargateContext context)
        {
            _context = context;
        }

        public async Task<GetPersonByNameResult> Handle(GetPersonByName request, CancellationToken cancellationToken)
        {
            var result = new GetPersonByNameResult
            {
                Person = await _context.People
                    .Include(p => p.AstronautDetail)
                    .Where(p => p.Name == request.Name)
                    .Select(p => PersonMapper.ToPersonAstronaut(p))
                    .SingleOrDefaultAsync(cancellationToken)
            };

            return result;
        }
    }

    public class GetPersonByNameResult : BaseResponse
    {
        public PersonAstronaut? Person { get; set; }
    }
}

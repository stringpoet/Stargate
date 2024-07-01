using MediatR;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
using StargateAPI.Business.Services;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Queries
{
    public class GetPeople : IRequest<GetPeopleResult>
    {

    }

    public class GetPeopleHandler : IRequestHandler<GetPeople, GetPeopleResult>
    {
        public readonly StargateContext _context;
        public GetPeopleHandler(StargateContext context)
        {
            _context = context;
        }
        public async Task<GetPeopleResult> Handle(GetPeople request, CancellationToken cancellationToken)
        {
            var result = new GetPeopleResult
            {
                People = await _context.People
                    .Include(p => p.AstronautDetail)
                    .Select(p => PersonMapper.ToPersonAstronaut(p)).ToListAsync(cancellationToken)
            };

            return result;
        }
    }

    public class GetPeopleResult : BaseResponse
    {
        public List<PersonAstronaut> People { get; set; } = new List<PersonAstronaut> { };

    }
}

using MediatR;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
using StargateAPI.Business.Services;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Queries
{
    public class GetAstronautDutiesByName : IRequest<GetAstronautDutiesByNameResult>
    {
        public string Name { get; set; } = string.Empty;
    }

    public class GetAstronautDutiesByNameHandler : IRequestHandler<GetAstronautDutiesByName, GetAstronautDutiesByNameResult>
    {
        private readonly StargateContext _context;

        public GetAstronautDutiesByNameHandler(StargateContext context)
        {
            _context = context;
        }

        public async Task<GetAstronautDutiesByNameResult> Handle(GetAstronautDutiesByName request, CancellationToken cancellationToken)
        {
            var result = new GetAstronautDutiesByNameResult();

            var person = await _context.People.Where(p => p.Name == request.Name)
                .Include(p => p.AstronautDuties)
                .Include(p => p.AstronautDetail)
                .FirstOrDefaultAsync(cancellationToken);

            if (person is not null) {
                result.Person = PersonMapper.ToPersonAstronaut(person);
                result.AstronautDuties = [.. person.AstronautDuties];
            }
            else
            {
                throw new BadHttpRequestException("Person not found");
            }

            return result;

        }
    }

    public class GetAstronautDutiesByNameResult : BaseResponse
    {
        public PersonAstronaut? Person { get; set; }
        public List<AstronautDuty> AstronautDuties { get; set; } = new List<AstronautDuty>();
    }
}

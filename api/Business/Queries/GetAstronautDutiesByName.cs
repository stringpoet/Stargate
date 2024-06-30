using MediatR;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
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

            var query = from person in _context.People
                        where person.Name == request.Name
                        join detail in _context.AstronautDetails
                        on person.Id equals detail.PersonId into personAstronaut
                        from pa in personAstronaut.DefaultIfEmpty()
                        select new PersonAstronaut
                        {
                            PersonId = person.Id,
                            Name = person.Name,
                            CurrentRank = pa.CurrentRank,
                            CurrentDutyTitle = pa.CurrentDutyTitle,
                            CareerStartDate = pa.CareerStartDate,
                            CareerEndDate = pa.CareerEndDate
                        };

            result.Person = await query.FirstOrDefaultAsync();

            if (result.Person is not null) {
                var duties = from duty in _context.AstronautDuties
                        where duty.PersonId == result.Person.PersonId
                        orderby duty.DutyStartDate descending
                        select new AstronautDuty
                        {
                            Id = duty.Id,
                            PersonId = result.Person.PersonId,
                            Rank = duty.Rank,
                            DutyTitle = duty.DutyTitle,
                            DutyStartDate = duty.DutyStartDate,
                            DutyEndDate = duty.DutyEndDate
                        };

                result.AstronautDuties = await duties.ToListAsync();
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

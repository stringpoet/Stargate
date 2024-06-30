using Dapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
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
            var result = new GetPeopleResult();

            var query = from person in _context.People
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

            result.People = await query.ToListAsync();

            return result;
        }
    }

    public class GetPeopleResult : BaseResponse
    {
        public List<PersonAstronaut> People { get; set; } = new List<PersonAstronaut> { };

    }
}

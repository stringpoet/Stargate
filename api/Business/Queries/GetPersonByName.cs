﻿using Dapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
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
            var result = new GetPersonByNameResult();

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

            return result;
        }
    }

    public class GetPersonByNameResult : BaseResponse
    {
        public PersonAstronaut? Person { get; set; }
    }
}

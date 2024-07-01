using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Commands
{
    public class CreateAstronautDuty : IRequest<CreateAstronautDutyResult>
    {
        public required string Name { get; set; }

        public required string Rank { get; set; }

        public required string DutyTitle { get; set; }

        public DateTime DutyStartDate { get; set; }
    }

    public class CreateAstronautDutyPreProcessor : IRequestPreProcessor<CreateAstronautDuty>
    {
        private readonly StargateContext _context;

        public CreateAstronautDutyPreProcessor(StargateContext context)
        {
            _context = context;
        }

        public async Task Process(CreateAstronautDuty request, CancellationToken cancellationToken)
        {
            var person = await _context.People.AsNoTracking().Where(z => z.Name == request.Name)
                .Include(p => p.AstronautDuties)
                .Include(p => p.AstronautDetail)
                .SingleOrDefaultAsync(cancellationToken);

            if (person is null)
            {
                await _context.ExceptionLog.AddAsync(new ExceptionLog { Process = "Create Astronaut Duty", Message = $"Person Not Found: {request.Name}" }, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                throw new BadHttpRequestException("Person not found");
            }

            if (person.AstronautDuties.Any(d => d.DutyTitle == request.DutyTitle && d.DutyStartDate == request.DutyStartDate))
            {
                await _context.ExceptionLog.AddAsync(new ExceptionLog { Process = "Create Astronaut Duty", Message = $"{person.Name} already has a duty of {request.DutyTitle} with a start date of {request.DutyStartDate}" }, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                throw new BadHttpRequestException("Duty already exists");
            }

            await Task.CompletedTask;
        }
    }

    public class CreateAstronautDutyHandler : IRequestHandler<CreateAstronautDuty, CreateAstronautDutyResult>
    {
        private readonly StargateContext _context;

        public CreateAstronautDutyHandler(StargateContext context)
        {
            _context = context;
        }
        public async Task<CreateAstronautDutyResult> Handle(CreateAstronautDuty request, CancellationToken cancellationToken)
        {
            var person = await _context.People.AsNoTracking().Where(z => z.Name == request.Name)
                .Include(p => p.AstronautDuties)
                .Include(p => p.AstronautDetail)
                .SingleAsync(cancellationToken);

            if (person.AstronautDetail is null)
            {
                var astronautDetail = new AstronautDetail { 
                    PersonId = person.Id,
                    CurrentDutyTitle = request.DutyTitle,
                    CurrentRank = request.Rank,
                    CareerStartDate = request.DutyStartDate.Date,
                    CareerEndDate = request.DutyTitle == "RETIRED" ? request.DutyStartDate.Date : null
                };

                await _context.AstronautDetails.AddAsync(astronautDetail, cancellationToken);
                await _context.SuccessLog.AddAsync(new SuccessLog
                {
                    Process = "Create Astronaut Duty: Create Astronaut Detail",
                    Message = $"Name: {person.Name}, Title: {request.DutyTitle}, Rank: {request.Rank}, StartDate: {astronautDetail.CareerStartDate}, EndDate {astronautDetail.CareerEndDate}"
                }, cancellationToken);
            }
            else
            {
                person.AstronautDetail.CurrentDutyTitle = request.DutyTitle;
                person.AstronautDetail.CurrentRank = request.Rank;
                if (request.DutyTitle == "RETIRED")
                {
                    person.AstronautDetail.CareerEndDate = request.DutyStartDate.AddDays(-1).Date;
                }

                _context.AstronautDetails.Update(person.AstronautDetail); 
                await _context.SuccessLog.AddAsync(new SuccessLog
                {
                    Process = "Create Astronaut Duty: Update Astronaut Detail",
                    Message = $"Name: {person.Name}, Title: {request.DutyTitle}, Rank: {request.Rank}, StartDate: {person.AstronautDetail.CareerStartDate}, EndDate {person.AstronautDetail.CareerEndDate}"
                }, cancellationToken);
            }

            var mostRecentDuty = person.AstronautDuties
                .OrderByDescending(ad => ad.DutyStartDate)
                .FirstOrDefault();

            if (mostRecentDuty != null)
            {
                mostRecentDuty.DutyEndDate = request.DutyStartDate.AddDays(-1).Date;
                _context.AstronautDuties.Update(mostRecentDuty);
                await _context.SuccessLog.AddAsync(new SuccessLog
                {
                    Process = "Create Astronaut Duty: Update Astronaut Duty End Date",
                    Message = $"Duty Id: {mostRecentDuty.Id}, EndDate {mostRecentDuty.DutyEndDate}"
                }, cancellationToken);
            }

            var newAstronautDuty = new AstronautDuty()
            {
                PersonId = person.Id,
                Rank = request.Rank,
                DutyTitle = request.DutyTitle,
                DutyStartDate = request.DutyStartDate.Date,
                DutyEndDate = null
            };

            await _context.AstronautDuties.AddAsync(newAstronautDuty, cancellationToken);
            await _context.SuccessLog.AddAsync(new SuccessLog
            {
                Process = "Create Astronaut Duty: Add Astronaut Duty",
                Message = $"Name: {person.Name}, Title: {request.DutyTitle}, Rank: {request.Rank}, StartDate: {newAstronautDuty.DutyStartDate}"
            }, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return new CreateAstronautDutyResult()
            {
                Id = newAstronautDuty.Id
            };
        }
    }

    public class CreateAstronautDutyResult : BaseResponse
    {
        public int? Id { get; set; }
    }
}

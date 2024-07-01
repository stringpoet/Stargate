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

        public Task Process(CreateAstronautDuty request, CancellationToken cancellationToken)
        {
            var person = _context.People.AsNoTracking().FirstOrDefault(z => z.Name == request.Name);

            if (person is null)
            {
                _context.ExceptionLog.Add(new ExceptionLog { Process = "Create Astronaut Duty", Message = $"Person Not Found: {request.Name}" });
                _context.SaveChanges();
                throw new BadHttpRequestException("Person not found");
            }

            var verifyNoPreviousDuty = _context.AstronautDuties.FirstOrDefault(z => z.DutyTitle == request.DutyTitle && z.DutyStartDate == request.DutyStartDate);

            if (verifyNoPreviousDuty is not null)
            {
                _context.ExceptionLog.Add(new ExceptionLog { Process = "Create Astronaut Duty", Message = $"{person.Name} already has a duty of {verifyNoPreviousDuty.DutyTitle} with a start date of {verifyNoPreviousDuty.DutyStartDate}" });
                _context.SaveChanges();
                throw new BadHttpRequestException("Duty already exists");
            }

            return Task.CompletedTask;
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
            var person = await _context.People.FirstAsync(p => p.Name == request.Name, cancellationToken);

            var astronautDetail = await _context.AstronautDetails.FirstOrDefaultAsync(ad => ad.PersonId == person.Id, cancellationToken);

            if (astronautDetail == null)
            {
                astronautDetail = new AstronautDetail { 
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
                astronautDetail.CurrentDutyTitle = request.DutyTitle;
                astronautDetail.CurrentRank = request.Rank;
                if (request.DutyTitle == "RETIRED")
                {
                    astronautDetail.CareerEndDate = request.DutyStartDate.AddDays(-1).Date;
                }

                _context.AstronautDetails.Update(astronautDetail); 
                await _context.SuccessLog.AddAsync(new SuccessLog
                {
                    Process = "Create Astronaut Duty: Update Astronaut Detail",
                    Message = $"Name: {person.Name}, Title: {request.DutyTitle}, Rank: {request.Rank}, StartDate: {astronautDetail.CareerStartDate}, EndDate {astronautDetail.CareerEndDate}"
                }, cancellationToken);
            }

            var mostRecentDuty = await _context.AstronautDuties
                .Where(ad => ad.PersonId == person.Id)
                .OrderByDescending(ad => ad.DutyStartDate)
                .FirstOrDefaultAsync(cancellationToken);

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

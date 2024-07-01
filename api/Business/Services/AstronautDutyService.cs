using StargateAPI.Business.Commands;
using StargateAPI.Business.Data;
using StargateAPI.Business.Repositories;

namespace StargateAPI.Business.Services
{
    public class AstronautDutyService : IAstronautDutyService
    {
        private readonly IPersonRepository _personRepository;
        private readonly ILogService _logService;

        public AstronautDutyService(IPersonRepository personRepository, ILogService logService)
        {
            _personRepository = personRepository;
            _logService = logService;
        }

        public async Task<CreateAstronautDutyResult> CreateDutyAsync(CreateAstronautDuty request, CancellationToken cancellationToken)
        {
            var person = await _personRepository.GetByNameAsync(request.Name, cancellationToken);

            if (person == null)
            {
                throw new BadHttpRequestException("Person not found");
            }

            if (person.AstronautDuties.Any(d => d.DutyTitle == request.DutyTitle && d.DutyStartDate == request.DutyStartDate))
            {
                await _logService.LogExceptionAsync("Create Astronaut Duty", $"{person.Name} already has a duty of {request.DutyTitle} with a start date of {request.DutyStartDate}", cancellationToken);
                throw new BadHttpRequestException("Duty already exists");
            }

            if (person.AstronautDetail == null)
            {
                var astronautDetail = new AstronautDetail
                {
                    PersonId = person.Id,
                    CurrentDutyTitle = request.DutyTitle,
                    CurrentRank = request.DutyTitle == "RETIRED" ? "" : request.Rank,
                    CareerStartDate = request.DutyStartDate.Date,
                    CareerEndDate = request.DutyTitle == "RETIRED" ? request.DutyStartDate.Date : null
                };

                person.AstronautDetail = astronautDetail;
                await _logService.LogSuccessAsync("Create Astronaut Duty: Create Astronaut Detail", $"Name: {person.Name}, Title: {request.DutyTitle}, Rank: {request.Rank}, StartDate: {astronautDetail.CareerStartDate}, EndDate: {astronautDetail.CareerEndDate}", cancellationToken);
            }
            else
            {
                person.AstronautDetail.CurrentDutyTitle = request.DutyTitle;
                person.AstronautDetail.CurrentRank = request.DutyTitle == "RETIRED" ? "" : request.Rank;
                if (request.DutyTitle == "RETIRED")
                {
                    person.AstronautDetail.CareerEndDate = request.DutyStartDate.AddDays(-1).Date;
                }
                await _logService.LogSuccessAsync("Create Astronaut Duty: Update Astronaut Detail", $"Name: {person.Name}, Title: {request.DutyTitle}, Rank: {request.Rank}, StartDate: {person.AstronautDetail.CareerStartDate}, EndDate: {person.AstronautDetail.CareerEndDate}", cancellationToken);
            }

            var mostRecentDuty = person.AstronautDuties
                .OrderByDescending(ad => ad.DutyStartDate)
                .FirstOrDefault();

            if (mostRecentDuty != null)
            {
                mostRecentDuty.DutyEndDate = request.DutyStartDate.AddDays(-1).Date;
                await _logService.LogSuccessAsync("Create Astronaut Duty: Update Astronaut Duty End Date", $"Duty Id: {mostRecentDuty.Id}, EndDate: {mostRecentDuty.DutyEndDate}", cancellationToken);
            }

            var newAstronautDuty = new AstronautDuty
            {
                PersonId = person.Id,
                Rank = request.Rank,
                DutyTitle = request.DutyTitle,
                DutyStartDate = request.DutyStartDate.Date,
                DutyEndDate = null
            };

            person.AstronautDuties.Add(newAstronautDuty);

            await _logService.LogSuccessAsync("Create Astronaut Duty: Add Astronaut Duty", $"Name: {person.Name}, Title: {request.DutyTitle}, Rank: {request.Rank}, StartDate: {newAstronautDuty.DutyStartDate}", cancellationToken);

            await _personRepository.SaveChangesAsync(cancellationToken);

            return new CreateAstronautDutyResult
            {
                Id = newAstronautDuty.Id
            };
        }
    }
}

using MediatR;
using MediatR.Pipeline;
using StargateAPI.Business.Repositories;
using StargateAPI.Business.Services;
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
        private readonly IPersonRepository _personRepository;
        private readonly ILogService _logService;

        public CreateAstronautDutyPreProcessor(IPersonRepository personRepository, ILogService logService)
        {
            _personRepository = personRepository;
            _logService = logService;
        }

        public async Task Process(CreateAstronautDuty request, CancellationToken cancellationToken)
        {
            var person = await _personRepository.GetByNameAsync(request.Name, cancellationToken);

            if (person == null)
            {
                await _logService.LogExceptionAsync("Create Astronaut Duty", $"Person Not Found: {request.Name}", cancellationToken);
                throw new BadHttpRequestException("Person not found");
            }

            if (person.AstronautDuties.Any(d => d.DutyTitle == request.DutyTitle && d.DutyStartDate == request.DutyStartDate))
            {
                await _logService.LogExceptionAsync("Create Astronaut Duty", $"{person.Name} already has a duty of {request.DutyTitle} with a start date of {request.DutyStartDate}", cancellationToken);
                throw new BadHttpRequestException("Duty already exists");
            }
        }
    }

    public class CreateAstronautDutyHandler : IRequestHandler<CreateAstronautDuty, CreateAstronautDutyResult>
    {
        private readonly IAstronautDutyService _astronautDutyService;

        public CreateAstronautDutyHandler(IAstronautDutyService astronautDutyService)
        {
            _astronautDutyService = astronautDutyService;
        }

        public Task<CreateAstronautDutyResult> Handle(CreateAstronautDuty request, CancellationToken cancellationToken)
        {
            return _astronautDutyService.CreateDutyAsync(request, cancellationToken);
        }
    }

    public class CreateAstronautDutyResult : BaseResponse
    {
        public int? Id { get; set; }
    }
}

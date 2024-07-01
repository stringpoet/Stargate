using MediatR;
using MediatR.Pipeline;
using StargateAPI.Business.Repositories;
using StargateAPI.Business.Services;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Commands
{
    public class CreatePerson : IRequest<CreatePersonResult>
    {
        public required string Name { get; set; } = string.Empty;
    }

    public class CreatePersonPreProcessor : IRequestPreProcessor<CreatePerson>
    {
        private readonly IPersonRepository _personRepository;
        private readonly ILogService _logService;

        public CreatePersonPreProcessor(IPersonRepository personRepository, ILogService logService)
        {
            _personRepository = personRepository;
            _logService = logService;
        }

        public async Task Process(CreatePerson request, CancellationToken cancellationToken)
        {
            var person = await _personRepository.GetByNameAsync(request.Name, cancellationToken);

            if (person != null)
            {
                await _logService.LogExceptionAsync("Create Person", $"{request.Name} already exists", cancellationToken);
                throw new BadHttpRequestException("Person already exists");
            }
        }
    }

    public class CreatePersonHandler : IRequestHandler<CreatePerson, CreatePersonResult>
    {
        private readonly IPersonService _personService;

        public CreatePersonHandler(IPersonService personService)
        {
            _personService = personService;
        }

        public Task<CreatePersonResult> Handle(CreatePerson request, CancellationToken cancellationToken)
        {
            return _personService.CreatePersonAsync(request, cancellationToken);
        }
    }

    public class CreatePersonResult : BaseResponse
    {
        public int Id { get; set; }
    }
}

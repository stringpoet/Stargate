using StargateAPI.Controllers;
using MediatR;
using MediatR.Pipeline;
using StargateAPI.Business.Repositories;
using StargateAPI.Business.Services;

namespace StargateAPI.Business.Commands
{
    public class UpdatePerson : IRequest<BaseResponse>
    {
        public required string Name { get; set; }
        public required string NewName { get; set; }
    }

    public class UpdatePersonPreProcessor : IRequestPreProcessor<UpdatePerson>
    {
        private readonly IPersonRepository _personRepository;
        private readonly ILogService _logService;

        public UpdatePersonPreProcessor(IPersonRepository personRepository, ILogService logService)
        {
            _personRepository = personRepository;
            _logService = logService;
        }

        public async Task Process(UpdatePerson request, CancellationToken cancellationToken)
        {
            var person = await _personRepository.GetByNameAsync(request.Name, cancellationToken);

            if (person == null)
            {
                await _logService.LogExceptionAsync("Update Person", $"{request.Name} not found", cancellationToken);
                throw new BadHttpRequestException("Person not found");
            }
        }
    }

    public class UpdatePersonHandler : IRequestHandler<UpdatePerson, BaseResponse>
    {
        private readonly IPersonService _personService;

        public UpdatePersonHandler(IPersonService personService)
        {
            _personService = personService;
        }

        public Task<BaseResponse> Handle(UpdatePerson request, CancellationToken cancellationToken)
        {
            return _personService.UpdatePersonAsync(request, cancellationToken);
        }
    }
}

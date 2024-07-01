using StargateAPI.Business.Commands;
using StargateAPI.Business.Data;
using StargateAPI.Business.Repositories;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Services
{
    public class PersonService : IPersonService
    {
        private readonly IPersonRepository _personRepository;
        private readonly ILogService _logService;

        public PersonService(IPersonRepository personRepository, ILogService logService)
        {
            _personRepository = personRepository;
            _logService = logService;
        }
        public async Task<BaseResponse> UpdatePersonAsync(UpdatePerson request, CancellationToken cancellationToken)
        {
            var person = await _personRepository.GetByNameAsync(request.Name, cancellationToken);

            if (person == null)
            {
                await _logService.LogExceptionAsync("Update Person", $"{request.Name} not found", cancellationToken);
                throw new BadHttpRequestException("Person not found");
            }

            person.Name = request.NewName;

            await _personRepository.UpdatePersonAsync(person, cancellationToken);
            await _logService.LogSuccessAsync("Update Person", $"Name: {request.Name}, New Name: {request.NewName}", cancellationToken);

            return new BaseResponse
            {
                Success = true,
                Message = "Person updated successfully",
                ResponseCode = 200
            };
        }

        public async Task<CreatePersonResult> CreatePersonAsync(CreatePerson request, CancellationToken cancellationToken)
        {
            var person = await _personRepository.GetByNameAsync(request.Name, cancellationToken);

            if (person != null)
            {
                await _logService.LogExceptionAsync("Create Person", $"{request.Name} already exists", cancellationToken);
                throw new BadHttpRequestException("Person already exists");
            }

            var newPerson = new Person { Name = request.Name };
            await _personRepository.AddAsync(newPerson, cancellationToken);
            await _logService.LogSuccessAsync("Create Person", $"Name: {request.Name}", cancellationToken);

            return new CreatePersonResult { Id = newPerson.Id };
        }
    }
}

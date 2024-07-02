using MediatR;
using StargateAPI.Business.Dtos;
using StargateAPI.Business.Repositories;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Queries
{
    public class GetPeople : IRequest<GetPeopleResult>
    {

    }

    public class GetPeopleHandler : IRequestHandler<GetPeople, GetPeopleResult>
    {
        private readonly IPersonRepository _personRepository;

        public GetPeopleHandler(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }

        public async Task<GetPeopleResult> Handle(GetPeople request, CancellationToken cancellationToken)
        {
            var result = new GetPeopleResult
            {
                People = await _personRepository.GetAllAsync(cancellationToken)
            };

            return result;
        }
    }

    public class GetPeopleResult : BaseResponse
    {
        public ICollection<PersonAstronaut> People { get; set; } = new List<PersonAstronaut> { };

    }
}

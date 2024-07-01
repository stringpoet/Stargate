using StargateAPI.Business.Commands;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Services
{
    public interface IPersonService
    {
        Task<CreatePersonResult> CreatePersonAsync(CreatePerson request, CancellationToken cancellationToken);
        Task<BaseResponse> UpdatePersonAsync(UpdatePerson request, CancellationToken cancellationToken);
    }
}

using StargateAPI.Business.Commands;

namespace StargateAPI.Business.Services
{
    public interface IAstronautDutyService
    {
        Task<CreateAstronautDutyResult> CreateDutyAsync(CreateAstronautDuty request, CancellationToken cancellationToken);
    }
}

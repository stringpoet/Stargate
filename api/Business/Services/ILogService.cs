namespace StargateAPI.Business.Services
{
    public interface ILogService
    {
        Task LogSuccessAsync(string process, string message, CancellationToken cancellationToken);
        Task LogExceptionAsync(string process, string message, CancellationToken cancellationToken);

    }
}

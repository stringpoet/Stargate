using StargateAPI.Business.Data;

namespace StargateAPI.Business.Services
{
    public class LogService : ILogService
    {
        private readonly StargateContext _context;

        public LogService(StargateContext context) => _context = context;

        public async Task LogSuccessAsync(string process, string message, CancellationToken cancellationToken)
        {
            await _context.SuccessLogs.AddAsync(new SuccessLog { Process = process, Message = message }, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task LogExceptionAsync(string process, string message, CancellationToken cancellationToken)
        {
            await _context.ExceptionLogs.AddAsync(new ExceptionLog { Process = process, Message = message }, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}

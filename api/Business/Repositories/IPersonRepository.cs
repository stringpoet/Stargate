using StargateAPI.Business.Data;

namespace StargateAPI.Business.Repositories
{
    public interface IPersonRepository
    {
        Task<Person?> GetByNameAsync(string name, CancellationToken cancellationToken);
        Task AddAsync(Person person, CancellationToken cancellationToken);
        Task UpdatePersonAsync(Person person, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}

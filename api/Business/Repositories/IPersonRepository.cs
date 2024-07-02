using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;

namespace StargateAPI.Business.Repositories
{
    public interface IPersonRepository
    {
        Task<Person?> GetByNameAsync(string name, CancellationToken cancellationToken);
        Task<ICollection<PersonAstronaut>> GetAllAsync(CancellationToken cancellationToken);
        Task AddAsync(Person person, CancellationToken cancellationToken);
        Task UpdatePersonAsync(Person person, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}

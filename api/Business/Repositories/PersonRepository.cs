using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
using StargateAPI.Business.Services;

namespace StargateAPI.Business.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly StargateContext _context;

        public PersonRepository(StargateContext context) => _context = context;

        public async Task<Person?> GetByNameAsync(string name, CancellationToken cancellationToken)
        {
            return await _context.People
                .Include(p => p.AstronautDuties)
                .Include(p => p.AstronautDetail)
                .SingleOrDefaultAsync(p => p.Name == name, cancellationToken);
        }

        public async Task AddAsync(Person person, CancellationToken cancellationToken)
        {
            await _context.People.AddAsync(person, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdatePersonAsync(Person person, CancellationToken cancellationToken)
        {
            _context.People.Update(person);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<ICollection<PersonAstronaut>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.People
                .Include(p => p.AstronautDetail)
                .Select(p => PersonMapper.ToPersonAstronaut(p))
                .ToListAsync(cancellationToken);
        }
    }
}

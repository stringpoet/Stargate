using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;

namespace StargateAPI.Business.Services
{
    public static class PersonMapper
    {
        public static PersonAstronaut ToPersonAstronaut(Person? person)
        {
            ArgumentNullException.ThrowIfNull(person);

            return new PersonAstronaut
            {
                PersonId = person.Id,
                Name = person.Name ?? string.Empty,
                CurrentRank = person.AstronautDetail?.CurrentRank ?? string.Empty,
                CurrentDutyTitle = person.AstronautDetail?.CurrentDutyTitle ?? string.Empty,
                CareerStartDate = person.AstronautDetail?.CareerStartDate,
                CareerEndDate = person.AstronautDetail?.CareerEndDate
            };
        }
    }
}

using DesafioTecnico.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesafioTecnico.Domain.Repositories.Interfaces
{
    public interface IDroneTravelRepository : IGenericRepository<DroneTravel, long>
    {
        Task<IEnumerable<DroneTravel>> FindLastTravels(int amoutToTake);
    }
}

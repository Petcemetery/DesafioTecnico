using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using JHipsterNet.Core.Pagination;
using JHipsterNet.Core.Pagination.Extensions;
using DesafioTecnico.Domain;
using DesafioTecnico.Domain.Repositories.Interfaces;
using DesafioTecnico.Infrastructure.Data.Extensions;
using DesafioTecnico.Domain.Entities;
using System.Diagnostics;

namespace DesafioTecnico.Infrastructure.Data.Repositories
{
    public class DroneTravelRepository : GenericRepository<DroneTravel, long>, IDroneTravelRepository
    {
        //private readonly AppContext _context;


        public DroneTravelRepository(IUnitOfWork context) : base(context)
        {
        }

        public async Task<IEnumerable<DroneTravel>> FindLastTravels(int amoutToTake) {

            return await GetAmountAsync(amoutToTake);
        }
    }
}

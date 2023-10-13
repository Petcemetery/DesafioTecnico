// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this

using DesafioTecnico.Crosscutting;
using DesafioTecnico.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesafioTecnico.Domain.Services.Interfaces
{
    public interface IDroneService
    {
        Task<DroneTravel> SaveTravel(string startingPosition, string objPosition, string finalDestination, double elapsedTime, string route);
        Task<ResultTravel> CalculateTravel(string a, string b, TimesVsPositions positionVsTime);
        Task<IEnumerable<DroneTravel>> GetLastTravels(int amoutToTake);
    }
}

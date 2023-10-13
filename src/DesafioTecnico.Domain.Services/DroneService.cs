// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this

using DesafioTecnico.Crosscutting;
using DesafioTecnico.Domain.Entities;
using DesafioTecnico.Domain.Repositories.Interfaces;
using DesafioTecnico.Domain.Services.Interfaces;
using DesafioTecnico.Dto;
using DesafioTecnico.Infrastructure.Data.Repositories;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesafioTecnico.Domain.Services
{
    public class DroneService : IDroneService
    {
        protected readonly IDroneTravelRepository _droneTravelRepository;

        public DroneService(IDroneTravelRepository droneRepository)
        {
            _droneTravelRepository = droneRepository;
        }
        /// <summary>
        /// Recieve two locations A and B and the values
        /// to travel to each location and calculate the fastest route
        /// </summary>
        /// <param name="a">starting location</param>
        /// <param name="b">Ending location</param>
        /// <param name="positionVsTime">Dictionary<string, Dictionary<string, double>> {"A1":{"A2":11.88,"B1":10.46}} </param>
        /// <returns></returns>
        public virtual async Task<ResultTravel> CalculateTravel(string a, string b, TimesVsPositions positionVsTime)
        {
            ResultTravel result = new ResultTravel();

            result.startingPoint = a;
            result.finalDestination = b;
            bool arrived = false;
            string wholePath = a + "-";
            double timeTaken = 0;
            string currentPosition = a;

            while (arrived == false)
            {
                var currentOptions = positionVsTime.timesVsPositions.Where(c => c.Key == currentPosition).FirstOrDefault().Value;
                if (currentPosition == b)
                {
                    arrived = true;
                    break;
                }

                if (currentPosition[0] == b[0])//we could probably add this validation with the other like this:  if (currentPosition[0] <= b[0])
                {
                    //we are currently in the same col
                    #region sameCol
                    if (currentPosition[1] > b[1])
                    {
                        //we have to go down
                        string positionWeGoingTo = currentPosition[0] + ((int)currentPosition[1] - 48 - 1).ToString();
                        timeTaken = timeTaken + currentOptions.Where(c => c.Key == positionWeGoingTo).First().Value;
                        currentPosition = positionWeGoingTo;//Moving
                        wholePath = wholePath + positionWeGoingTo + "+" + positionWeGoingTo + "-";
                    }

                    if (currentPosition[1] < b[1])
                    {
                        //we have to go up
                        string positionWeGoingTo = currentPosition[0] + ((int)currentPosition[1] - 48 + 1).ToString();
                        timeTaken = timeTaken + currentOptions.Where(c => c.Key == positionWeGoingTo).First().Value;
                        currentPosition = positionWeGoingTo;//Moving
                        wholePath = wholePath + positionWeGoingTo + "+" + positionWeGoingTo + "-";
                    }
                    if (currentPosition[1] == b[1])
                        arrived = true;    //we arrived
                    #endregion

                }
                else if (currentPosition[1] == b[1])
                {
                    //we are in the same row
                    #region sameRow
                    if (currentPosition[0] < b[0])
                    {
                        //we have to go right
                        string positionWeGoingTo = ((char)(currentPosition[0] + 1) + currentPosition[1].ToString());
                        timeTaken = timeTaken + currentOptions.Where(c => c.Key == positionWeGoingTo).First().Value;
                        currentPosition = positionWeGoingTo;//Moving
                        wholePath = wholePath + positionWeGoingTo + "+" + positionWeGoingTo + "-";
                    }

                    if (currentPosition[0] > b[0])
                    {
                        //we have to go left
                        string positionWeGoingTo = ((char)(currentPosition[0] - 1) + currentPosition[1].ToString());
                        timeTaken = timeTaken + currentOptions.Where(c => c.Key == positionWeGoingTo).First().Value;
                        currentPosition = positionWeGoingTo;//Moving
                        wholePath = wholePath + positionWeGoingTo + "+" + positionWeGoingTo + "-";
                    }
                    if (currentPosition[1] == b[1])
                        arrived = true;    //we arrived
                    #endregion
                }
                else if (currentPosition[0] < b[0])
                {
                    if (currentPosition[1] > b[1])
                    {
                        //we have to go right or down
                        //calculate to see wich one
                        string positionAtRight = ((char)(currentPosition[0] + 1) + currentPosition[1].ToString());
                        double costRight = currentOptions.Where(c => c.Key == positionAtRight).First().Value;
                        string positionAtDown = currentPosition[0] + ((int)currentPosition[1] - 48 - 1).ToString();
                        double costDown = currentOptions.Where(c => c.Key == positionAtDown).First().Value;
                        if (costRight <= costDown)
                        {
                            //We going right
                            timeTaken = timeTaken + currentOptions.Where(c => c.Key == positionAtRight).First().Value;
                            currentPosition = positionAtRight;//Moving
                            wholePath = wholePath + positionAtRight + "+" + positionAtRight + "-";
                        }
                        else
                        {
                            //we going down
                            timeTaken = timeTaken + currentOptions.Where(c => c.Key == positionAtDown).First().Value;
                            currentPosition = positionAtDown;//Moving
                            wholePath = wholePath + positionAtDown + "+" + positionAtDown + "-";
                        }

                        if (currentPosition[1] == b[1] && currentPosition[0] == b[0])
                            arrived = true;//we arrived
                    }
                    else
                    {
                        //We have to go right or up
                        //calculate to see wich one
                        string positionAtRight = ((char)(currentPosition[0] + 1) + currentPosition[1].ToString());
                        double costRight = currentOptions.Where(c => c.Key == positionAtRight).First().Value;
                        string positionAtUp = currentPosition[0] + ((int)currentPosition[1] - 48 + 1).ToString();
                        double costDUp = currentOptions.Where(c => c.Key == positionAtUp).First().Value;
                        if (costRight <= costDUp)
                        {
                            //We going right
                            timeTaken = timeTaken + currentOptions.Where(c => c.Key == positionAtRight).First().Value;
                            currentPosition = positionAtRight;//Moving
                            wholePath = wholePath + positionAtRight + "+" + positionAtRight + "-";
                        }
                        else
                        {
                            //we going up
                            timeTaken = timeTaken + currentOptions.Where(c => c.Key == positionAtUp).First().Value;
                            currentPosition = positionAtUp;//Moving
                            wholePath = wholePath + positionAtUp + "+" + positionAtUp + "-";
                        }

                        if (currentPosition[1] == b[1] && currentPosition[0] == b[0])
                            arrived = true;   //we arrived
                    }
                }
                else
                {
                    if (currentPosition[1] > b[1])
                    {
                        //We have to got left or down
                        //Calculate to see wich one
                        string positionAtLeft = ((char)(currentPosition[0] - 1) + currentPosition[1].ToString());
                        double costLeft = currentOptions.Where(c => c.Key == positionAtLeft).First().Value;
                        string positionAtDown = currentPosition[0] + ((int)currentPosition[1] - 48 - 1).ToString();
                        double costDown = currentOptions.Where(c => c.Key == positionAtDown).First().Value;
                        if (costLeft <= costDown)
                        {
                            //We going left
                            timeTaken = timeTaken + currentOptions.Where(c => c.Key == positionAtLeft).First().Value;
                            currentPosition = positionAtLeft;//Moving
                            wholePath = wholePath + positionAtLeft + "+" + positionAtLeft + "-";
                        }
                        else
                        {
                            //we going down
                            timeTaken = timeTaken + currentOptions.Where(c => c.Key == positionAtDown).First().Value;
                            currentPosition = positionAtDown;//Moving
                            wholePath = wholePath + positionAtDown + "+" + positionAtDown + "-";
                        }
                        if (currentPosition[1] == b[1] && currentPosition[0] == b[0])
                            arrived = true;    //we arrived

                    }
                    else
                    {
                        //We have to got left or up
                        //Calculate to see wich one

                        string positionAtLeft = ((char)(currentPosition[0] - 1) + currentPosition[1].ToString());
                        double costLeft = currentOptions.Where(c => c.Key == positionAtLeft).First().Value;
                        string positionAtUp = currentPosition[0] + ((int)currentPosition[1] - 48 + 1).ToString();
                        double costUp = currentOptions.Where(c => c.Key == positionAtUp).First().Value;
                        if (costLeft <= costUp)
                        {
                            //We going left
                            timeTaken = timeTaken + currentOptions.Where(c => c.Key == positionAtLeft).First().Value;
                            currentPosition = positionAtLeft;//Moving
                            wholePath = wholePath + positionAtLeft + "+" + positionAtLeft + "-";

                        }
                        else
                        {
                            //we going up
                            timeTaken = timeTaken + currentOptions.Where(c => c.Key == positionAtUp).First().Value;
                            currentPosition = positionAtUp;//Moving
                            wholePath = wholePath + positionAtUp + "+" + positionAtUp + "-";
                        }

                        if (currentPosition[1] == b[1] && currentPosition[0] == b[0])
                            arrived = true;   //we arrived
                    }
                }
            }

            result.routeTraveled = (wholePath + currentPosition);
            result.elapsedTime = timeTaken;
            return result;
        }

        public async Task<IEnumerable<DroneTravel>> GetLastTravels(int amountToTake)
        {

            return await _droneTravelRepository.FindLastTravels(amountToTake);
        }

        public virtual async Task<DroneTravel> SaveTravel(string startingPosition, string objPosition, string finalDestination, double elapsedTime, string route)
        {
            DroneTravel droneTravelToInsert = new DroneTravel()
            {
                startingPosition = startingPosition,
                objPosition = objPosition,
                finalPosition = finalDestination,
                elapsedTime = elapsedTime,
                route = route,
                date = DateTime.Now
            };
            await _droneTravelRepository.CreateOrUpdateAsync(droneTravelToInsert);
            await _droneTravelRepository.SaveChangesAsync();
            return droneTravelToInsert;
        }
    }
}


using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using JHipsterNet.Core.Pagination;
using DesafioTecnico.Domain;
using DesafioTecnico.Crosscutting.Exceptions;
using DesafioTecnico.Web.Extensions;
using DesafioTecnico.Web.Filters;
using DesafioTecnico.Web.Rest.Utilities;
using DesafioTecnico.Domain.Repositories.Interfaces;
using DesafioTecnico.Domain.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using DesafioTecnico.Dto;
using AutoMapper.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Extensions;
using System;
using DesafioTecnico.Crosscutting;
using System.Net.Http.Headers;
using System.Net.Http;
using LanguageExt.Pipes;
using Newtonsoft.Json;
using System.Reflection.Emit;
using System.Linq;

namespace DesafioTecnico.Controllers
{

    [Route("api/Drone")]
    [ApiController]
    public class DroneController : ControllerBase
    {
        //Number of rows and columns in the board
        private static readonly int MAX_ROWS = 8;
        private static readonly int MAX_COLS = 8;

        //Ways the first row and column are represented in chess notation
        private static readonly char MIN_ROW_CHAR = 'A';
        private static readonly char MIN_COL_CHAR = '1';

        private readonly ILogger<DroneController> _log;
        private readonly IDroneService _droneService;


        public DroneController(ILogger<DroneController> log,
        IDroneService droneService)
        {
            _log = log;
            _droneService = droneService;
        }

        [HttpPost("CalculateTravel")]
        public async Task<ActionResult<DroneResultTravel>> CalculateTravel([FromBody] DroneTravelRequest droneTravelRequest)
        {
            string error = Validate(droneTravelRequest);


            if (!string.IsNullOrEmpty(error))
                return BadRequest(error);
            else
            {
                DroneResultTravel result = new DroneResultTravel();
                result.startingPoint = droneTravelRequest.startingPoint;
                result.objLocation = droneTravelRequest.objLocation;
                result.finalDestination = droneTravelRequest.finalDestination;

                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync("https://mocki.io/v1/10404696-fd43-4481-a7ed-f9369073252f");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                //var timesAndPositions = JsonConvert.DeserializeObject<Root>(responseBody);
                TimesVsPositions timesAndPositions = new TimesVsPositions();
                timesAndPositions.timesVsPositions = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, double>>>(responseBody);


                ResultTravel travelToObj = await _droneService.CalculateTravel(droneTravelRequest.startingPoint, droneTravelRequest.objLocation, timesAndPositions);
                result.routeTraveled = travelToObj.routeTraveled;
                result.elapsedTime = travelToObj.elapsedTime;

                ResultTravel finalTravel = await _droneService.CalculateTravel(droneTravelRequest.objLocation, droneTravelRequest.finalDestination, timesAndPositions);
                result.routeTraveled = result.routeTraveled + "+" + finalTravel.routeTraveled;
                result.elapsedTime = result.elapsedTime + finalTravel.elapsedTime;

                await _droneService.SaveTravel(result.startingPoint, result.objLocation, result.finalDestination, result.elapsedTime, result.routeTraveled);
                var lastTravels = await _droneService.GetLastTravels(10);
                string travelResult = $"The set delivery will have the route {result.routeTraveled}, and will take {result.elapsedTime}seconds to be delivered as fast as possible.\n";

                string lastTravelsText = string.Empty;
                if (lastTravels != null && lastTravels.Count() > 0)
                {
                    lastTravelsText = "Last deliveries: \n";
                    foreach (var t in lastTravels)
                        lastTravelsText += $"From {t.startingPosition}, picking-up at {t.objPosition} to {t.finalPosition} in {t.elapsedTime} seconds.\n";
                }
                return Ok(travelResult + lastTravelsText);
            }
        }

        private string Validate(DroneTravelRequest droneTravelRequest)
        {
            string retorno = string.Empty;

            if (string.IsNullOrEmpty(droneTravelRequest.startingPoint) || string.IsNullOrEmpty(droneTravelRequest.objLocation) || string.IsNullOrEmpty(droneTravelRequest.finalDestination))
                retorno = "Not possible to search for empty params.";

            #region validatingStartingPoint
            if (droneTravelRequest.startingPoint.Length != 2)
                retorno = "Starting Point Invalid. Please Enter a Value from A1 to H8";

            if (droneTravelRequest.startingPoint[0] >= MIN_ROW_CHAR + MAX_ROWS || droneTravelRequest.startingPoint[0] < MIN_ROW_CHAR)
                retorno = "Starting Point Invalid. Please Enter a Value from A1 to H8";

            if (droneTravelRequest.startingPoint[1] >= MIN_COL_CHAR + MAX_COLS || droneTravelRequest.startingPoint[1] < MIN_COL_CHAR)
                retorno = "Starting Point Invalid. Please Enter a Value from A1 to H8";
            #endregion

            #region ValidatingObjLocation
            if (droneTravelRequest.objLocation.Length != 2)
                retorno = "Object Location Invalid. Please Enter a Value from A1 to H8";

            if (droneTravelRequest.objLocation[0] >= MIN_ROW_CHAR + MAX_ROWS || droneTravelRequest.objLocation[0] < MIN_ROW_CHAR)
                retorno = "Object Location Invalid. Please Enter a Value from A1 to H8";

            if (droneTravelRequest.objLocation[1] >= MIN_COL_CHAR + MAX_COLS || droneTravelRequest.objLocation[1] < MIN_COL_CHAR)
                retorno = "Object Location Invalid. Please Enter a Value from A1 to H8";
            #endregion

            #region ValidatingfinalDestination
            if (droneTravelRequest.finalDestination.Length != 2)
                retorno = "Final Destination Invalid. Please Enter a Value from A1 to H8";

            if (droneTravelRequest.finalDestination[0] >= MIN_ROW_CHAR + MAX_ROWS || droneTravelRequest.finalDestination[0] < MIN_ROW_CHAR)
                retorno = "Final Destination Invalid. Please Enter a Value from A1 to H8";

            if (droneTravelRequest.finalDestination[1] >= MIN_COL_CHAR + MAX_COLS || droneTravelRequest.finalDestination[1] < MIN_COL_CHAR)
                retorno = "Final Destination Invalid. Please Enter a Value from A1 to H8";
            #endregion

            return retorno;
        }
    }
}

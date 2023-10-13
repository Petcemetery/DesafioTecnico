
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using DesafioTecnico.Infrastructure.Data;
using DesafioTecnico.Domain;
using DesafioTecnico.Domain.Repositories.Interfaces;
using DesafioTecnico.Test.Setup;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Xunit;
using DesafioTecnico.Domain.Entities;
using System;
using DesafioTecnico.Crosscutting;
using Newtonsoft.Json;
using System.Text;
using System.Collections.Generic;

namespace DesafioTecnico.Test.Controllers
{
    public class DroneControllerIntTest
    {
        //TODO: Learn how to build decent tests!
        public DroneControllerIntTest()
        {
            _factory = new AppWebApplicationFactory<TestStartup>().WithMockUser();
            _client = _factory.CreateClient();

            _droneRepository = _factory.GetRequiredService<IDroneTravelRepository>();


            InitTest();
        }

        private const double DefaultTime = 555;
        

        private const string StartingPositionDefault = "A1";
        private const string ObjtPositionDefault = "F4";
        private const string FinalPositionDefault = "B8";


        private readonly AppWebApplicationFactory<TestStartup> _factory;
        private readonly HttpClient _client;
        private readonly IDroneTravelRepository _droneRepository;

        private DroneTravel _droneTravel;


        private DroneTravel CreateEntity()
        {
            return new DroneTravel
            {
                date = DateTime.Now,
                elapsedTime = DefaultTime,
                finalPosition = FinalPositionDefault,
                objPosition = ObjtPositionDefault,
                startingPosition = StartingPositionDefault,
            };
        }

        private void InitTest()
        {
            _droneTravel = CreateEntity();
        }

        /// <summary>
        /// I'm still learning to create tests, that's why the current test simply don't work, sorry
        /// </summary>
        /// <returns></returns>

        [Fact]
        public async Task CreateDroneTravel()
        {
            var payload2 = new Dictionary<string, string>
            {
              {"startingPoint", "A1"},
              {"finalDestination", "A1"},
              {"objLocation", "A1"}
            };

            string strPayload = JsonConvert.SerializeObject(payload2);
            HttpContent c2 = new StringContent(strPayload, Encoding.UTF8, "application/json");

            var databaseSizeBeforeCreate = await _droneRepository.CountAsync();
            DroneTravelRequest request = new DroneTravelRequest() { startingPoint = StartingPositionDefault, objLocation = ObjtPositionDefault, finalDestination = FinalPositionDefault };
            //var RJSON = TestUtil.ToJsonContent(request);
            var myContent = JsonConvert.SerializeObject(request);
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);


            var payload = "{\"startingPoint\": \"A1\",\"finalDestination\": \"B2\",\"objLocation\": \"A1\",}";

            HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");


            var response = await _client.PostAsync("/api/Drone/CalculateTravel", c);//Returning not found for some reason! (probably not biding the [frombody] class)


            var response2 = await _client.PostAsync("/api/Drone/CalculateTravel", c2);//Returning not found for some reason!

            //var response2 = await _client.PostAsync("/api/Drone/CalculateTravel&startingPoint=A1");
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var travelsInDB = await _droneRepository.GetAllAsync();
            travelsInDB.Count().Should().Be(databaseSizeBeforeCreate + 1);
            var testDroneTravel = travelsInDB.Last();
            testDroneTravel.startingPosition.Should().Be(StartingPositionDefault);
            testDroneTravel.objPosition.Should().Be(ObjtPositionDefault);
            testDroneTravel.finalPosition.Should().Be(FinalPositionDefault);


          
        }

    }
}

using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using IOU.Backend.Functions.Models;

namespace IOU.Backend.Functions.Functions
{
    public class GetAddressInformation
    {
        #region Fields

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        #endregion

        #region Constructors

        public GetAddressInformation(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        #endregion

        [FunctionName(nameof(GetAddressInformation))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "address/{postalCode}/{houseNumber}")] HttpRequest req,
            string postalCode, string houseNumber,
            ILogger log)
        {
            var mapsKey = _configuration.GetValue<string>("mapsKey");
            var mapsClientId = _configuration.GetValue<string>("46630e98-fea4-4749-9050-bf8dd62d9c9c");

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("x-ms-client-id", mapsClientId);
            client.BaseAddress = new Uri("https://atlas.microsoft.com/search/address/structured/");
            var responseMessage = await client.GetAsync($"json?countryCode=NL&api-version=1.0&subscription-key={mapsKey}&postalCode={postalCode.Replace(" ", "")}&streetNumber={houseNumber}");

            var getAddress = JsonConvert.DeserializeObject<SearchAddressRepsonse>(
                await responseMessage.Content.ReadAsStringAsync(), 
                new JsonSerializerSettings 
                { 
                    ContractResolver = new CamelCasePropertyNamesContractResolver() 
                });
            var streetAddress = getAddress.Results.OrderByDescending(ga => ga.Score).FirstOrDefault();

            if (streetAddress != null)
            { 
                var address = new Common.Models.Address
                {
                    City = streetAddress.Address.LocalName,
                    HouseNumber = streetAddress.Address.StreetNumber,
                    Latitude = streetAddress.Position.Lat,
                    Longitude = streetAddress.Position.Lon,
                    Street = streetAddress.Address.StreetName,
                    Zipcode = streetAddress.Address.ExtendedPostalCode
                };
                return new OkObjectResult(address);
            }

            return new NotFoundResult();
        }
    }
}
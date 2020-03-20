using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;

using IOU.Common.Entities;
using IOU.Common.Models;
using Microsoft.AspNetCore.Http;
using IOU.Common;

namespace IOU.Backend.Functions.Functions
{
    public static class Companies
    {
        [FunctionName("CreateCompany")]
        public static async Task<IActionResult> RunCreate(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] Company company,
            [Table(CompanyEntity.TABLE_NAME, Connection = "storageConnectionString")] CloudTable table,
            ILogger log)
        {
            if (company == null || company.Address == null)
            {
                return new BadRequestResult();
            }

            var entity = new CompanyEntity
            {
                AcceptedTerms = company.AcceptedTerms,
                City = company.Address.City,
                RowKey = GenerateSlug(company),
                CompanyName = company.CompanyName,
                CopyAcceptedTerms = company.CopyAcceptedTerms,
                Email = company.Email,
                Firstname = company.ContactFirstName,
                HouseNumber = company.Address.HouseNumber,
                IBAN = company.IBAN,
                Lastname = company.ContactLastName,
                Latitude = company.Address.Latitude,
                Longitude = company.Address.Longitude,
                Street = company.Address.Street,
                Zipcode = company.Address.Zipcode
            };
            await table.ExecuteAsync(TableOperation.Insert(entity));
            company.Slug = entity.Slug;

            return new OkObjectResult(company);
        }

        [FunctionName("GetCompany")]
        public static IActionResult RunGet(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "companies/{slug}")] HttpRequest req,
            [Table(CompanyEntity.TABLE_NAME, Constants.PARTITIONKEY_COMPANY, "{slug}", Connection = "storageConnectionString")] CompanyEntity companyEntity,
            ILogger log)
        {
            if (companyEntity == null)
            {
                return new NotFoundResult();
            }

            var company = new Company
            {
                AcceptedTerms = companyEntity.AcceptedTerms,
                Address = new Address
                { 
                    City = companyEntity.City,
                    HouseNumber = companyEntity.HouseNumber,
                    Latitude = companyEntity.Latitude,
                    Longitude = companyEntity.Longitude,
                    Street = companyEntity.Street,
                    Zipcode = companyEntity.Zipcode
                },
                Slug = companyEntity.RowKey,
                CompanyName = companyEntity.CompanyName,
                CopyAcceptedTerms = companyEntity.CopyAcceptedTerms,
                Email = companyEntity.Email,
                ContactFirstName = companyEntity.Firstname,
                IBAN = companyEntity.IBAN,
                ContactLastName = companyEntity.Lastname,
            };

            return new OkObjectResult(company);
        }

        private static string GenerateSlug(Company company)
        {
            // TODO: implement slug generator that ALWAYS returns a unique slug
            // Think about adding an abbreviation of the city if the slug was taken already
            return company.CompanyName.Replace(" ", ".");
        }
    }
}
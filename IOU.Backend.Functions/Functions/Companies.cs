using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;

using IOU.Common;
using IOU.Common.Entities;
using IOU.Common.Models;

namespace IOU.Backend.Functions.Functions
{
    public static class Companies
    {
        [FunctionName("CreateCompany")]
        public static async Task<IActionResult> RunCreate(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "companies")] Company company,
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
                CompanyName = company.CompanyName,
                CopyAcceptedTerms = company.CopyAcceptedTerms,
                Email = company.Email,
                Firstname = company.ContactFirstName,
                HouseNumber = company.Address.HouseNumber,
                IBAN = company.IBAN,
                Lastname = company.ContactLastName,
                Latitude = company.Address.Latitude,
                Longitude = company.Address.Longitude,
                RowKey = await GenerateSlugAsync(company, table),
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
                CompanyName = companyEntity.CompanyName,
                ContactFirstName = companyEntity.Firstname,
                ContactLastName = companyEntity.Lastname,
                CopyAcceptedTerms = companyEntity.CopyAcceptedTerms,
                Email = companyEntity.Email,
                IBAN = companyEntity.IBAN,
                Slug = companyEntity.RowKey
            };

            return new OkObjectResult(company);
        }

        #region Private methods

        /// <summary>
        /// Method to generate a slug for a company. This slug needs to be unique.
        /// </summary>
        /// <param name="company">Instance of a <see cref="Company"/> to generate the slug for.</param>
        /// <param name="table">Reference to a <see cref="CloudTable"/> with existing companies to check uniqueness of the slug.</param>
        /// <returns>A unique slug for the specified <paramref name="company"/>.</returns>
        /// <remarks>The slug cannot contain '/', '\', '#', '?' or any character that is not supported in an URL.</remarks>
        private static async Task<string> GenerateSlugAsync(Company company, CloudTable table)
        {
            bool found;

            // First remove RowKey disallowed characters and lowercase the rest of the CompanyName.
            var slug = company.CompanyName
                .Replace("/", "")
                .Replace("\\", "")
                .Replace("#", "")
                .Replace("?", "").ToLower();

            // Second remove any characters with accents
            slug = Encoding.ASCII.GetString(Encoding.GetEncoding("Cyrillic").GetBytes(slug));

            // Third we remove any characters not allowed in a URL
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");

            // Then replace space with a dash for readability
            slug = slug.Replace(" ", "-");

            found = (await table.ExecuteAsync(TableOperation.Retrieve(Constants.PARTITIONKEY_COMPANY, slug))).Result != null;
            if (found)
            {
                if (company.Address != null && !string.IsNullOrWhiteSpace(company.Address.City) && company.Address.City.Length >= 3)
                {
                    slug += "-" + company.Address.City.ToLower().Substring(0, 3);
                    found = (await table.ExecuteAsync(TableOperation.Retrieve(Constants.PARTITIONKEY_COMPANY, slug))).Result != null;
                }
                if (found)
                {
                    int counter = 0;
                    do
                    {
                        counter++;
                        found = (await table.ExecuteAsync(TableOperation.Retrieve(Constants.PARTITIONKEY_COMPANY, $"{slug}-{counter:000}"))).Result != null;
                    } while (found);
                    slug = $"{slug}-{counter:000}";
                }
            }

            return slug;
        }

        #endregion
    }
}
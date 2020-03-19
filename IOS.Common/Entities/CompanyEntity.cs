using Microsoft.WindowsAzure.Storage.Table;

namespace IOU.Common.Entities
{
    public class CompanyEntity : TableEntity
    {
        public const string TABLE_NAME = "Companies";

        public CompanyEntity()
        {
            PartitionKey = Constants.PARTITIONKEY_COMPANY;
        }

        [IgnoreProperty]
        public string CocNumber => RowKey;

        public string CompanyName { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string Street { get; set; }

        public string HouseNumber { get; set; }

        public string Zipcode { get; set; }

        public string City { get; set; }

        public float Longitude { get; set; }

        public float Latitude { get; set; }

        public string Email { get; set; }

        public string IBAN { get; set; }

        public bool AcceptedTerms { get; set; }

        public string CopyAcceptedTerms { get; set; }
    }
}
using Microsoft.WindowsAzure.Storage.Table;

namespace IOU.Common.Entities
{
    public class CompanyEntity : TableEntity
    {
        public CompanyEntity()
        {
            PartitionKey = Constants.PARTITIONKEY_COMPANY;
        }

        public string CompanyName => RowKey;

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string Street { get; set; }

        public int HouseNumber { get; set; }

        public string HouseNumberAddition { get; set; }

        public string Zipcode { get; set; }

        public string City { get; set; }

        public decimal Longitude { get; set; }

        public decimal Latitude { get; set; }

        public string Email { get; set; }

        public string IBAN { get; set; }

        public bool AcceptedTerms { get; set; }

        public string CopyAcceptedTerms { get; set; }
    }
}
namespace IOU.Backend.Functions.Models
{
    public class SearchAddressRepsonse
    {
        public Result[] Results { get; set; }
    }

    public class Result
    {
        public string Type { get; set; }
        public string Id { get; set; }
        public float Score { get; set; }
        public Address Address { get; set; }
        public Position Position { get; set; }
    }

    public class Address
    {
        public string StreetNumber { get; set; }
        public string StreetName { get; set; }
        public string MunicipalitySubdivision { get; set; }
        public string Municipality { get; set; }
        public string CountrySubdivision { get; set; }
        public string PostalCode { get; set; }
        public string ExtendedPostalCode { get; set; }
        public string CountryCode { get; set; }
        public string Country { get; set; }
        public string CountryCodeISO3 { get; set; }
        public string FreeformAddress { get; set; }
        public string LocalName { get; set; }
    }

    public class Position
    {
        public float Lat { get; set; }
        public float Lon { get; set; }
    }

}

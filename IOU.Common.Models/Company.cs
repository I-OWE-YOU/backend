namespace IOU.Common.Models
{
    public class Company
    {
        public string CompanyName { get; set; }

        public string ContactFirstName { get; set; }

        public string ContactLastName { get; set; }

        public Address Address { get; set; }

        public string Email { get; set; }

        public string IBAN { get; set; }

        public bool AcceptedTerms { get; set; }

        public string CopyAcceptedTerms { get; set; }
    }
}
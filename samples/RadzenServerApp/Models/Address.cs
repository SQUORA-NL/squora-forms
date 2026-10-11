namespace RadzenServerApp.Models
{
    public class Address
    {
        public string StreetName { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public int Number { get; set; }

        public bool IsActive { get; set; }
    }
}
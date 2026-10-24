namespace RadzenServerApp.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public Address Address { get; set; }

        public Squora.Forms.Core.Models.UploadedFile? Photo { get; set; }
        public List<Squora.Forms.Core.Models.UploadedFile> Documents { get; set; } = new();

        public List<Car> Cars { get; set; }
    }
}

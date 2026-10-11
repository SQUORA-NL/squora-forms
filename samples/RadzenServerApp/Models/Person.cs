namespace RadzenServerApp.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public Address Address { get; set; }

        public List<Car> Cars { get; set; }
    }
}

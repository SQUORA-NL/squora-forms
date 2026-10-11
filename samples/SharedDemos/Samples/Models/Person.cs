namespace SharedDemos.Samples.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public Address Address { get; set; }

        public double Number { get; set; } = 10;
        public List<Car> Cars { get; set; }
    }
}

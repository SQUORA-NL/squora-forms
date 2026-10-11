namespace RadzenServerApp.Models
{
    public class Car
    {
        public string Make { get; set; }
        public string Model { get; set; }

        public int Rating { get; set; }

        public List<Comment> Comments { get;set;} = new();
     }

    public class Comment
    {
        public string Text { get; set; }
    }
}

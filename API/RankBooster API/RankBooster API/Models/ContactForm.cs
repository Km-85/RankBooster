namespace RankBooster_API.Models
{
    public class ContactForm
    {
        public string Name { get; set; }    
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Website { get; set; }

        // Change this to an array or a list to store multiple services
        public string[] Services { get; set; }

        public string Message { get; set; }
    }
}

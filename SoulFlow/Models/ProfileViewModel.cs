namespace SoulFlow.Models
{
    public class ProfileViewModel
    {
        public string Username { get; set; }


        public string? Bio { get; set; }
        public string? ProfileImage { get; set; }

        public string? UserInterests { get; set; }
        public List<Event> MyEvents { get; set; }
        public List<Event> JoinedEvents { get; set; }
    }
}
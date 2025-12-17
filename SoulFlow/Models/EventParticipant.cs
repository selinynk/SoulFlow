using System;

namespace SoulFlow.Models
{
    public class EventParticipant
    {
        public int Id { get; set; }
        public DateTime JoinedAt { get; set; } = DateTime.Now; 

        
        public string UserId { get; set; }
        public AppUser User { get; set; }

        
        public int EventId { get; set; }
        public Event Event { get; set; }
    }
}
using System;
using System.ComponentModel.DataAnnotations;

namespace SoulFlow.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.Now; 

        
        public string UserId { get; set; }
        public AppUser User { get; set; }

        
        public int EventId { get; set; }
        public Event Event { get; set; }
    }
}
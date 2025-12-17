using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoulFlow.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }

        public string ImageUrl { get; set; }

        [NotMapped]
        public IFormFile ImageFile { get; set; }

        public int Quota { get; set; }

        public string HostId { get; set; }
        [ForeignKey("HostId")]
        public AppUser Host { get; set; }

        public bool IsActive { get; set; } = true;
        public string EventCode { get; set; }
        public int Duration { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<EventParticipant> Participants { get; set; } = new List<EventParticipant>();
    }
}
using System;
using System.ComponentModel.DataAnnotations;
using EgyptTourism.Models;

namespace EgyptTourism
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        public string Text { get; set; }
        public int UserId { get; set; }
        public int DestinationId { get; set; }

        //Navigation Properties
        public User User { get; set; }
        public Destination Destination { get; set; }
        // Time/Date Stamps
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
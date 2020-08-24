using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EgyptTourism.Models
{
    public class Destination
    {
        [Key]
        public int DestinationId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string ImgURL { get; set; }

        // Navigation Properties
        List<Wishlist> Wishlists { get; set; }
        List<Comment> Comments { get; set; }
        // List of Comments
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

    }
}
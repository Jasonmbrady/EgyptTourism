using System.ComponentModel.DataAnnotations;

namespace EgyptTourism.Models
{
    public class Wishlist
    {
        [Key]
        public int WishlistId { get; set; }
        public int DestinationId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public Destination Destination { get; set; }
    }

}
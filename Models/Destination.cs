using System.ComponentModel.DataAnnotations;

namespace EgyptTourism.Models
{
    public class Destination
    {
        [Key]
        public int DestinationId { get; set; }

        // Navigation Properties
    }
}
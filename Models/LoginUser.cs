using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EgyptTourism.Models
{
    [NotMapped]
    public class LoginUser
    {
        // Properties
        [Required(ErrorMessage = "is required")]
        [EmailAddress]
        [Display(Name = "E-Mail Address")]
        public string LoginEmail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]

        public string LoginPassword { get; set; }
    }
}
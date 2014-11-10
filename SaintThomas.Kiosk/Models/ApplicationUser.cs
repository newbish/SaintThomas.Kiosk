using System.ComponentModel.DataAnnotations;
using RavenDB.AspNet.Identity;

namespace SaintThomas.Kiosk.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "*")]
        public string Email { get; set; }
    }

}
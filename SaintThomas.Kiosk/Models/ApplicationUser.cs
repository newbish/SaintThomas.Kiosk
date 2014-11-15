using System.ComponentModel.DataAnnotations;
using RavenDB.AspNet.Identity;

namespace SaintThomas.Kiosk.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "*"), Display(Name="Email")]
        public string Email { get; set; }
        [Display(Name="First Name")]
        public string FirstName { get; set; }
        [Display(Name="Last Name")]
        public string LastName { get; set; }
        [Display(Name="Name")]
        public string Name
        {
            get
            {
                if (!string.IsNullOrEmpty(LastName) && !string.IsNullOrEmpty(FirstName))
                    return string.Format("{0}, {1}", LastName, FirstName);
                else if (!string.IsNullOrEmpty(LastName))
                    return LastName;
                else if (!string.IsNullOrEmpty(FirstName))
                    return FirstName;
                return string.Empty;
            }
        }
    }

}
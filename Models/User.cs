using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Asp_MVC.Models
{
    public class User : IdentityUser<long>
    {
        // IdentityUser  dostarcza Id, Email, PhoneNumber i Password

        public virtual ICollection<Advertisement> Advertisements { get; set; } = new List<Advertisement>();

        [Display(Name = "Imię i nazwisko")]
        public string Name { get; set; } = string.Empty;

    }
}

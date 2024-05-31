using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Bulky.Models
{
    public class ApplicationUser:IdentityUser
    {
        [Required]
        public string Name{ get; set; }
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }


    }
}

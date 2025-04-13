using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace AgroPlaner.DAL.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Navigation properties - relationships to other entities
        public virtual ICollection<Location> Locations { get; set; } = new List<Location>();

        // User's crops - one-to-many relationship
        public virtual ICollection<Crop> Crops { get; set; } = new List<Crop>();
    }
}

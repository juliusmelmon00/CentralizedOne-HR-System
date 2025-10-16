using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CentralizedOne.Data.Models
{
    public class Role
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        // Navigation: one role can have many users
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}

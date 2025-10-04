using System.ComponentModel.DataAnnotations;

namespace SafeVault.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public string Role { get; set; } // "admin" or "user"
    }
}
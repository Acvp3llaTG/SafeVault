using System.ComponentModel.DataAnnotations;

namespace SafeVault.Models
{
    public class SensitiveData
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 3)]
        public string Username { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 6)]
        public string Password { get; set; }
    }
}
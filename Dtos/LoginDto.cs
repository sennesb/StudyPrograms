using System.ComponentModel.DataAnnotations;

namespace FakeXiecheng.API.Dtos
{
    public class LoginDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public string? Username { get; set; }
    }
}

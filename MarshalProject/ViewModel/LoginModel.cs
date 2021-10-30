using System.ComponentModel.DataAnnotations;

namespace MarshalProject.ViewModel
{
    public class LoginModel
    {
        [StringLength(256), Required]
        [EmailAddress]
        public string Email { get; set; }
        [StringLength(256), Required]
        public string Password { get; set; }
        [Required]
        public bool RememberMe { get; set; }
    }
}

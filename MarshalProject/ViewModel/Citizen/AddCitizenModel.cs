using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MarshalProject.ViewModel.Citizen
{
    public class AddCitizenModel
    {
        [StringLength(256), Required]
        [EmailAddress]
        public string Email { get; set; }
        [StringLength(256), Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public bool EmailConfirmed { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }

    }
    public class EditCitizenModel
    {
        [Required]
        public string Id { get; set; }
        [StringLength(256), Required]
        [EmailAddress]
        public string Email { get; set; }
        [StringLength(256), Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public bool EmailConfirmed { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }
    }
}

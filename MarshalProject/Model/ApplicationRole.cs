using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace MarshalProject.Model
{
    public class ApplicationRole: IdentityRole
    {
        public ICollection<ApplicationUserRoles> UserRoles { get; set; }
    }
}

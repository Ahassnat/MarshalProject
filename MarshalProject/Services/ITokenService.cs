using MarshalProject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarshalProject.Services
{
   public interface ITokenService
    {
        Task<string> CreateToken(ApplicationUser user);
    }
}

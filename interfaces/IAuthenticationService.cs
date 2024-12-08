using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PWManager
{
    public interface IAuthenticationService
    {
        bool Authenticate(string masterPassword);
        bool IsAuthenticated { get; }
    }
}
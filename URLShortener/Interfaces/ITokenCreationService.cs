using Microsoft.AspNetCore.Identity;
using URLShortener.Models;

namespace URLShortener.Interfaces
{
    public interface ITokenCreationService
    {
        AuthenticationResponse CreateToken(IdentityUser user);
    }
}

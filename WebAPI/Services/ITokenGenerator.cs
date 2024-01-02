using Microsoft.AspNetCore.Identity;
using WebAPI.Models.DTO;

namespace WebAPI.Services
{
    public interface ITokenGenerator
    {
        Token CreateJWTToken(IdentityUser user, List<string> roles);
    }
}
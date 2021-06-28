using Identity.Core.Entities;
using System.Threading.Tasks;

namespace Identity.WebApi.Interfaces
{
    public interface IAuthHelper
    {
        Task<string> GenerateJwtToken(User user);
        void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
        bool VerifyUserPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
    }
}

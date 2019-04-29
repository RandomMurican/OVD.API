using System.Threading.Tasks;
using OVD.API.Dtos;
using OVD.API.Models;

namespace OVD.API.Data
{
    public interface IAuthRepository
    {
         Task<User> Register(UserForLoginDto user);
         Task<User> Login(UserForLoginDto user);
         Task<bool> UserExists(string username);
         Task<bool> UserIsAdmin(string username);
    }
}
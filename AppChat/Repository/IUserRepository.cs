using AppChat.Models;
using AppChat.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppChat.Repositories
{
    public interface IUserRepository
    {
        Task<User> CreateUser(User user);
        Task<User> GetUserById(int id);
        Task<List<User>> GetAllUsersAsync();
        Task<bool> UpdateUser(User user);
        Task<bool> DeleteUser(int id);
        
        // For Profile Mini (without password)
        Task<UserProfileDTO> GetUserProfileAsync(int id);
        
        // For Searching Users
        Task<List<UserProfileDTO>> SearchUsersAsync(string searchTerm);
    }
}

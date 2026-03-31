using AppChat.Data;
using AppChat.Models;
using AppChat.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppChat.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context) => _context = context;

        public async Task<User> CreateUser(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> GetUserById(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<bool> UpdateUser(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        // Get User Profile (without password) - for mini profile in chat
        public async Task<UserProfileDTO> GetUserProfileAsync(int id)
        {
            var user = await _context.Users
                .Where(u => u.Id == id)
                .Select(u => new UserProfileDTO
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    PhoneNumber = u.PhoneNumber,
                    AvatarUrl = u.AvatarUrl,
                    IsOnline = u.IsOnline,
                    LastSeen = u.LastSeen
                })
                .FirstOrDefaultAsync();

            return user;
        }

        // Search Users by name or phone
        public async Task<List<UserProfileDTO>> SearchUsersAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<UserProfileDTO>();

            var term = searchTerm.ToLower();
            var users = await _context.Users
                .Where(u => u.FirstName.ToLower().Contains(term)
                         || u.LastName.ToLower().Contains(term)
                         || u.PhoneNumber.Contains(term))
                .Select(u => new UserProfileDTO
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    PhoneNumber = u.PhoneNumber,
                    AvatarUrl = u.AvatarUrl,
                    IsOnline = u.IsOnline,
                    LastSeen = u.LastSeen
                })
                .OrderBy(u => u.IsOnline == false)  // Online users first
                .ThenBy(u => u.FirstName)
                .ToListAsync();

            return users;
        }
    }
}

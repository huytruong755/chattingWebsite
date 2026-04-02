using AppChat.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppChat.Repositories
{
    public interface IBlockedUserRepository
    {
        Task<BlockedUserDto> BlockUserAsync(int blockerId, int blockedUserId);

        Task<bool> UnblockUserAsync(int blockerId, int blockedUserId);

        Task<List<BlockedUserDto>> GetBlockedUsersByIdAsync(int userId);

        Task<bool> IsUserBlockedAsync(int blockerId, int blockedUserId);

        Task<bool> IsBlockedBothWaysAsync(int userId1, int userId2);
    }
}

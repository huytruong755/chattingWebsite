using AppChat.Models.DTOs;
using AppChat.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppChat.Services
{
    public class BlockingService
    {
        private readonly IBlockedUserRepository _repo;

        public BlockingService(IBlockedUserRepository repo) => _repo = repo;

        public async Task<BlockedUserDto> BlockUserAsync(int blockerId, int blockedUserId)
        {
            return await _repo.BlockUserAsync(blockerId, blockedUserId);
        }

        public async Task<bool> UnblockUserAsync(int blockerId, int blockedUserId)
        {
            return await _repo.UnblockUserAsync(blockerId, blockedUserId);
        }

        public async Task<List<BlockedUserDto>> GetBlockedUsersByIdAsync(int userId)
        {
            return await _repo.GetBlockedUsersByIdAsync(userId);
        }

        public async Task<bool> IsUserBlockedAsync(int blockerId, int blockedUserId)
        {
            return await _repo.IsUserBlockedAsync(blockerId, blockedUserId);
        }

        public async Task<bool> IsBlockedBothWaysAsync(int userId1, int userId2)
        {
            return await _repo.IsBlockedBothWaysAsync(userId1, userId2);
        }
    }
}

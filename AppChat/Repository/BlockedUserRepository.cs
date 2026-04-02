using AppChat.Data;
using AppChat.Models;
using AppChat.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppChat.Repositories
{
    public class BlockedUserRepository : IBlockedUserRepository
    {
        private readonly AppDbContext _context;

        public BlockedUserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<BlockedUserDto> BlockUserAsync(int blockerId, int blockedUserId)
        {
            if (blockerId == blockedUserId)
                throw new Exception("Cannot block yourself");

            var existingBlock = await _context.BlockedUsers
                .FirstOrDefaultAsync(bu => bu.BlockerId == blockerId && bu.BlockedUserId == blockedUserId);

            if (existingBlock != null)
                return await MapToDtoAsync(existingBlock);

            var blockedUser = new BlockedUser
            {
                BlockerId = blockerId,
                BlockedUserId = blockedUserId,
                BlockedAt = DateTime.UtcNow
            };

            _context.BlockedUsers.Add(blockedUser);
            await _context.SaveChangesAsync();

            return await MapToDtoAsync(blockedUser);
        }

        public async Task<bool> UnblockUserAsync(int blockerId, int blockedUserId)
        {
            var blockedUser = await _context.BlockedUsers
                .FirstOrDefaultAsync(bu => bu.BlockerId == blockerId && bu.BlockedUserId == blockedUserId);

            if (blockedUser == null)
                return false;

            _context.BlockedUsers.Remove(blockedUser);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<BlockedUserDto>> GetBlockedUsersByIdAsync(int userId)
        {
            var blockedUsers = await _context.BlockedUsers
                .Where(bu => bu.BlockerId == userId)
                .ToListAsync();

            var dtos = new List<BlockedUserDto>();
            foreach (var bu in blockedUsers)
            {
                dtos.Add(await MapToDtoAsync(bu));
            }

            return dtos;
        }

        public async Task<bool> IsUserBlockedAsync(int blockerId, int blockedUserId)
        {
            return await _context.BlockedUsers
                .AnyAsync(bu => bu.BlockerId == blockerId && bu.BlockedUserId == blockedUserId);
        }

        public async Task<bool> IsBlockedBothWaysAsync(int userId1, int userId2)
        {
            var isBlocked1 = await IsUserBlockedAsync(userId1, userId2);
            var isBlocked2 = await IsUserBlockedAsync(userId2, userId1);

            return isBlocked1 || isBlocked2;
        }

        private async Task<BlockedUserDto> MapToDtoAsync(BlockedUser blockedUser)
        {
            var user = await _context.Users.FindAsync(blockedUser.BlockedUserId);

            return new BlockedUserDto
            {
                Id = blockedUser.Id,
                BlockedUserId = blockedUser.BlockedUserId,
                PhoneNumber = user?.PhoneNumber ?? "",
                FirstName = user?.FirstName ?? "",
                LastName = user?.LastName ?? "",
                AvatarUrl = user?.AvatarUrl ?? "",
                BlockedAt = blockedUser.BlockedAt
            };
        }
    }
}

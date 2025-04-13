using ListLife.Data;
using ListLife.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ListLife.Services
{
    public class ListSharingService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<UserList> _userManager;

        public ListSharingService(ApplicationDbContext context, UserManager<UserList> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //Method to check if a list can be shared with a user
        public (bool success, string message) CanShareList(
            IdentityUser userToShareWith,
            List<SharedList> existingShares,
            int shoppingListId)
        {
            if (userToShareWith == null)
                return (false, "User not found");

            bool alreadyShared = existingShares
                .Any(sl => sl.ShoppingListId == shoppingListId && sl.SharedWithUserId == userToShareWith.Id);

            if (alreadyShared)
                return (false, "List is already shared with this user");

            return (true, "OK");
        }

        //Method to share a list with a user
        public async Task<(bool success, string message)> ShareListAsync(int shoppingListId, IdentityUser userToShareWith)
        {
            // Ensure the user to share with exists
            if (userToShareWith == null)
                return (false, "User not found");

            var existingUser = await _context.Users
        .FirstOrDefaultAsync(u => u.Id == userToShareWith.Id);

            if (existingUser == null)
                return (false, "User not found");

            // Check if the list is already shared
            var existingShares = await _context.SharedLists
                .Where(sl => sl.ShoppingListId == shoppingListId)
                .ToListAsync();

            var result = CanShareList(userToShareWith, existingShares, shoppingListId);
            if (!result.success)
                return result;

            // Create the new shared list entry
            var sharedList = new SharedList
            {
                ShoppingListId = shoppingListId,
                SharedWithUserId = userToShareWith.Id
            };

            await _context.SharedLists.AddAsync(sharedList);
            await _context.SaveChangesAsync();

            return (true, "List successfully shared");
        }
    }
}

using E2ETesting.UnitTests.Helpers;
using ListLife.Models;
using ListLife.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E2ETesting.UnitTests
{
    public class ShareList
    {
        [Fact]
        public async Task ShareList_Returns_Error_When_User_Does_Not_Exist()
        {
            using var context = InMemoryDbHelper.GetContext();

            var service = new ListSharingService(context, null); 

            var nonExistentUser = new IdentityUser { Email = "nope@example.com" };

            var result = await service.ShareListAsync(1, nonExistentUser);

            Assert.False(result.success);
            Assert.Equal("User not found", result.message);
        }


        [Fact]
        public void CanShareList_Returns_False_When_List_Already_Shared()
        {
            var user = new IdentityUser { Id = "user123" };
            var existingShares = new List<SharedList>
        {
            new SharedList { ShoppingListId = 1, SharedWithUserId = "user123" }
        };
            var service = new ListSharingService(null, null);

            var result = service.CanShareList(user, existingShares, 1);

            Assert.False(result.success);
            Assert.Equal("List is already shared with this user", result.message);
        }

        [Fact]
        public void CanShareList_Returns_True_When_Not_Shared_With_User_Yet()
        {
            // Arrange
            var user = new IdentityUser { Id = "user123" };
            var existingShares = new List<SharedList>(); // Empty list
            var service = new ListSharingService(null, null);

            // Act
            var result = service.CanShareList(user, existingShares, 1);

            // Assert
            Assert.True(result.success);
            Assert.Equal("OK", result.message);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ListLife.Data;
using ListLife.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;

namespace ListLife.Pages
{
    [Authorize]
    public class MyPageModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<UserList> _userManager;

        public MyPageModel(ApplicationDbContext context, UserManager<UserList> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //Properties for the sharing list functionality
        [BindProperty]
        public string UserEmail { get; set; }
        public string Message { get; set; }

        //Hold the user's lists
        public IList<UserList> UserList { get; set; }

        // List to hold shopping lists for the logged-in user
        public IList<ShoppingList> ShoppingLists { get; set; }
        public IList<ShoppingList> SharedShoppingLists { get; set; } = new List<ShoppingList>();

        public IList<Product> Products { get; set; }

        [BindProperty]
        public ShoppingList EditList { get; set; }

        [BindProperty]
        public Product AddNewProduct { get; set; }/* = new ShoppingList();*/

        // Hämtar Shoppinglistorna
        public async Task OnGetAsync()
        {
            // Get user
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {

                //Get user's shopping lists
                ShoppingLists = await _context.ShoppingLists.Where(u => u.UserId == user.Id).ToListAsync();

                // Get user's shopping lists and include related products
                //ShoppingLists = await _context.ShoppingLists
                //    .Where(u => u.UserId == user.Id)
                //    .Include(sl => sl.Products)  // Inkludera produkter
                //    .ToListAsync();

                //Get lists that are shared with the user
                SharedShoppingLists = await _context.SharedLists
                    .Where(sl => sl.SharedWithUserId == user.Id)
                    .Include(sl => sl.ShoppingList)
                    .Select(sl => sl.ShoppingList)
                    .ToListAsync();
            }
        }

        // Hämta ShoppingId baserat på Id samt tillhörande produkter
        public async Task<IActionResult> OnGetEditAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Get user
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                //Get user's shopping lists
                ShoppingLists = await _context.ShoppingLists.Where(u => u.UserId == user.Id).ToListAsync();
            }

                if (EditList != null)
            {
                // Get user's shopping lists and include related products
                ShoppingLists = await _context.ShoppingLists
                    .Where(u => u.UserId == user.Id)
                    .Include(sl => sl.Products)  // Inkludera produkter
                    .ToListAsync();
            }

            if (EditList == null)
            {
                return NotFound();
            }

            return Page();
        }


        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            // Get currently logged-in user ID
            var userId = _userManager.GetUserId(User);
            var newList = new UserList
            {
                ListName = Request.Form["ListName"],
                Id = userId
            };

            // Add list to database
            _context.Users.Add(newList);
            await _context.SaveChangesAsync();


            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostShareAsync(int listId)
        {
            if (string.IsNullOrWhiteSpace(UserEmail))
            {
                Message = "Enter a valid email address";
                return RedirectToPage();
            }
            var userToShareWith = await _userManager.FindByEmailAsync(UserEmail);
            if (userToShareWith == null)
            {
                //Message for alert if user not found
                TempData["Message"] = "User not found";
                TempData["MessageType"] = "error";
                return RedirectToPage();
            }
            //Control if the list is already shared with the user
            bool alreadyShared = await _context.SharedLists
                .AnyAsync(sl => sl.ShoppingListId == listId && sl.SharedWithUserId == userToShareWith.Id);

            if (alreadyShared)
            {
                //Message for alert if list is already shared
                TempData["Message"] = "List is already shared with this user";
                TempData["MessageType"] = "error";
                return RedirectToPage();
            }

            var sharedList = new SharedList
            {
                SharedWithUserId = userToShareWith.Id,
                ShoppingListId = listId
            };

            _context.SharedLists.Add(sharedList);
            await _context.SaveChangesAsync();

            //Message for alert if list is shared successfully
            TempData["Message"] = "List shared successfully!";
            TempData["MessageType"] = "success";
            return RedirectToPage();
        }


        public async Task<IActionResult> OnPostDeleteAsync(int listId)
        {
            // Hämta shoppinglistan från databasen baserat på listId
            var deleteList = await _context.ShoppingLists.FindAsync(listId);

            if (deleteList != null)
            {
                _context.ShoppingLists.Remove(deleteList);
                await _context.SaveChangesAsync(); 
            }

            return RedirectToPage(); 
        }

        public async Task<IActionResult> OnPostEditAsync(int listId)
        {
            // Hämtar listan som ska redigeras
            var editList = await _context.ShoppingLists.FindAsync(listId);

            if (editList == null)
            {
                return NotFound(); 
            }

            EditList = editList;

            return Page();
        }

        // POST för att lägga till ny produkt i listan och spara till databasen
        public async Task<IActionResult> OnPostAddProductAsync(int shoppingListId)
        {
            // Hämta shoppinglistan från databasen
            var shoppingList = await _context.ShoppingLists
                .Include(sl => sl.Products) // Se till att produkterna är inkluderade
                .FirstOrDefaultAsync(sl => sl.Id == shoppingListId);

            if (shoppingList == null)
            {
                return NotFound();
            }

            // Skapa ny produkt och lägg till den i listan
            var newProduct = new Product
            {
                Name = AddNewProduct.Name,
                Amount = AddNewProduct.Amount,
                Category = AddNewProduct.Category,
                ShoppingListId = shoppingListId
            };

            _context.Products.Add(newProduct);
            await _context.SaveChangesAsync();


            EditList = shoppingList; 

            return Page(); 
        }

    }
}
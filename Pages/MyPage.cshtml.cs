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
        public Product AddNewProduct { get; set; }

        // Hämtar Shoppinglistorna
        public async Task OnGetAsync()
        {
            // Get user
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {

                //Get user's shopping lists
                ShoppingLists = await _context.ShoppingLists.Where(u => u.UserId == user.Id).ToListAsync();

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
            else
            {
                Message = "No products added yet."; // Funkar inte
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
            // hämtar listan
            var editList = await _context.ShoppingLists
                .Include(sl => sl.Products)  // Inkludera produkterna
                .FirstOrDefaultAsync(sl => sl.Id == listId);

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
                ShoppingListId = shoppingListId // FK kopplas här
            };

            _context.Products.Add(newProduct);
            await _context.SaveChangesAsync();

            EditList = shoppingList;

            return Page();
        }

        public async Task<IActionResult> OnPostEditProductAsync(int productId)
        {
            // hämta produkten baserat på produktens ID
            var product = await _context.Products
                .Include(p => p.ShoppingList)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product != null)
            {
                // Hämta den shoppinglistan produkten tillhör
                var shoppingList = product.ShoppingList;

                _context.Products.Update(product);
                await _context.SaveChangesAsync();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteProductAsync(int productId)
        {
            // Hämta produkten baserat på produktens ID
            var product = await _context.Products
                .Include(p => p.ShoppingList) // Se till att få med shoppinglistan
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product != null)
            {
                // Hämta den shoppinglistan produkten tillhör
                var shoppingListId = product.ShoppingList.Id;

                // Ta bort produkten
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                //// Hämta den uppdaterade shoppinglistan och dess produkter
                EditList = await _context.ShoppingLists
                    .Include(s => s.Products)
                    .FirstOrDefaultAsync(s => s.Id == shoppingListId);
            }

            // Återgå till samma sida och visa uppdaterad lista
            return Page();
        }

    }
}
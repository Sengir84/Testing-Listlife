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

        // La till ShoppingList / Nän 
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

                //Get lists that are shared with the user
                SharedShoppingLists = await _context.SharedLists
                    .Where(sl => sl.SharedWithUserId == user.Id)
                    .Include(sl => sl.ShoppingList)
                    .Select(sl => sl.ShoppingList)
                    .ToListAsync();
            }
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

        public async Task<IActionResult> OnPostSaveChangesAsync(int shoppingListId)
        {
            var shoppingList = await _context.ShoppingLists
                .Include(sl => sl.Products)  // Inkludera produkterna relaterade till shoppinglistan
                .FirstOrDefaultAsync(sl => sl.Id == shoppingListId && sl.UserId == _userManager.GetUserId(User));

            if (shoppingList == null)
            {
                return NotFound();  
            }

            shoppingList.Title = EditList.Title;

            // Uppdatera produkterna
            foreach (var product in shoppingList.Products)
            {
                product.Category ??= "Other";
            }

            _context.ShoppingLists.Update(shoppingList);
            await _context.SaveChangesAsync();

            return Page();
        }


        // POST för att lägga till ny produkt
        public async Task<IActionResult> OnPostAddProductAsync(int shoppingListId)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var shoppingList = await _context.ShoppingLists
                .FirstOrDefaultAsync(sl => sl.Id == shoppingListId);

            if (shoppingList == null)
            {
                return NotFound();
            }

            // Skapa en ny produkt kopplad till shoppinglistan
            var newProduct = new Product
            {
                Name = AddNewProduct.Name,
                Amount = AddNewProduct.Amount,
                Category = AddNewProduct.Category ?? "Other",
                ShoppingListId = shoppingListId
            };

            _context.Products.Add(newProduct);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { shoppingListId = shoppingListId }); // Ladda om sidan med aktuell lista
        }

    }
}
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

        //Hold the user's lists
        public IList<UserList> UserList { get; set; }

        // List to hold shopping lists for the logged-in user
        public IList<ShoppingList> ShoppingLists { get; set; }

        [BindProperty]
        public ShoppingList EditList { get; set; }

        [BindProperty]
        public ShoppingList AddNewProduct { get; set; } = new ShoppingList();


        ////Lista för att hålla produkterna
        //public IList<ShoppingList> ProductsInList { get; set; }

        // Hämtar Shoppinglistorna
        public async Task OnGetAsync()
        {
            //Get user
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                
                //Get user's shopping lists
                ShoppingLists = await _context.ShoppingLists.Where(u => u.UserId == user.Id).ToListAsync();
            }

        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            //Get currently logged in user id
            var userId = _userManager.GetUserId(User);
            var newList = new UserList
            {
                ListName = Request.Form["ListName"],
                Id = userId
            };

            //Add list to database
            _context.Users.Add(newList);
            await _context.SaveChangesAsync();

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

        public async Task<IActionResult> OnPostSaveChangesAsync(int listId)
        {
            // Hämta shoppinglistan
            var shoppingList = await _context.ShoppingLists
                .FirstOrDefaultAsync(x => x.Id == listId);

            if (shoppingList == null)
            {
                return NotFound("TEST");
            }

            // Uppdatera shoppinglistan med de nya värdena
            shoppingList.Title = EditList.Title;
            shoppingList.Category = EditList.Category;

            // Spara ändringar till databasen
            _context.ShoppingLists.Update(shoppingList);
            await _context.SaveChangesAsync();

            return RedirectToPage(); 
        }


        // POST för att lägga till ny produkt
        public async Task<IActionResult> OnPostAddProductAsync(int listId)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Hämta inloggad användare
            var user = await _userManager.GetUserAsync(User);

            // Hämta shoppinglistan från databasen
            var shoppingList = await _context.ShoppingLists
                .FirstOrDefaultAsync(x => x.Id == listId && x.UserId == user.Id);

            if (shoppingList == null)
            {
                return NotFound();
            }

            // Hantera att lägga till den nya produkten i listan
            if (!string.IsNullOrEmpty(shoppingList.Product))
            {
                // Om det finns redan produkter, lägg till den nya produkten med komma-separation
                shoppingList.Product += ", " + AddNewProduct.Product;
            }
            else
            {
                // Om inga produkter finns, sätt den första produkten
                shoppingList.Product = AddNewProduct.Product;
            }

            // Lägg till mängden och uppdatera kategori (om det är nödvändigt)
            shoppingList.Amount += AddNewProduct.Amount;
            shoppingList.Category = AddNewProduct.Category;

            // Uppdatera shoppinglistan i databasen
            _context.ShoppingLists.Update(shoppingList);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}

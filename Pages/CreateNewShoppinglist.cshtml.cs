using ListLife.Data;
using ListLife.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ListLife.Pages
{

    public class CreateNewShoppingList : PageModel
    {
        // instans av databasen för att lagra listor
        private readonly ApplicationDbContext Dbcontext;

        // Hanterar användare via ASP.NET Identity (UserManager) för att hämta och hantera inloggade användare
        private readonly UserManager<UserList> userManager;

        // Konstruktor
        public CreateNewShoppingList(ApplicationDbContext context, UserManager<UserList> userManager)
        {
            this.Dbcontext = context;
            this.userManager = userManager;
        }

        [BindProperty]
        // Property för att hålla användarens shoppinglistor
        public ShoppingList ShoppingList { get; set; }

        public string UserListName { get; set; }

        [BindProperty]  
        public List<Product> Products { get; set; } = new List<Product>();

        public List<ShoppingList> ShoppingLists { get; set; } = new List<ShoppingList>();

        public List<Product> RecentlyPurchasedProducts { get; set; } = new List<Product>();



        public async Task OnGetAsync()
        {
            var user = await userManager.GetUserAsync(User); // Hämta den inloggade användaren
            if (user != null)
            {
                UserListName = user.ListName;

                ShoppingLists = await Dbcontext.ShoppingLists
                    .Include(s => s.Products)
                    .Where(s => s.UserId == user.Id)
                    .ToListAsync();

                // Hämta nyligen köpta produkter (senaste 10 produkterna från tidigare shoppinglistor)
                RecentlyPurchasedProducts = await Dbcontext.Products
                    .Where(p => p.ShoppingList.UserId == user.Id)
                    .OrderByDescending(p => p.Id) // Senaste först
                    .Take(10)
                    .ToListAsync();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user != null)
            {
                var newShoppingList = new ShoppingList
                {
                    UserId = user.Id,
                    UserList = user,
                    Title = ShoppingList.Title ?? "New Shopping List",
                    Products = new List<Product>()
                };

                Dbcontext.ShoppingLists.Add(newShoppingList);
                await Dbcontext.SaveChangesAsync(); // Sparar shoppinglistan och får ID

                // Lägg till produkter kopplade till shoppinglistan
                foreach (var product in Products)
                {
                    product.ShoppingListId = newShoppingList.Id; // Koppla produkten till rätt lista
                    Dbcontext.Products.Add(product);
                }

                await Dbcontext.SaveChangesAsync(); // Spara produkterna i databasen

                await Dbcontext.SaveChangesAsync(); // Save products in DB

                // Clear product input fields
                ModelState.Remove("Products");
                Products = new List<Product>();

                
            }

            return RedirectToPage("/MyPage");
        }
    }
}
    

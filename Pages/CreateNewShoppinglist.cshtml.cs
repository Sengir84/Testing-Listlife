using ListLife.Data;
using ListLife.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ListLife.Pages
{
    public class CreateNewShoppingList : PageModel
    {
        // instans av databasen för att lagra listor
        private readonly ApplicationDbContext context;

        // Hanterar användare via ASP.NET Identity (UserManager) för att hämta och hantera inloggade användare
        private readonly UserManager<IdentityUser> userManager;


        // Property för att hålla användarens shoppinglistor
        public IList<ShoppingList> ShoppingLists { get; set; }

        // Konstruktor
        public CreateNewShoppingList(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Hämta den inloggade användaren asynkront
            var user = await userManager.GetUserAsync(User); // User är en inbyggd property i PageModel som innehåller information om den inloggade användaren

            if (user != null)
            {
                // Hämta alla shoppinglistor för den inloggade användaren
                ShoppingLists = context.ShoppingLists
                    .Where(shoppingList => shoppingList.UserId == user.Id)  // Filtrera baserat på användarens ID
                    .ToList();

                context.Add(ShoppingLists);
                await context.SaveChangesAsync();                
            }
            return RedirectToPage("/MyPage");
        }
    }
}

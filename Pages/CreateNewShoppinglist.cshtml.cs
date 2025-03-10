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
        public Product Product { get; set; }


        public async Task OnGetAsync()
        {
            var user = await userManager.GetUserAsync(User); // Hämta den inloggade användaren
            if (user != null)
            {
                UserListName = user.ListName;
            }
        }

        public async Task<IActionResult> OnPostAsync() 
        {
            var user = await userManager.GetUserAsync(User);

            if (user != null)
            {
                // Koppla shoppinglistan till användaren och sätt användarens ID
                ShoppingList.UserId = user.Id;
                ShoppingList.UserList = user;

                Product.Category ??= "Other";  // Om kategorin �r null, s�tt den till "�vrigt"
                ShoppingList.Title ??= "New List";  // Om titeln �r null, s�tt den till "Ny lista"

                Product.Category ??= "Other";  // Om kategorin är null, sätt den till "Övrigt"


                // Lägg till shoppinglistan i databasen
                Dbcontext.ShoppingLists.Add(ShoppingList);
                await Dbcontext.SaveChangesAsync();  // Spara ändringarna i databasen
            }

            return RedirectToPage("/MyPage");  // Omdirigera till annan sida efter att ha sparat listan
        }
    }
}
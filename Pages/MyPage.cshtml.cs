using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ListLife.Data;
using ListLife.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ListLife.Pages
{
  //  [Authorize]
    public class MyPageModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public MyPageModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //Hold the user's lists
        public IList<UserList> UserList { get; set; }

        public async Task OnGetAsync()
        {
            //Get user
            var user = await _userManager.GetUserAsync(User);


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

    }
}

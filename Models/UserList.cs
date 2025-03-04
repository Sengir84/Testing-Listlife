using Microsoft.AspNetCore.Identity;

namespace ListLife.Models
{
    public class UserList : IdentityUser
    {
        //Name of the list
        public string ListName { get; set; }
        //FK to user
        public int ShoppingId  { get; set; }
        public ShoppingList ShoppingList { get; set; }
    }
}

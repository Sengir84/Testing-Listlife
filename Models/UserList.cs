using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ListLife.Models
{
    /* Representerar själva användaren och deras information (t.ex. användarnamn, e-post etc.) 
     * och innehåller en lista med shoppinglistor som användaren har.*/
    public class UserList : IdentityUser
    {
        ////Name of the list
        public string? ListName { get; set; }

        //FK to user

        public List<ShoppingList> ShoppingLists { get; set; } = new();

    }
}

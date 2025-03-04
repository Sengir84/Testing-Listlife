namespace ListLife.Models
{
    public class ShoppingList
    {
        public int Id { get; set; }
        public string Product { get; set; }
        public string Category { get; set; } 
        public string Amount { get; set; }

        // ConnectionTable
        public string UserListId { get; set; }
        //public UserList UserList { get; set; }

    }
}

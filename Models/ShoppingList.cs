namespace ListLife.Models
{
    public class ShoppingList
    {
        public int Id { get; set; }
        public string Product { get; set; }
        public string Amount { get; set; }

        // ConnectionTable
        public string UserId { get; set; }
        public UserList UserList { get; set; }
        

        // FK till Category
        public int CategoryId { get; set; }
        public Category Category { get; set; }

    }
}

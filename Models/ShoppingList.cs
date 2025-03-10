namespace ListLife.Models
{
    public class ShoppingList
    {
        /* Representerar en enskild shoppinglista. Den är kopplad till användaren via UserId och UserList.*/

        public int Id { get; set; }

        // ConnectionTable
        public string UserId { get; set; }
        public UserList UserList { get; set; }

        // Name of List
        public string? Title { get; set; }

        //Navigation property
        public ICollection<SharedList> SharedWith { get; set; } = new List<SharedList>();

        // Lista av produkter i shoppinglistan
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
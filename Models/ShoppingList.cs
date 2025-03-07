namespace ListLife.Models
{
    public class ShoppingList
    {
        /* Representerar en enskild shoppinglista och innehåller detaljer som produktnamn, mängd och kategori för varje produkt i listan. 
         * Den är kopplad till användaren via UserId och UserList.*/

        public int Id { get; set; }
        public string Product { get; set; }
        public decimal Amount { get; set; }

        // ConnectionTable
        public string UserId { get; set; }
        public UserList UserList { get; set; }

        //Stores category of product
        public string Category { get; set; }

        // Name of List
        public string? Title { get; set; }

        //Navigation property
        public ICollection<SharedList> SharedWith { get; set; } = new List<SharedList>();
    }
}

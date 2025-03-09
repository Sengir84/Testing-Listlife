namespace ListLife.Models
{
    public class Product
    {
        // PK för produkten
        public int Id { get; set; }  
        public string Name { get; set; }  
        public decimal Amount { get; set; } 
        public string Category { get; set; } 

        // FK till ShoppingList
        public int ShoppingListId { get; set; }
        public ShoppingList ShoppingList { get; set; }  
    }
}

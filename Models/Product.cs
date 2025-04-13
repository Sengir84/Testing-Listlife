namespace ListLife.Models
{
    public class Product
    {
        // PK for product
        public int Id { get; set; }  
        public string Name { get; set; }
        
        private int _amount;
        public int Amount
        {
            get => _amount;
            set
            {
                if (value < 1)
                    throw new ArgumentException("Amount cannot be negative");
                _amount = value;
            }
        }
        public string Category { get; set; } 

        // FK to Shopping list
        public int ShoppingListId { get; set; }
        public ShoppingList ShoppingList { get; set; }  
    }
}

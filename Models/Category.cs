using System.ComponentModel.DataAnnotations;

namespace ListLife.Models
{
    // kategori-modell som representerar en kategori i databasen
    public class Category 
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}

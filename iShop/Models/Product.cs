using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iShop.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        [ForeignKey("FK_Category_123")]
        public int CategoryId { get; set; }
        public string Description { get; set; }
        public int Count { get; set; }
        public int PriceIn { get; set; }
        public int PriceOut { get; set; }


    }
}

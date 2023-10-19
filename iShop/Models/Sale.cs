using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace iShop.Models
{
    public class Sale
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("FK_Product_123")]
        public int ProductId { get; set; }
        public int PriceOut { get; set; }
        public int Count { get; set; }

        [ForeignKey("FK_User_123")]
        public string UserId { get; set; }
        public DateTime SaleDate { get; set; }

    }
}

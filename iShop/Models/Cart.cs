using System.ComponentModel.DataAnnotations;

namespace iShop.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string UserId { get; set; }
        public int Count { get; set; }
        public int PriceOut { get; set; }
        public DateTime AddedAt { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace iShop.Models
{
    public class Picture
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("FK_Product_123")]
        public int ProductId { get; set; }
        public string PictureUrl { get; set; }
    }
}

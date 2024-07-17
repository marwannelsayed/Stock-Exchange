using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StockExchangeAPI.Models
{
    public class BoughtStock
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BoughtStockId { get; set; }
        [Required]
        public int StockId { get; set; }
        [Required]
        [MaxLength(20)]
        public string? StockSymbol { get; set; }
        [Required]
        public int Price { get; set; }
        [Required]
        public int Quantity { get; set; }
    }


}
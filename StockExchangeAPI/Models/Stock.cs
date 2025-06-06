using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StockExchangeAPI.Models
{
    public class Stock
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StockId { get; set; }
        [Required]
        [MaxLength(20)]
        public string? StockSymbol { get; set; }
        [Required]
        public int Price { get; set; }
        [Required]
        public DateTime TimeStamp { get; set; }
        [MaxLength(20)]
        public string? Subscriber { get; set; }
    }


}
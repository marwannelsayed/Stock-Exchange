using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StockExchangeAPI.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        [Required]
        [MaxLength(20)]
        public string? Username { get; set; }
        [Required]
        [MaxLength(200)]
        public string? Password { get; set; }
    }
}
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZyroMoney_DESKTOP.Models
{
    [Table("CardTransactions")]
    public class CardTransaction
    {
        [Key]
        public int TransactionId { get; set; }

        [Required]
        public int CreditCardId { get; set; }

        [ForeignKey("CreditCardId")]
        public CreditCard? CreditCard { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; } = DateTime.Now;

        [Required]
        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Category { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(3)]
        public string OriginalCurrency { get; set; } = "DOP";

        [Column(TypeName = "decimal(18,4)")]
        public decimal ExchangeRate { get; set; } = 1.0000m;

        [MaxLength(500)]
        public string? Comment { get; set; }

        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
    }
}
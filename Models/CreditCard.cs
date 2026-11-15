using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZyroMoney_DESKTOP.Models
{
    [Table("CreditCards")]
    public class CreditCard
    {
        [Key]
        public int CreditCardId { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CreditLimit { get; set; }

        [Required]
        [MaxLength(3)]
        public string CurrencyCode { get; set; } = "DOP";

        [Required]
        public int ClosingDay { get; set; }

        [Required]
        public int DueDay { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal InitialBalance { get; set; } = 0.00m;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        public ICollection<CardTransaction> Transactions { get; set; } = new List<CardTransaction>();
        public ICollection<CardPayment> Payments { get; set; } = new List<CardPayment>();
    }
}
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZyroMoney_DESKTOP.Models
{
    [Table("CardPayments")]
    public class CardPayment
    {
        [Key]
        public int PaymentId { get; set; }

        [Required]
        public int CreditCardId { get; set; }

        [ForeignKey("CreditCardId")]
        public CreditCard? CreditCard { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; } = DateTime.Now;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(30)]
        public string PaymentType { get; set; } = "Parcial";

        [MaxLength(500)]
        public string? Notes { get; set; }

        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
    }
}
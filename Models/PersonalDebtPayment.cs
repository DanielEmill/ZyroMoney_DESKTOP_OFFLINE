using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZyroMoney_DESKTOP.Models
{
    [Table("PersonalDebtPayments")]
    public class PersonalDebtPayment
    {
        [Key]
        public int DebtPaymentId { get; set; }

        [Required]
        public int PersonalDebtId { get; set; }

        [ForeignKey("PersonalDebtId")]
        public PersonalDebt? PersonalDebt { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; } = DateTime.Now;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
    }
}
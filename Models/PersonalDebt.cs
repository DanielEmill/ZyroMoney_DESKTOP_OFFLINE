using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZyroMoney_DESKTOP.Models
{
    [Table("PersonalDebts")]
    public class PersonalDebt
    {
        [Key]
        public int PersonalDebtId { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        [Required]
        [MaxLength(100)]
        public string CreditorName { get; set; } = string.Empty;

        [MaxLength(250)]
        public string? Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal InitialAmount { get; set; }

        [Required]
        [MaxLength(3)]
        public string CurrencyCode { get; set; } = "DOP";

        [Required]
        public DateTime DebtDate { get; set; }

        public DateTime? DueDate { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        public ICollection<PersonalDebtPayment> Payments { get; set; } = new List<PersonalDebtPayment>();
    }
}
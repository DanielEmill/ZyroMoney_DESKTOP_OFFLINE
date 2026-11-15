using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZyroMoney_DESKTOP.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = "Usuario Local"; // Nombre del perfil local

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relaciones de navegación local
        public ICollection<CreditCard> CreditCards { get; set; } = new List<CreditCard>();
        public ICollection<PersonalDebt> PersonalDebts { get; set; } = new List<PersonalDebt>();
    }
}
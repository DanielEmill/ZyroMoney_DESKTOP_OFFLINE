using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using ZyroMoney_DESKTOP.Models;

namespace ZyroMoney_DESKTOP.Data
{
    public class ZyroMoneyLocalContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<CreditCard> CreditCards => Set<CreditCard>();
        public DbSet<CardTransaction> Transactions => Set<CardTransaction>();
        public DbSet<CardPayment> Payments => Set<CardPayment>();
        public DbSet<PersonalDebt> PersonalDebts => Set<PersonalDebt>();
        public DbSet<PersonalDebtPayment> PersonalDebtPayments => Set<PersonalDebtPayment>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ZyroMoneyDesktop");
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string dbPath = Path.Combine(folder, "zyromoney_local.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }
    }
}
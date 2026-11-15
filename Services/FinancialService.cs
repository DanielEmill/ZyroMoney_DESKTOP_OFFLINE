using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using ZyroMoney_DESKTOP.Data;
using ZyroMoney_DESKTOP.Models;

namespace ZyroMoney_DESKTOP
{
    public static class FinancialService
    {
        // Obtener resumen de todas las tarjetas activas para el Dashboard
        public static List<CreditCardSummary> GetActiveCardSummaries()
        {
            using (var context = new ZyroMoneyLocalContext())
            {
                var cards = context.CreditCards
                    .Include(c => c.Transactions)
                    .Include(c => c.Payments)
                    .Where(c => c.IsActive && !c.IsDeleted)
                    .ToList();

                var summaries = new List<CreditCardSummary>();

                foreach (var card in cards)
                {
                    decimal totalSpent = card.Transactions.Where(t => !t.IsDeleted).Sum(t => t.Amount);
                    decimal totalPaid = card.Payments.Where(p => !p.IsDeleted).Sum(p => p.Amount);

                    // CORREGIDO: Se agregó el signo de menos (-) aquí
                    decimal currentBalance = card.InitialBalance + totalSpent - totalPaid;

                    summaries.Add(new CreditCardSummary
                    {
                        CreditCardId = card.CreditCardId,
                        Name = card.Name,
                        CurrencyCode = card.CurrencyCode,
                        CreditLimit = card.CreditLimit,
                        CurrentBalance = currentBalance,
                        ClosingDay = card.ClosingDay,
                        DueDay = card.DueDay, // Representa los días de gracia después del corte
                    });
                }

                return summaries;
            }
        }

        // Obtener el nombre del perfil de usuario
        public static string GetUserProfileName()
        {
            using (var context = new ZyroMoneyLocalContext())
            {
                var user = context.Users.FirstOrDefault();
                return user != null ? $"👤 {user.Name}" : "👤 Perfil Local";
            }
        }

        // Borrado lógico de tarjeta
        public static void DeleteCard(int cardId)
        {
            using (var context = new ZyroMoneyLocalContext())
            {
                var card = context.CreditCards.FirstOrDefault(c => c.CreditCardId == cardId);
                if (card != null)
                {
                    card.IsDeleted = true;
                    card.IsActive = false;
                    context.SaveChanges();
                }
            }
        }

        // Registrar consumo
        public static void AddTransaction(int cardId, string description, decimal amount, string category, string comment)
        {
            using (var context = new ZyroMoneyLocalContext())
            {
                var transaction = new CardTransaction
                {
                    CreditCardId = cardId,
                    Description = description,
                    Amount = amount,
                    Category = category,
                    Comment = string.IsNullOrEmpty(comment) ? null : comment,
                    TransactionDate = DateTime.Now
                };

                context.Transactions.Add(transaction);
                context.SaveChanges();
            }
        }

        // Registrar pago / abono
        public static void AddPayment(int cardId, decimal amount, string paymentType, string notes)
        {
            using (var context = new ZyroMoneyLocalContext())
            {
                var payment = new CardPayment
                {
                    CreditCardId = cardId,
                    Amount = amount,
                    PaymentType = paymentType,
                    Notes = string.IsNullOrEmpty(notes) ? null : notes,
                    PaymentDate = DateTime.Now
                };

                context.Payments.Add(payment);
                context.SaveChanges();
            }
        }

        // Calcular el balance exacto al último corte
        public static decimal CalculateStatementBalance(int cardId)
        {
            using (var context = new ZyroMoneyLocalContext())
            {
                var card = context.CreditCards
                    .Include(c => c.Transactions)
                    .Include(c => c.Payments)
                    .FirstOrDefault(c => c.CreditCardId == cardId);

                if (card == null) return 0;

                DateTime today = DateTime.Today;
                int safeDay = Math.Min(card.ClosingDay, DateTime.DaysInMonth(today.Year, today.Month));
                DateTime lastClosingDate = new DateTime(today.Year, today.Month, safeDay);

                if (today < lastClosingDate)
                {
                    DateTime prevMonth = today.AddMonths(-1);
                    int prevSafeDay = Math.Min(card.ClosingDay, DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month));
                    lastClosingDate = new DateTime(prevMonth.Year, prevMonth.Month, prevSafeDay);
                }

                decimal spentUpToClosing = card.Transactions
                    .Where(t => !t.IsDeleted && t.TransactionDate.Date <= lastClosingDate)
                    .Sum(t => t.Amount);

                decimal totalPaid = card.Payments
                    .Where(p => !p.IsDeleted)
                    .Sum(p => p.Amount);

                // CORREGIDO: Se agregó el signo de menos (-) aquí también
                decimal balanceAtClosing = card.InitialBalance + spentUpToClosing - totalPaid;

                return balanceAtClosing < 0 ? 0 : balanceAtClosing;
            }
        }

        // Calcular pago mínimo estimado basado en el corte (5% del balance al corte)
        public static decimal CalculateMinimumPayment(int cardId)
        {
            decimal statement = CalculateStatementBalance(cardId);
            if (statement <= 0) return 0;

            decimal min = Math.Round(statement * 0.05m, 2);
            return min > statement ? statement : min;
        }

        public static void UpdateCard(int cardId, string name, decimal limit, int closingDay, int dueDay)
        {
            using (var context = new ZyroMoneyLocalContext())
            {
                var card = context.CreditCards.FirstOrDefault(c => c.CreditCardId == cardId);
                if (card != null)
                {
                    card.Name = name;
                    card.CreditLimit = limit;
                    card.ClosingDay = closingDay;
                    card.DueDay = dueDay;
                    context.SaveChanges();
                }
            }
        }
        public static void AddTransactionWithDate(int cardId, string description, decimal amount, string category, string comment, DateTime transactionDate)
        {
            using (var context = new ZyroMoneyLocalContext())
            {
                var transaction = new CardTransaction
                {
                    CreditCardId = cardId,
                    Description = description,
                    Amount = amount,
                    Category = category,
                    Comment = string.IsNullOrEmpty(comment) ? null : comment,
                    TransactionDate = transactionDate 
                };

                context.Transactions.Add(transaction);
                context.SaveChanges();
            }
        }
        public static void AddPaymentWithDate(int cardId, decimal amount, string paymentType, string notes, DateTime paymentDate)
        {
            using (var context = new ZyroMoneyLocalContext())
            {
                var payment = new CardPayment
                {
                    CreditCardId = cardId,
                    Amount = amount,
                    PaymentType = paymentType,
                    Notes = string.IsNullOrEmpty(notes) ? null : notes,
                    PaymentDate = paymentDate 
                };

                context.Payments.Add(payment);
                context.SaveChanges();
            }
        }
    }
}
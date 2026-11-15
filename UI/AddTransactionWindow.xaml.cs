using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ZyroMoney_DESKTOP.Data;
using ZyroMoney_DESKTOP.Models;
using Microsoft.EntityFrameworkCore;

namespace ZyroMoney_DESKTOP
{
    public partial class AddTransactionWindow : Window
    {
        public AddTransactionWindow()
        {
            InitializeComponent();
            LoadCards();
        }

        private void LoadCards()
        {
            var cards = FinancialService.GetActiveCardSummaries();
            CmbCards.ItemsSource = cards;
            if (cards.Any()) CmbCards.SelectedIndex = 0;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (CmbCards.SelectedItem == null)
            {
                MessageBox.Show("Selecciona una tarjeta.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtDescription.Text))
            {
                MessageBox.Show("Ingresa una descripción o comercio.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(TxtAmount.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Ingresa un monto válido mayor a 0.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedCard = (CreditCardSummary)CmbCards.SelectedItem;

            // Validación de límite de crédito disponible
            using (var context = new ZyroMoneyLocalContext())
            {
                var card = context.CreditCards
                    .Include(c => c.Transactions.Where(t => !t.IsDeleted))
                    .Include(c => c.Payments.Where(p => !p.IsDeleted))
                    .FirstOrDefault(c => c.CreditCardId == selectedCard.CreditCardId);

                if (card != null)
                {
                    decimal totalSpent = card.Transactions.Sum(t => t.Amount);
                    decimal totalPaid = card.Payments.Sum(t => t.Amount);
                    decimal currentBalance = card.InitialBalance + totalSpent - totalPaid;
                    decimal availableLimit = card.CreditLimit - currentBalance;

                    if (amount > availableLimit)
                    {
                        MessageBox.Show($"El consumo excede el límite disponible en la tarjeta.\nDisponible actual: {card.CurrencyCode} {availableLimit:N2}",
                            "Fondos Insuficientes", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
            }

            string category = ((ComboBoxItem)CmbCategory.SelectedItem)?.Content?.ToString() ?? "General";
            string comment = TxtComment.Text.Trim();

            DateTime chosenDate = DpTransactionDate.SelectedDate ?? DateTime.Now;

            FinancialService.AddTransactionWithDate(selectedCard.CreditCardId, TxtDescription.Text.Trim(), amount, category, comment, chosenDate);

            DialogResult = true;
            Close();
        }
    }
}
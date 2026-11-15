using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ZyroMoney_DESKTOP.Data;
using ZyroMoney_DESKTOP.Models;

namespace ZyroMoney_DESKTOP
{
    public partial class AddPaymentWindow : Window
    {
        public AddPaymentWindow()
        {
            InitializeComponent();
            LoadCards();
        }

        private void LoadCards()
        {
            using (var context = new ZyroMoneyLocalContext())
            {
                var cards = context.CreditCards
                    .Where(c => c.IsActive && !c.IsDeleted)
                    .ToList();

                CmbCards.ItemsSource = cards;
                if (cards.Any()) CmbCards.SelectedIndex = 0;
            }
        }

        private void PaymentSelection_Changed(object sender, SelectionChangedEventArgs e)
        {
            // Método auxiliar existente para tus eventos de selección
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (CmbCards.SelectedItem == null)
            {
                MessageBox.Show("Selecciona una tarjeta.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(TxtAmount.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Ingresa un monto de pago válido mayor a 0.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedCard = (CreditCard)CmbCards.SelectedItem;

            // ==========================================
            // VALIDACIÓN: NO PAGAR MÁS DE LO QUE SE DEBE
            // ==========================================
            using (var context = new ZyroMoneyLocalContext())
            {
                var card = context.CreditCards
                    .Include(c => c.Transactions.Where(t => !t.IsDeleted))
                    .Include(c => c.Payments.Where(p => !p.IsDeleted))
                    .FirstOrDefault(c => c.CreditCardId == selectedCard.CreditCardId);

                if (card != null)
                {
                    decimal totalSpent = card.Transactions.Sum(t => t.Amount);
                    decimal totalPaid = card.Payments.Sum(p => p.Amount);

                    // Deuda actual de la tarjeta
                    decimal currentDebt = card.InitialBalance + totalSpent - totalPaid;

                    if (currentDebt <= 0)
                    {
                        MessageBox.Show("Esta tarjeta no tiene balance pendiente de pago.", "Sin Deuda", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }

                    if (amount > currentDebt)
                    {
                        MessageBox.Show($"El monto a pagar ({amount:N2}) supera la deuda actual de la tarjeta ({currentDebt:N2}). No puedes abonar de más.",
                            "Pago Excedido", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
            }

            string paymentType = ((ComboBoxItem)CmbPaymentType.SelectedItem)?.Content?.ToString() ?? "Parcial";
            string notes = TxtNotes.Text.Trim();

            // Obtener la fecha seleccionada del DatePicker
            DateTime paymentDate = DpPaymentDate.SelectedDate ?? DateTime.Now;

            // Guardar con fecha personalizada
            FinancialService.AddPaymentWithDate(selectedCard.CreditCardId, amount, paymentType, notes, paymentDate);

            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
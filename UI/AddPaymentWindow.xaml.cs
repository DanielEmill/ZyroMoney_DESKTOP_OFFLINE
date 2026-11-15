using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
            var cards = FinancialService.GetActiveCardSummaries();
            CmbCards.ItemsSource = cards;
            if (cards.Any()) CmbCards.SelectedIndex = 0;
        }

        // Este evento se dispara cada vez que cambias de tarjeta o de tipo de pago
        private void PaymentSelection_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (CmbCards == null || CmbPaymentType == null || TxtAmount == null) return;
            if (CmbCards.SelectedItem is not CreditCardSummary card) return;
            if (CmbPaymentType.SelectedItem is not ComboBoxItem typeItem) return;

            string selectedType = typeItem.Content?.ToString() ?? "";

            if (selectedType == "Total / Al Corte")
            {
                // Pide al servicio el balance calculado hasta el último día de corte
                decimal statementBalance = FinancialService.CalculateStatementBalance(card.CreditCardId);
                TxtAmount.Text = statementBalance > 0 ? statementBalance.ToString("N2") : "0.00";
            }
            else if (selectedType == "Mínimo")
            {
                decimal minPayment = FinancialService.CalculateMinimumPayment(card.CreditCardId);
                TxtAmount.Text = minPayment > 0 ? minPayment.ToString("N2") : "0.00";
            }
            else if (selectedType == "Parcial")
            {
                TxtAmount.Text = ""; 
            }
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

            if (!decimal.TryParse(TxtAmount.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Ingresa un monto válido mayor a 0.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedCard = (CreditCardSummary)CmbCards.SelectedItem;
            string paymentType = ((ComboBoxItem)CmbPaymentType.SelectedItem).Content.ToString() ?? "Parcial";
            string notes = TxtNotes.Text.Trim();

            FinancialService.AddPayment(selectedCard.CreditCardId, amount, paymentType, notes);

            DialogResult = true;
            Close();
        }
    }
}
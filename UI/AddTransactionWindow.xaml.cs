using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ZyroMoney_DESKTOP.Models;

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
            string category = ((ComboBoxItem)CmbCategory.SelectedItem)?.Content?.ToString() ?? "General";
            string comment = TxtComment.Text.Trim();

            FinancialService.AddTransaction(selectedCard.CreditCardId, TxtDescription.Text.Trim(), amount, category, comment);

            DialogResult = true;
            Close();
        }
    }
}
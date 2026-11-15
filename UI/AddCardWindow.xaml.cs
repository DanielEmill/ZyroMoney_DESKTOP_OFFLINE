using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ZyroMoney_DESKTOP.Data;
using ZyroMoney_DESKTOP.Models;

namespace ZyroMoney_DESKTOP
{
    public partial class AddCardWindow : Window
    {
        public AddCardWindow()
        {
            InitializeComponent();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtName.Text))
            {
                MessageBox.Show("Escribe un nombre para la tarjeta.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(TxtLimit.Text, out decimal limit) || limit <= 0)
            {
                MessageBox.Show("Ingresa un límite de crédito válido mayor a 0.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            decimal.TryParse(TxtInitialBalance.Text, out decimal initialBalance);

            if (!int.TryParse(TxtClosingDay.Text, out int closingDay) || closingDay < 1 || closingDay > 31)
            {
                MessageBox.Show("El día de corte debe estar entre 1 y 31.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(TxtDueDay.Text, out int dueDay) || dueDay < 1 || dueDay > 31)
            {
                MessageBox.Show("El día límite de pago debe estar entre 1 y 31.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string currency = ((ComboBoxItem)CmbCurrency.SelectedItem).Content.ToString() ?? "DOP";

            using (var context = new ZyroMoneyLocalContext())
            {
                var user = context.Users.First();

                var newCard = new CreditCard
                {
                    UserId = user.UserId,
                    Name = TxtName.Text.Trim(),
                    CurrencyCode = currency,
                    CreditLimit = limit,
                    InitialBalance = initialBalance,
                    ClosingDay = closingDay,
                    DueDay = dueDay,
                    IsActive = true
                };

                context.CreditCards.Add(newCard);
                context.SaveChanges();
            }

            DialogResult = true;
            Close();
        }
    }
}
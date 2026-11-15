using System;
using System.Linq;
using System.Windows;
using ZyroMoney_DESKTOP.Data;
using ZyroMoney_DESKTOP.Models;

namespace ZyroMoney_DESKTOP
{
    public partial class EditCardWindow : Window
    {
        private readonly int _cardId;

        public EditCardWindow(int cardId)
        {
            InitializeComponent();
            _cardId = cardId;
            LoadCardData();
        }

        private void LoadCardData()
        {
            using (var context = new ZyroMoneyLocalContext())
            {
                var card = context.CreditCards.FirstOrDefault(c => c.CreditCardId == _cardId);
                if (card != null)
                {
                    TxtName.Text = card.Name;
                    TxtLimit.Text = card.CreditLimit.ToString("F2");
                    TxtClosingDay.Text = card.ClosingDay.ToString();
                    TxtDueDay.Text = card.DueDay.ToString();
                }
            }
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
                MessageBox.Show("Ingresa un nombre para la tarjeta.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(TxtLimit.Text, out decimal limit) || limit < 0)
            {
                MessageBox.Show("Ingresa un límite de crédito válido.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(TxtClosingDay.Text, out int closingDay) || closingDay < 1 || closingDay > 31)
            {
                MessageBox.Show("El día de corte debe estar entre 1 y 31.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(TxtDueDay.Text, out int dueDay) || dueDay < 0)
            {
                MessageBox.Show("Ingresa días de gracia válidos.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var context = new ZyroMoneyLocalContext())
            {
                var card = context.CreditCards.FirstOrDefault(c => c.CreditCardId == _cardId);
                if (card != null)
                {
                    card.Name = TxtName.Text.Trim();
                    card.CreditLimit = limit;
                    card.ClosingDay = closingDay;
                    card.DueDay = dueDay;
                    context.SaveChanges();
                }
            }

            DialogResult = true;
            Close();
        }
    }
}
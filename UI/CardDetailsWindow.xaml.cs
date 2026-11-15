using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
using ZyroMoney_DESKTOP.Data;

namespace ZyroMoney_DESKTOP
{
    public partial class CardDetailsWindow : Window
    {
        private readonly int _cardId;

        public CardDetailsWindow(int cardId)
        {
            InitializeComponent();
            _cardId = cardId;
            LoadCardDetails();
        }

        private void LoadCardDetails()
        {
            using (var context = new ZyroMoneyLocalContext())
            {
                var card = context.CreditCards
                    .FirstOrDefault(c => c.CreditCardId == _cardId);

                if (card != null)
                {
                    TxtCardTitle.Text = $"💳 {card.Name}";
                    TxtCardSubtitle.Text = $"Límite: {card.CurrencyCode} {card.CreditLimit:N2} | Corte: Día {card.ClosingDay} | Gracia: Día {card.DueDay}";

                    // Cargar explícitamente filtrando por la tarjeta y que no estén borrados
                    var transactions = context.Transactions
                        .Where(t => t.CreditCardId == _cardId && !t.IsDeleted)
                        .OrderByDescending(t => t.TransactionDate)
                        .ToList();

                    var payments = context.Payments
                        .Where(p => p.CreditCardId == _cardId && !p.IsDeleted)
                        .OrderByDescending(p => p.PaymentDate)
                        .ToList();

                    GridTransactions.ItemsSource = transactions;
                    GridPayments.ItemsSource = payments;
                }
            }
        }

        private void BtnDeleteTransaction_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button btn && btn.Tag is int transactionId)
            {
                var result = MessageBox.Show("¿Estás seguro de eliminar este consumo? El balance se actualizará.", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    using (var context = new ZyroMoneyLocalContext())
                    {
                        var tx = context.Transactions.FirstOrDefault(t => t.TransactionId == transactionId);
                        if (tx != null)
                        {
                            tx.IsDeleted = true; // Borrado lógico para conservar integridad
                            context.SaveChanges();
                        }
                    }
                    LoadCardDetails();
                }
            }
        }

        private void BtnDeletePayment_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button btn && btn.Tag is int paymentId)
            {
                var result = MessageBox.Show("¿Estás seguro de eliminar este pago? El balance se actualizará.", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    using (var context = new ZyroMoneyLocalContext())
                    {
                        var pay = context.Payments.FirstOrDefault(p => p.PaymentId == paymentId);
                        if (pay != null)
                        {
                            pay.IsDeleted = true; // Borrado lógico
                            context.SaveChanges();
                        }
                    }
                    LoadCardDetails();
                }
            }
        }
    }
}
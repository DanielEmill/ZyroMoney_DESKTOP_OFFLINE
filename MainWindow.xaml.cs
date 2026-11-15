using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ZyroMoney_DESKTOP.Models;

namespace ZyroMoney_DESKTOP
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            RefreshDashboard();
        }

        private void RefreshDashboard()
        {
            // 1. Obtener nombre del usuario y tarjetas mediante el servicio
            TxtProfileName.Text = FinancialService.GetUserProfileName();
            var summaries = FinancialService.GetActiveCardSummaries();

            // Enlazar tarjetas a la interfaz
            CardsList.ItemsSource = summaries;

            // --- CÁLCULO DE MÉTRICAS DEL DASHBOARD ---
            var dopCards = summaries.Where(s => s.CurrencyCode == "DOP").ToList();
            var usdCards = summaries.Where(s => s.CurrencyCode == "USD").ToList();

            decimal totalDebtDop = dopCards.Sum(c => c.CurrentBalance);
            decimal totalLimitDop = dopCards.Sum(c => c.CreditLimit);
            decimal availableDop = totalLimitDop - totalDebtDop;
            decimal totalDebtUsd = usdCards.Sum(c => c.CurrentBalance);

            TxtTotalDebtDop.Text = $"DOP {totalDebtDop:N2}";
            TxtTotalDebtUsd.Text = $"USD {totalDebtUsd:N2} en dólares";
            TxtAvailableDop.Text = $"DOP {availableDop:N2}";
            TxtTotalLimitDop.Text = $"Límite total: DOP {totalLimitDop:N2}";

            // Porcentaje de utilización
            if (totalLimitDop > 0)
            {
                decimal utilization = (totalDebtDop / totalLimitDop) * 100;
                TxtUtilization.Text = $"{utilization:N1}%";
                TxtUtilizationHint.Text = utilization <= 30
                    ? "Salud financiera óptima (<30%)"
                    : "Uso elevado, considera abonar";
            }
            else
            {
                TxtUtilization.Text = "0.0%";
                TxtUtilizationHint.Text = "Sin límite DOP registrado";
            }

            // Próximo vencimiento de pago (Basado en Días de Gracia después del Corte)
            if (summaries.Any())
            {
                DateTime today = DateTime.Today;

                var nextCard = summaries
                    .Select(c =>
                    {
                        int daysInCurrentMonth = DateTime.DaysInMonth(today.Year, today.Month);
                        int safeClosingDay = Math.Min(c.ClosingDay, daysInCurrentMonth);
                        DateTime closingDate = new DateTime(today.Year, today.Month, safeClosingDay);

                        // Fecha de vencimiento = Fecha de corte + Días de gracia (DueDay)
                        DateTime dueDate = closingDate.AddDays(c.DueDay);

                        // Si la fecha ya pasó este mes, calculamos la del mes siguiente
                        if (dueDate < today)
                        {
                            DateTime nextMonth = today.AddMonths(1);
                            int nextClosingDay = Math.Min(c.ClosingDay, DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month));
                            dueDate = new DateTime(nextMonth.Year, nextMonth.Month, nextClosingDay).AddDays(c.DueDay);
                        }

                        int daysLeft = (dueDate - today).Days;
                        return new { Card = c, DueDate = dueDate, DaysLeft = daysLeft };
                    })
                    .OrderBy(x => x.DaysLeft)
                    .First();

                TxtNextDueCard.Text = nextCard.Card.Name;
                TxtNextDueDays.Text = nextCard.DaysLeft == 0
                    ? "¡Vence hoy!"
                    : $"Vence en {nextCard.DaysLeft} días ({nextCard.DueDate:dd/MMM})";
            }
            else
            {
                TxtNextDueCard.Text = "Sin tarjetas";
                TxtNextDueDays.Text = "Agrega una tarjeta para iniciar";
            }
        }

        private void BtnAddCard_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddCardWindow { Owner = this };
            if (addWindow.ShowDialog() == true)
            {
                RefreshDashboard();
            }
        }

        private void BtnAddTransaction_Click(object sender, RoutedEventArgs e)
        {
            var txWindow = new AddTransactionWindow { Owner = this };
            if (txWindow.ShowDialog() == true)
            {
                RefreshDashboard();
                MessageBox.Show("Consumo registrado correctamente.", "ZyroMoney", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnAddPayment_Click(object sender, RoutedEventArgs e)
        {
            var payWindow = new AddPaymentWindow { Owner = this };
            if (payWindow.ShowDialog() == true)
            {
                RefreshDashboard();
                MessageBox.Show("Pago registrado y balance actualizado.", "ZyroMoney", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnDeleteCard_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int cardId)
            {
                var result = MessageBox.Show("¿Estás seguro de que deseas eliminar esta tarjeta y sus registros asociados?",
                                             "Confirmar Eliminación",
                                             MessageBoxButton.YesNo,
                                             MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    FinancialService.DeleteCard(cardId);
                    RefreshDashboard();
                }
            }
        }
        private void BtnEditCard_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int cardId)
            {
                var editWindow = new EditCardWindow(cardId) { Owner = this };
                if (editWindow.ShowDialog() == true)
                {
                    RefreshDashboard();
                    MessageBox.Show("Tarjeta actualizada correctamente.", "ZyroMoney", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
        private void BtnViewCardDetails_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int cardId)
            {
                var detailsWindow = new CardDetailsWindow(cardId) { Owner = this };
                detailsWindow.ShowDialog();
                RefreshDashboard(); // Recalcula los balances en el inicio al volver
            }
        }
    }
}
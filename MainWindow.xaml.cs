using System.Linq;
using System.Windows;
using ZyroMoney_DESKTOP.Data;

namespace ZyroMoney_DESKTOP
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadLocalUserData();
        }

        // Carga los datos de prueba desde SQLite al iniciar la ventana
        private void LoadLocalUserData()
        {
            using (var context = new ZyroMoneyLocalContext())
            {
                var user = context.Users.FirstOrDefault();
                if (user != null)
                {
                    TxtUserStatus.Text = $"Perfil activo: {user.Name} (ID: {user.UserId})";
                }
                else
                {
                    TxtUserStatus.Text = "No se encontró ningún usuario local.";
                }
            }
        }

        // Botón para refrescar o probar la conexión con la base de datos
        private void BtnTestDb_Click(object sender, RoutedEventArgs e)
        {
            LoadLocalUserData();
            MessageBox.Show("Conexión con SQLite local exitosa.", "ZyroMoney", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
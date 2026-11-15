using System.IO;
using System.Windows;
using ZyroMoney_DESKTOP.Data;
using ZyroMoney_DESKTOP.Models;

namespace ZyroMoney_DESKTOP
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Crea la base de datos SQLite automáticamente al arrancar
            using (var context = new ZyroMoneyLocalContext())
            {
                context.Database.EnsureCreated();

                // Si no hay usuarios, creamos el perfil local por defecto
                if (!context.Users.Any())
                {
                    var defaultUser = new User
                    {
                        Name = "Mi Perfil Local"
                    };
                    context.Users.Add(defaultUser);
                    context.SaveChanges();
                }
            }
        }
    }
}
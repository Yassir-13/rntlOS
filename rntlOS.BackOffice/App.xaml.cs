using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using rntlOS.BackOffice.Views;
using rntlOS.Core.Data;
using rntlOS.Core.Services;
using System;
using System.Windows;

namespace rntlOS.BackOffice
{
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=RNTLOS_DB;Trusted_Connection=True;"));

            services.AddScoped<UserService>();
            services.AddScoped<ClientService>();
            services.AddScoped<VehiculeService>();
            services.AddScoped<TypeVehiculeService>();
            services.AddScoped<VehiculeImageService>();
            services.AddScoped<BookingService>();
            services.AddScoped<PaiementService>();
            services.AddScoped<MaintenanceService>();
            services.AddScoped<LogService>();

            // UI - CHANGEZ TOUT EN Scoped
            services.AddScoped<MainWindow>();
            services.AddScoped<DashboardView>();
            services.AddScoped<VehiculesView>();
            services.AddScoped<ClientsView>();
            services.AddScoped<BookingsView>();
            services.AddScoped<UsersView>();
            services.AddScoped<MaintenanceView>();
            services.AddScoped<PaiementsView>();
            services.AddTransient<AjouterVehiculeWindow>();
            services.AddTransient<ModifierVehiculeWindow>();
            services.AddTransient<AjouterClientWindow>();
            services.AddTransient<ModifierClientWindow>();
            services.AddTransient<AjouterBookingWindow>();
            services.AddTransient<ModifierBookingWindow>();
            services.AddTransient<AjouterUserWindow>();
            services.AddTransient<ModifierUserWindow>();
            services.AddTransient<AjouterMaintenanceWindow>();
            services.AddTransient<ModifierMaintenanceWindow>();
            services.AddTransient<AjouterPaiementWindow>();
            services.AddTransient<ModifierPaiementWindow>();
            services.AddTransient<LoginWindow>();
            services.AddScoped<VehiculeImageService>();
            services.AddTransient<GererImagesWindow>();
            services.AddScoped<ExcelExportService>();
            services.AddScoped<EmailService>();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Afficher le login
            var userService = ServiceProvider.GetRequiredService<UserService>();
            var loginWindow = new LoginWindow(userService);

            if (loginWindow.ShowDialog() == true)
            {
                // Login réussi
                UserSession.Login(loginWindow.UserId, loginWindow.UserName, loginWindow.UserRole);

                // Ouvrir MainWindow SANS ServiceProvider.GetRequiredService
                var mainWindow = new MainWindow(ServiceProvider);
                Application.Current.MainWindow = mainWindow;
                mainWindow.Show();
            }
            else
            {
                // Login annulé
                Application.Current.Shutdown();
            }
        }
    }
}
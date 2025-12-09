using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using rntlOS.BackOffice.Views;
using rntlOS.Core.Data;
using rntlOS.Core.Services;
using System;
using System.Windows;
using System.Globalization;

namespace rntlOS.BackOffice
{
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }

        public App()
        {
            // Initialiser Serilog
            LogService.InitializeLogger("BackOffice");

            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Ajouter le DbContext avec Factory pour éviter les problèmes de concurrence
            services.AddDbContextFactory<AppDbContext>(options =>
                options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=RNTLOS_DB;Trusted_Connection=True;"));

            services.AddScoped<UserService>();
            services.AddScoped<ClientService>();
            services.AddScoped<VehiculeService>();
            services.AddScoped<TypeVehiculeService>();
            services.AddScoped<VehiculeImageService>();
            services.AddScoped<BookingService>();
            services.AddScoped<PaiementService>();
            services.AddScoped<MaintenanceService>();

            // UI - CHANGEZ TOUT EN Scoped
            services.AddScoped<MainWindow>();
            services.AddScoped<DashboardView>();
            services.AddScoped<VehiculesView>();
            services.AddScoped<ClientsView>();
            services.AddScoped<BookingsView>();
            services.AddScoped<UsersView>();
            services.AddScoped<MaintenanceView>();
            services.AddScoped<PaiementsView>();
            services.AddScoped<LogsView>();

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
            services.AddTransient<GererImagesWindow>();

            services.AddScoped<ExcelExportService>();
            services.AddScoped<ExcelImportService>();
            services.AddScoped<EmailService>();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Configurer la culture marocaine pour afficher MAD au lieu de € ou $
            var culture = new CultureInfo("fr-MA"); // Français Maroc
            culture.NumberFormat.CurrencySymbol = " MAD";
            culture.NumberFormat.CurrencyDecimalDigits = 2;
            culture.NumberFormat.CurrencyPositivePattern = 3; // n MAD (espace avant)

            System.Threading.Thread.CurrentThread.CurrentCulture = culture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

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
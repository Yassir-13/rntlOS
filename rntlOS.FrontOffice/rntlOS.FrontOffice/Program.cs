using Microsoft.EntityFrameworkCore;
using rntlOS.Core.Data;
using rntlOS.Core.Services;
using rntlOS.FrontOffice.Components;

var builder = WebApplication.CreateBuilder(args);

// Initialiser Serilog
LogService.InitializeLogger("FrontOffice");

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Ajouter le DbContext avec Factory pour éviter les problèmes de concurrence
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=RNTLOS_DB;Trusted_Connection=True;"));

// Ajouter les services
builder.Services.AddScoped<ClientService>();
builder.Services.AddScoped<VehiculeService>();
builder.Services.AddScoped<TypeVehiculeService>();
builder.Services.AddScoped<BookingService>();
builder.Services.AddScoped<rntlOS.FrontOffice.Services.AuthenticationStateService>();
builder.Services.AddScoped<rntlOS.Core.PdfService.ReservationPdfService>();

// EmailService avec configuration SMTP
builder.Services.AddSingleton(sp => new EmailService(
    smtpServer: "smtp.gmail.com",
    smtpPort: 587,
    smtpUser: "nacir3030@gmail.com",
    smtpPassword: "mnkrqloqnzffnlug",
    fromEmail: "nacir3030@gmail.com",
    fromName: "rntlOS - Location de Véhicules"
));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
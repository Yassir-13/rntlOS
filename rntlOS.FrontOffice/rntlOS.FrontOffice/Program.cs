using Microsoft.EntityFrameworkCore;
using rntlOS.Core.Data;
using rntlOS.Core.Services;
using rntlOS.FrontOffice.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Ajouter le DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=RNTLOS_DB;Trusted_Connection=True;"));

// Ajouter les services
builder.Services.AddScoped<ClientService>();
builder.Services.AddScoped<VehiculeService>();
builder.Services.AddScoped<TypeVehiculeService>();
builder.Services.AddScoped<BookingService>();

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
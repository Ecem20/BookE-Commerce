using AuthAPI.Data;
using SellBooks.Models;
using SellBooks.Service;
using SellBooks.Service.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using FluentValidation;
using SellBooks.Models.DTO;
using FluentValidation.AspNetCore;
using SellBooksAuthService.Service.IService;
using SellBooksAuthService.Service;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("ApiSettings:JwtOptions"));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

builder.Services.AddHttpClient("Email", u => u.BaseAddress = new Uri(builder.Configuration["ServiceUrls:EmailAPI"]));


//builder.Services.AddControllers();
builder.Services.AddControllers().AddFluentValidation(c => c.RegisterValidatorsFromAssemblyContaining<Program>());


builder.Services.AddScoped<IAutService,AutService>();
builder.Services.AddScoped<IEmailService,EmailService>();   
builder.Services.AddScoped<IJwtTokenGenerator,JwtTokenGenerator>();


// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File($"C:/Users/10132587/source/repos/SellBooks-Son/Logs/SellBooksAuthAPI.txt",
                    outputTemplate: "SellBooksAuthService | {Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30, // Son 30 günlük dosyalarý saklar
                    retainedFileTimeLimit: TimeSpan.FromDays(30)) // 30 günden eski dosyalarý siler

         .WriteTo.MSSqlServer("Server = TRN00282\\SQLEXPRESS; Database = SellBooksLogs; Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True",
                    sinkOptions: new MSSqlServerSinkOptions
                    {
                        TableName = "SellBooksAuthAPI",
                        AutoCreateSqlTable = true
                    }, restrictedToMinimumLevel: LogEventLevel.Information)
    .CreateLogger();

builder.Host.UseSerilog();




// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
ApplyMigration();
app.Run();

void ApplyMigration()
{
    using (var scope = app.Services.CreateScope())
    {
        var _db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (_db.Database.GetPendingMigrations().Count() > 0)
        {
            _db.Database.Migrate();
        }
    }
}


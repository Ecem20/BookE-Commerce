using OrdersAPI.Data;
using AutoMapper;
using OrdersAPI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using OrdersAPI.Extensions;
using OrdersAPI.Service.IService;
using OrdersAPI.Service;
using ShoppingCartAPI;
using ShoppingCartAPI.Service;
using Microsoft.Extensions.DependencyInjection;
using OrdersAPI.Utility;
using FluentValidation.AspNetCore;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File($"C:/Users/10132587/source/repos/SellBooks-Son/Logs/OrderAPI.txt",
                    outputTemplate: "OrderAPI | {Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30, // Son 30 g�nl�k dosyalar� saklar
                    retainedFileTimeLimit: TimeSpan.FromDays(30)) // 30 g�nden eski dosyalar� siler

        .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(le => le.Level == LogEventLevel.Error)
        .WriteTo.File($"C:/Users/10132587/source/repos/SellBooks-Son/ErrorLog/OrderAPI.txt",
            outputTemplate: "OrderAPI | {Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 30,
            retainedFileTimeLimit: TimeSpan.FromDays(30))
    )


    .WriteTo.MSSqlServer("Server = TRN00282\\SQLEXPRESS; Database = SellBooksLogs; Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True",
                    sinkOptions: new MSSqlServerSinkOptions
                    {
                        TableName = "OrderAPI",
                        AutoCreateSqlTable = true
                    }, restrictedToMinimumLevel: LogEventLevel.Information)
    .CreateLogger();


builder.Host.UseSerilog();



builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<ICartService, CartService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped < BackendApiAuthenticationHttpClientHandler>();

builder.Services.AddHttpClient("Book", u => u.BaseAddress = new Uri(builder.Configuration["ServiceUrls:BookAPI"]));
builder.Services.AddHttpClient("Cart", u => u.BaseAddress = new Uri(builder.Configuration["ServiceUrls:CartAPI"]));

builder.Services.AddHttpClient("Auth", u => u.BaseAddress = new Uri(builder.Configuration["ServiceUrls:AuthAPI"]));

//builder.Services.AddControllers();

builder.Services.AddControllers().AddFluentValidation(c => c.RegisterValidatorsFromAssemblyContaining<Program>());


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme, securityScheme: new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter the Bearer Authorization string as following: `Bearer Generated-JWT-Token`",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference= new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id=JwtBearerDefaults.AuthenticationScheme
                }
            }, new string[]{}
        }
    });
});

builder.AddAppAuthetication();

builder.Services.AddAuthorization();

builder.Services.AddExceptionHandler<CustomExceptionHandler>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(_ => { });


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
using AutoMapper;
using EmailAPI;
using EmailAPI.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;

var builder = WebApplication.CreateBuilder(args);


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
    .WriteTo.File($"C:/Users/10132587/source/repos/SellBooks-Son/Logs/EmailAPI.txt",
                    outputTemplate: "EmailAPI | {Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30, // Son 30 günlük dosyalarý saklar
                    retainedFileTimeLimit: TimeSpan.FromDays(30)) // 30 günden eski dosyalarý siler

        .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(le => le.Level == LogEventLevel.Error)
        .WriteTo.File($"C:/Users/10132587/source/repos/SellBooks-Son/ErrorLog/EmailAPI.txt",
            outputTemplate: "EmailAPI | {Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 30,
            retainedFileTimeLimit: TimeSpan.FromDays(30))
    )



    .WriteTo.MSSqlServer("Server = TRN00282\\SQLEXPRESS; Database = SellBooksLogs; Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True",
                    sinkOptions: new MSSqlServerSinkOptions
                    {
                        TableName = "EmailAPI",
                        AutoCreateSqlTable = true
                    }, restrictedToMinimumLevel: LogEventLevel.Information)
    .CreateLogger();

builder.Host.UseSerilog();



// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.UseAuthorization();

app.MapControllers();

app.Run();

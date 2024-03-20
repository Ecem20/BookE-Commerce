using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using MVCWeb.Models;
using MVCWeb.Service;
using MVCWeb.Service.IService;
using MVCWeb.Utility;
using MVCWeb.FluentValidation;
using Microsoft.AspNetCore.Identity;
using MVCWeb.Web.Hubs;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

// Add services to the container.

builder.Services.AddControllersWithViews();//.AddFluentValidation(options =>
                                           //{
                                           //    options.RegisterValidatorsFromAssemblyContaining<Program>();
                                           //});


builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

//builder.Services.AddValidatorsFromAssemblyContaining<BookValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<OrderValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CategoryValidator>();

builder.Services.AddFluentValidationAutoValidation(); // the same old MVC pipeline behavior
builder.Services.AddFluentValidationClientsideAdapters(); // for client side


SD.AuthAPIBase = builder.Configuration["ServiceUrls:AuthAPI"];
SD.BookAPIBase = builder.Configuration["ServiceUrls:BookAPI"];
SD.ShoppingCartAPIBase = builder.Configuration["ServiceUrls:ShoppingCartAPI"];
SD.OrderAPIBase = builder.Configuration["ServiceUrls:OrderAPI"];
SD.EmailAPIBase = builder.Configuration["ServiceUrls:EmailAPI"];

builder.Services.AddHttpClient<IAutService, AutService>();
builder.Services.AddHttpClient<IBookService, BookService>();
builder.Services.AddHttpClient<ICartService, CartService>();
builder.Services.AddHttpClient<IOrderService, OrderService>();
builder.Services.AddHttpClient<IEmailService, EmailService>();
builder.Services.AddHttpClient<ICategoryService, CategoryService>();


builder.Services.AddScoped<ITokenProvider, TokenProvider>();
builder.Services.AddScoped<IBaseService, BaseService>();
builder.Services.AddScoped<IAutService, AutService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ICategoryService,CategoryService>();


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromHours(10);
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/AccessDenied";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<SessionHub>("/session");

app.Run();

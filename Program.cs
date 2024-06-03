using Loto.DataAccessLayer;
using Loto.LotoData;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Text;
var builder = WebApplication.CreateBuilder(args);

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

// Add services to the container.

builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<LotoNumberService>(provider =>
    new LotoNumberService(builder.Configuration));

builder.Services.AddSingleton<LotoTicketsService>(provider =>
    new LotoTicketsService(builder.Configuration));

builder.Services.AddHostedService<DataFetchingService>();

// Add UserService and AuthService
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IEmailService, EmailService>();

// Configure JWT authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login"; // 配置登录路径
        options.AccessDeniedPath = "/Auth/AccessDenied"; // 配置访问被拒绝路径
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // 设置cookie过期时间
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
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
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();

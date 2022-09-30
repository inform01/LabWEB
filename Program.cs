using Crypto;
using Crypto.Models;
using Crypto.Models.Entities.Identity;
using Crypto.Options;
using Crypto.Services.ExternalData;
using Crypto.Services.Identity;
using Crypto.Services.Identity.Implementations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<MailOptions>().BindConfiguration("MailOptions");

var authConnectionString = builder.Configuration.GetConnectionString("DockerAuthConnection");
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(authConnectionString));

var dataConnectionString = builder.Configuration.GetConnectionString("DockerConnection");
builder.Services.AddDbContext<CryptoDbContext>(options =>
    options.UseNpgsql(dataConnectionString));

builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, CryptoClaimsIdentityFactory>();

builder.Services.AddScoped<DataTransformationService>();

builder.Services.AddScoped<ExcelDataService>();
builder.Services.AddScoped<DocxDataService>();

builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddSingleton<ISmtpClientFactory, SmtpClientFactory>();

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedEmail = false;
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 4;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<AuthDbContext>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(PolicyNames.Admin, policyBuilder => policyBuilder
        .RequireAuthenticatedUser()
        .RequireRole(RoleNames.Admin)
        .Build());
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = new PathString("/Identity/Account/Login");
    options.AccessDeniedPath = new PathString("/Identity/Account/AccessDenied");
});

builder.Services.AddRazorPages(options => {
    options.Conventions.AuthorizeFolder("/orders");
    options.Conventions.AuthorizeFolder("/charts", PolicyNames.Admin);
});
builder.Services.AddControllersWithViews();

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

app.Use(async (context, next) => {
    try
    {
        await next(context);
    }
    catch (NotFoundException)
    {
        context.Response.Redirect("/notfound");
    }
});

app.MapRazorPages();
app.MapControllerRoute(
    name: "exchangePairs",
    pattern: "ExchangePairs/{exchangeId:int}/{action}/{id?}",
    defaults: new { controller = "ExchangePairs", action = "Index" });
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

await app.EnsureIdentityInitialized();

await app.RunAsync();

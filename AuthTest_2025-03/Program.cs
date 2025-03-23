using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AuthTest_2025_03.Identity.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using AuthTest_2025_03.Services;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("ApplicationDBContextConnection") ?? throw new InvalidOperationException("Connection string 'ApplicationDBContextConnection' not found.");;

builder.Services.AddDbContext<ApplicationDBContext>(options => options.UseSqlServer(connectionString));

//builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDBContext>();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDBContext>();
    

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddAuthentication(); //CookieAuthenticationDefaults.AuthenticationScheme
    //.AddCookie(x =>
    //{
    //    x.Cookie.SameSite = SameSiteMode.Strict;
    //    x.Cookie.Name = "__AuthTest";
    //    x.Cookie.HttpOnly = false;
    //    x.SlidingExpiration = true;
    //    x.ExpireTimeSpan = TimeSpan.FromMinutes(60);

    //    //x.Events.OnRedirectToLogin = (context) => // Return StatusCode instead of html response // TODO : MRB do this?
    //    //{
    //    //    context.Response.StatusCode =
    //    //      StatusCodes.Status401Unauthorized;
    //    //    return Task.CompletedTask;
    //    //};
    //});
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "__AuthTest";
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.HttpOnly = true;
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
});

builder.Services.AddApplicationServices();

//builder.Services.AddReverseProxy(); // TODO : MRB Configure reverse proxy later


var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // TODO : MRB Add support for JWT later?

app.UseAuthorization();

app.MapControllers();

//app.MapReverseProxy(); // TODO : MRB Configure reverse proxy later

app.Run();

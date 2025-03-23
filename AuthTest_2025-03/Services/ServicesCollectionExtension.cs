using AuthTest_2025_03.Services.Interfaces;

namespace AuthTest_2025_03.Services
{
    public static class ServicesCollectionExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IEmailService, EmailService>();
            return services;
        }
    }
}

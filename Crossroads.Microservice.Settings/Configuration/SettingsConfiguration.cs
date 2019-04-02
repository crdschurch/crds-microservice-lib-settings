using System;
using Microsoft.Extensions.DependencyInjection;

namespace Crossroads.Microservice.Settings
{
    public static class SettingsConfiguration
    {
        public static SettingsService Register(IServiceCollection services)
        {
            SettingsService settingsService = new SettingsService();
            services.AddSingleton<ISettingsService>(settingsService);

            return settingsService;
        }

        public static SettingsService Register(IServiceCollection services, string vaultRoleId, string vaultSecret)
        {
            SettingsService settingsService = new SettingsService(vaultRoleId, vaultSecret);
            services.AddSingleton<ISettingsService>(settingsService);

            return settingsService;
        }

        public static SettingsService Register(IServiceCollection services, string vaultRoleId, string vaultSecret, NLog.ILogger logger)
        {
            SettingsService settingsService = new SettingsService(vaultRoleId, vaultSecret, logger);
            services.AddSingleton<ISettingsService>(settingsService);

            return settingsService;
        }
    }
}

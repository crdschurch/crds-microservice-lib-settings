using System.Collections.Generic;

namespace Crossroads.Microservice.Services
{
    public interface ISettingsService
    {
        string GetSetting(string key);
        bool TryGetSetting(string key, out string secret);

        void AddSettings(Dictionary<string, string> settings, string source);
        void AddSetting(string key, string value, string source);
    }
}
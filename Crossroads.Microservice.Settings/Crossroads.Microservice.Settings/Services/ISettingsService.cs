using System.Collections.Generic;

namespace Crossroads.Microservice.Services
{
    public interface ISettingsService
    {
        string GetSecret(string key);
        void AddAdditionalSettings(Dictionary<string, string> settings, string source);
    }
}
using System;
using Xunit;
using Crossroads.Microservice.Services;
namespace Crossroads.Microservice.Settings.Tests
{
    public class UnitTest1
    {

        [Fact]
        public void GetSecret_NullKey_ThrowsException()
        {
            var service = new SettingsService();

            //TODO: 
            Assert.ThrowsAny<Exception>(() => service.GetSecret(null));
        }

        [Fact]
        public void GetSecret_EmptyStringKey_ThrowsException()
        {
            
        }

        [Fact]
        public void GetSecret_NoKeyInSecrets_ReturnsNull()
        {
            var service = new SettingsService();

            var secret = service.GetSecret("THIS_KEY_DOES_NOT_EXIST");

            Assert.Null(secret);
        }

        //string GetSecret(string key);
        //bool TryGetSecret(string key, out string secret);

        //void AddSettings(Dictionary<string, string> settings, string source);
        //void AddSetting(string key, string value, string source);
    }
}

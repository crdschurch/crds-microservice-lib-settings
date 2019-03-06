using System;
using Xunit;
using Crossroads.Microservice.Services;
namespace Crossroads.Microservice.Settings.Tests
{
    public class UnitTest1
    {
        //string GetSecret(string key);
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
            //TODO:
        }

        [Fact]
        public void GetSecret_NoKeyInSecrets_ReturnsNull()
        {
            var service = new SettingsService();

            var secret = service.GetSecret("THIS_KEY_DOES_NOT_EXIST");

            Assert.Null(secret);
        }

        [Fact]
        public void GetSecret_AddSetting_GetSecret_ReturnsValue()
        {
            string testValue = "test value";

            var service = new SettingsService();
            service.AddSetting("TEST_KEY", testValue, "test source");

            var secret = service.GetSecret("TEST_KEY");

            Assert.Equal(testValue, secret);
        }

        //bool TryGetSecret(string key, out string secret);
        [Fact]
        public void TryGetSecret_NullKey_ThrowsException()
        {
            var service = new SettingsService();

            //TODO:
        }

        [Fact]
        public void TryGetSecret_EmptyStringKey_ThrowsException()
        {
            //TODO:
        }

        [Fact]
        public void TryGetSecret_NoKeyInSecrets_HasNullAsOut()
        {
            var service = new SettingsService();

            string secret;
            service.TryGetSecret("THIS_KEY_DOES_NOT_EXIST", out secret);

            Assert.Null(secret);
        }

        [Fact]
        public void TryGetSecret_NoKeyInSecrets_ReturnsFalse()
        {
            var service = new SettingsService();

            string secret;
            bool sucess = service.TryGetSecret("THIS_KEY_DOES_NOT_EXIST", out secret);

            Assert.False(sucess);
        }

        [Fact]
        public void TryGetSecret_AddSetting_GetSecret_ReturnsValue()
        {
            string testValue = "test value";

            var service = new SettingsService();
            service.AddSetting("TEST_KEY", testValue, "test source");

            string secret;
            bool success = service.TryGetSecret("TEST_KEY", out secret);

            Assert.Equal(testValue, secret);
        }

        [Fact]
        public void TryGetSecret_AddSetting_GetSecret_Returns_True()
        {
            var service = new SettingsService();
            service.AddSetting("TEST_KEY", "test value", "test source");

            string secret;
            bool success = service.TryGetSecret("TEST_KEY", out secret);

            Assert.True(success);
        }

        //void AddSettings(Dictionary<string, string> settings, string source);
        //void AddSetting(string key, string value, string source);
    }
}

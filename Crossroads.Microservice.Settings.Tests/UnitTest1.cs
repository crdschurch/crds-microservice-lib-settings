using System;
using Xunit;
using Crossroads.Microservice.Services;
namespace Crossroads.Microservice.Settings.Tests
{
    public class UnitTest1
    {
        //TODO: Test constructor

        //string GetSetting(string key);
        [Fact]
        public void GetSetting_NullKey_ThrowsException()
        {
            var service = new SettingsService();

            //TODO: 
            Assert.ThrowsAny<Exception>(() => service.GetSetting(null));
        }

        [Fact]
        public void GetSetting_EmptyStringKey_ThrowsException()
        {
            //TODO:
        }

        [Fact]
        public void GetSetting_NoKeyInSettings_ReturnsNull()
        {
            var service = new SettingsService();

            var setting = service.GetSetting("THIS_KEY_DOES_NOT_EXIST");

            Assert.Null(setting);
        }

        [Fact]
        public void GetSetting_AddSetting_GetSetting_ReturnsValue()
        {
            string testValue = "test value";

            var service = new SettingsService();
            service.AddSetting("TEST_KEY", testValue, "test source");

            var setting = service.GetSetting("TEST_KEY");

            Assert.Equal(testValue, setting);
        }

        //bool TryGetSetting(string key, out string setting);
        [Fact]
        public void TryGetSetting_NullKey_ThrowsException()
        {
            var service = new SettingsService();

            //TODO:
        }

        [Fact]
        public void TryGetSetting_EmptyStringKey_ThrowsException()
        {
            //TODO:
        }

        [Fact]
        public void TryGetSetting_NoKeyInSettings_HasNullAsOut()
        {
            var service = new SettingsService();

            string setting;
            service.TryGetSetting("THIS_KEY_DOES_NOT_EXIST", out setting);

            Assert.Null(setting);
        }

        [Fact]
        public void TryGetSetting_NoKeyInSettings_ReturnsFalse()
        {
            var service = new SettingsService();

            string setting;
            bool sucess = service.TryGetSetting("THIS_KEY_DOES_NOT_EXIST", out setting);

            Assert.False(sucess);
        }

        [Fact]
        public void TryGetSetting_AddSetting_GetSetting_ReturnsValue()
        {
            string testValue = "test value";

            var service = new SettingsService();
            service.AddSetting("TEST_KEY", testValue, "test source");

            string setting;
            bool success = service.TryGetSetting("TEST_KEY", out setting);

            Assert.Equal(testValue, setting);
        }

        [Fact]
        public void TryGetSetting_AddSetting_GetSetting_Returns_True()
        {
            var service = new SettingsService();
            service.AddSetting("TEST_KEY", "test value", "test source");

            string setting;
            bool success = service.TryGetSetting("TEST_KEY", out setting);

            Assert.True(success);
        }

        //void AddSettings(Dictionary<string, string> settings, string source);
        //void AddSetting(string key, string value, string source);
    }
}

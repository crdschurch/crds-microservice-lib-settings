using System;
using Xunit;
using Crossroads.Microservice.Services;
using System.Collections.Generic;

namespace Crossroads.Microservice.Settings.Tests
{
    public class UnitTest1
    {
        //Test constructor

        [Fact]
        public void CreateService_NullLogger()
        {
            //TODO:
        }

        public void CreateService_PassLogger()
        {
            //TODO:
        }

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
            string testValue = "test value";

            var service = new SettingsService();
            service.AddSetting("TEST_KEY", testValue, "test source");

            string setting;
            bool success = service.TryGetSetting("TEST_KEY", out setting);

            Assert.True(success);
        }

        [Fact]
        public void AddSettings_AddOneSetting_GetValue()
        {
            string testValue = "test value";
            string testKey = "TEST_KEY";

            var service = new SettingsService();

            var settings = new Dictionary<string, string> {{ testKey, testValue }};

            service.AddSettings(settings, "test source");

            var setting = service.GetSetting(testKey);

            Assert.Equal(testValue, setting);
        }

        [Fact]
        public void AddSettings_AddTwoSettings_GetValues()
        {
            string testValueOne = "test value";
            string testKeyOne = "TEST_KEY";

            string testValueTwo = "test value2";
            string testKeyTwo = "TEST_KEY2";

            var service = new SettingsService();

            var settings = new Dictionary<string, string> { { testKeyOne, testValueOne }, { testKeyTwo, testValueTwo} };

            service.AddSettings(settings, "test source");

            var settingOne = service.GetSetting(testKeyOne);
            var settingTwo = service.GetSetting(testKeyTwo);

            Assert.Equal(testValueOne, settingOne);
            Assert.Equal(testValueTwo, settingTwo);
        }

        [Fact]
        public void AddSettings_OverwriteSetting_GetOverwriteValue()
        {
            string testValueOne = "test value";
            string testKeyOne = "TEST_KEY";

            string testValueTwo = "test value2";
            string testKeyTwo = "TEST_KEY2";

            var service = new SettingsService();

            var settings = new Dictionary<string, string> { { testKeyOne, testValueOne }, { testKeyTwo, testValueTwo } };

            service.AddSettings(settings, "test source");

            string overwriteTestValueOne = "test value one";

            var settingsOverwrite = new Dictionary<string, string> { { testKeyOne, overwriteTestValueOne } };

            service.AddSettings(settingsOverwrite, "overwrite source");

            var settingOne = service.GetSetting(testKeyOne);
            var settingTwo = service.GetSetting(testKeyTwo);

            Assert.Equal(overwriteTestValueOne, settingOne);
            Assert.Equal(testValueTwo, settingTwo);
        }

        [Fact]
        public void AddSettings_EmptySettingsDictionary_DoesNotBreak()
        {
            var service = new SettingsService();

            var settings = new Dictionary<string, string> ();

            service.AddSettings(settings, "test source");
        }

        [Fact]
        public void AddSettings_NullDictionary_ThrowsException()
        {
            var service = new SettingsService();
            Assert.Throws<Exception>(() => { service.AddSettings(null, "test source"); }); //TODO:
        }

        [Fact]
        public void AddSettings_NullSource_ThrowsException()
        {
            var service = new SettingsService();
            var settings = new Dictionary<string, string>();
            Assert.Throws<Exception>(() => { service.AddSettings(settings, null); }); //TODO:
        }

        [Fact]
        public void AddSettings_EmptyStringSource_ThrowsException()
        {
            var service = new SettingsService();
            var settings = new Dictionary<string, string>();
            Assert.Throws<Exception>(() => { service.AddSettings(settings, ""); }); //TODO:
        }

        //void AddSetting(string key, string value, string source);
        [Fact]
        public void AddSetting_AddOneSetting_GetValue()
        {
            string testValue = "test value";
            string testKey = "TEST_KEY";

            var service = new SettingsService();

            service.AddSetting(testKey, testValue, "test source");

            var setting = service.GetSetting(testKey);

            Assert.Equal(testValue, setting);
        }

        [Fact]
        public void AddSetting_AddTwoSettings_GetValues()
        {
            string testValueOne = "test value";
            string testKeyOne = "TEST_KEY";

            string testValueTwo = "test value2";
            string testKeyTwo = "TEST_KEY2";

            var service = new SettingsService();

            service.AddSetting(testKeyOne, testValueOne, "test source");
            service.AddSetting(testKeyTwo, testValueTwo, "test source");

            var settingOne = service.GetSetting(testKeyOne);
            var settingTwo = service.GetSetting(testKeyTwo);

            Assert.Equal(testValueOne, settingOne);
            Assert.Equal(testValueTwo, settingTwo);
        }

        [Fact]
        public void AddSetting_OverwriteSetting_GetOverwriteValue()
        {
            string testValueOne = "test value";
            string testKeyOne = "TEST_KEY";

            string testValueTwo = "test value2";
            string testKeyTwo = "TEST_KEY2";

            var service = new SettingsService();

            service.AddSetting(testKeyOne, testValueOne, "test source");
            service.AddSetting(testKeyTwo, testValueTwo, "test source");

            string overwriteTestValueOne = "test value one";

            service.AddSetting(testKeyOne, overwriteTestValueOne, "test source");

            var settingOne = service.GetSetting(testKeyOne);
            var settingTwo = service.GetSetting(testKeyTwo);

            Assert.Equal(overwriteTestValueOne, settingOne);
            Assert.Equal(testValueTwo, settingTwo);
        }

        [Fact]
        public void AddSetting_NullKey_ThrowsException()
        {
            var service = new SettingsService();
            Assert.Throws<Exception>(() => { service.AddSetting(null, "value" "test source"); }); //TODO:
        }

        [Fact]
        public void AddSetting_NullValue_ThrowsException()
        {
            var service = new SettingsService();
            Assert.Throws<Exception>(() => { service.AddSetting("key", null, "test source"); }); //TODO:
        }

        [Fact]
        public void AddSetting_NullSource_ThrowsException()
        {
            var service = new SettingsService();
            Assert.Throws<Exception>(() => { service.AddSetting("key", "value", null); }); //TODO:
        }

        [Fact]
        public void AddSetting_EmptyStringKey_ThrowsException()
        {
            var service = new SettingsService();
            Assert.Throws<Exception>(() => { service.AddSetting("", "value", "test source"); }); //TODO:
        }

        [Fact]
        public void AddSetting_EmptyStringSource_ThrowsException()
        {
            var service = new SettingsService();
            Assert.Throws<Exception>(() => { service.AddSetting("key", "value", ""); }); //TODO:
        }
    }
}

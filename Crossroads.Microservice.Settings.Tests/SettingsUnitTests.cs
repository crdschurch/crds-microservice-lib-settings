using System;
using Xunit;
using System.Collections.Generic;
using Crossroads.Microservice.Settings.Services;
using NLog.Config;
using NLog.Targets;
using NLog.Web;

namespace Crossroads.Microservice.Settings.Tests
{
    public class UnitTest1
    {

        [Fact]
        public void OverwriteSetting()
        {

            var service = new SettingsService(null);

            service.AddSetting("testKey","value1", "testsource");
            service.AddSetting("testKey", "value2", "testsource");

            Assert.Equal(service.GetSetting("testKey"), "value2");

        }

        [Fact]
        public void CreateService_PassLogger()
        {
            var loggingConfig = new LoggingConfiguration();

            var consoleTarget = new ColoredConsoleTarget("console")
            {
                Layout = @"${date:format=HH\:mm\:ss} ${level} ${message} ${exception:format=ToString}"
            };
            loggingConfig.AddTarget("console", consoleTarget);

            //Log everything in development
            loggingConfig.AddRuleForAllLevels(consoleTarget, "*");

            NLog.LogManager.Configuration = loggingConfig;

            var logger = NLogBuilder.ConfigureNLog(loggingConfig).GetCurrentClassLogger();

            var service = new SettingsService(logger);
        }

        [Fact]
        public void GetSetting_NullKey_ThrowsException()
        {
            var service = new SettingsService();

            Assert.ThrowsAny<Exception>(() => service.GetSetting(null));
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

            string value;

            Assert.ThrowsAny<Exception>(() => service.TryGetSetting(null, out value)); 
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
        public void GetVaultPath_NoValue()
        {
            var service = new SettingsService();
            var vaultPath = service.GetVaultPath();

            Assert.NotNull(vaultPath);
        }

        [Fact]
        public void GetVaultPath_WithValue()
        {
            var service = new SettingsService();
            service.AddSetting("VAULT_URI","test","Testing");
            var vaultPath = service.GetVaultPath();

            Assert.Equal(vaultPath, "test");
        }

        [Fact]
        public void GetCrdsEnv_NoValue()
        {
            var service = new SettingsService();
            var vaultPath = service.GetCrdsEnv();

            Assert.NotNull(vaultPath);
        }

        [Fact]
        public void GetCrdsEnv_WithValue()
        {
            var service = new SettingsService();
            service.AddSetting("CRDS_ENV", "test", "Testing");
            var vaultPath = service.GetCrdsEnv();

            Assert.Equal(vaultPath, "test");
        }


        [Fact]
        public void GetCSettingsFromVault_WithValue()
        {
            var service = new SettingsService();
            service.AddSetting("VAULT_ROLE_ID", "test", "Testing");
            service.AddSetting("VAULT_SECRET_ID", "test", "Testing");

            var vaultPath = service.GetSettingsFromVault("Bucket");

            Assert.Empty(vaultPath);
        }

        [Fact]
        public void GetCSettingsFromVault_WithoutValue()
        {
            var service = new SettingsService();
            service.AddSetting("VAULT_ROLE_ID", "test", "Testing");

            var vaultPath = service.GetSettingsFromVault("Bucket");

            Assert.Empty(vaultPath);
        }



    }
}

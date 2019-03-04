using System;
using System.Collections.Generic;
using System.Net.Http;
using VaultSharp.V1.AuthMethods;
using VaultSharp;
using VaultSharp.V1.AuthMethods.AppRole;
using System.Collections;
using NLog.Config;
using NLog.Targets;
using NLog.Web;

namespace Crossroads.Microservice.Services
{
    public class SettingsService: ISettingsService
    {
        private static NLog.Logger _logger;

        private Dictionary<string,string> appSettings;

        private static readonly HttpClient client = new HttpClient();

        public SettingsService(NLog.Logger logger = null)
        { 
            if (logger == null)
            {
                logger = GetLogger();
            }

            _logger = logger;

            appSettings = new Dictionary<string, string>();

            var envVarSettings = GetSettingsFromEnvironmentVariables();
            AddSettings(envVarSettings, "Environment Variables");

            //Check for env, if it doesn't exist default to local
            string crdsEnv = GetCrdsEnv();

            if (VaultCredentialsAvailable())
            {
                var secretServiceCommonSettings = GetSettingsFromSecretService(crdsEnv, "common");
                AddSettings(secretServiceCommonSettings, "Vault Common");

                var nameOfThisApplication = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
                var secretServiceAppSettings = GetSettingsFromSecretService(crdsEnv, nameOfThisApplication);
                AddSettings(secretServiceAppSettings, "Vault App");
                AddSettings(new Dictionary<string, string> { { "APP_NAME", nameOfThisApplication } }, "App Name");
            }
        }

        public void AddAdditionalSettings(Dictionary<string, string> settings, string source)
        {
            AddSettings(settings, source);
        }

        private NLog.Logger GetLogger()
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

            return NLogBuilder.ConfigureNLog(loggingConfig).GetCurrentClassLogger();
        }

        public string GetSecret(string key)
        {
            if (appSettings.TryGetValue(key, out string value))
            {
                return value;
            }
            else
            {
                AlertKeyNotFound(key);
                return null;
            }
        }

        private bool VaultCredentialsAvailable()
        {
            bool vaultCredentialsAvailable = true;
            if (GetSecret("VAULT_ROLE_ID") == null)
            {
                vaultCredentialsAvailable = false;
                _logger.Warn("VAULT_ROLE_ID not set, unable to get vault secrets");
            }

            if (GetSecret("VAULT_SECRET_ID") == null)
            {
                vaultCredentialsAvailable = false;
                _logger.Warn("VAULT_SECRET_ID not set, unable to get vault secrets");
            }

            return vaultCredentialsAvailable;
        }

        private string GetCrdsEnv()
        {
            //Check for env, if it doesn't exist default to local
            string crdsEnv;
            if (!appSettings.TryGetValue("CRDS_ENV", out crdsEnv))
            {
                crdsEnv = "local";
            }
            _logger.Info("Getting settings for " + crdsEnv + " environment");

            return crdsEnv;
        }

        private void AddSettings(Dictionary<string,string> settings, string source)
        {
            foreach(var setting in settings)
            {
                if (appSettings.ContainsKey(setting.Key))
                {
                    AlertDuplicateKey(setting.Key, source);
                }

                appSettings[setting.Key] = setting.Value;
            }

            _logger.Info("Added settings from " + source);
        }

        private Dictionary<string, string> GetSettingsFromEnvironmentVariables()
        {
            var envSettings = new Dictionary<string, string>();

            try
            {
                var envVars = Environment.GetEnvironmentVariables();

                foreach (DictionaryEntry envVar in envVars)
                {
                    envSettings.Add(envVar.Key.ToString(), envVar.Value.ToString());
                }
            }
            catch(Exception ex)
            {
                var foo = ex;
            }

            return envSettings;
        }

        private Dictionary<string, string> GetSettingsFromSecretService(string env, string bucket)
        {
            var vaultSecrets = new Dictionary<string, string>();

            try
            {
                var vaultRoleId = GetSecret("VAULT_ROLE_ID");
                var vaultSecretId = GetSecret("VAULT_SECRET_ID");
                var vaultPath = GetVaultPath();

                IAuthMethodInfo authMethod = new AppRoleAuthMethodInfo(vaultRoleId, vaultSecretId);
                VaultClientSettings vaultClientSettings = new VaultClientSettings(vaultPath, authMethod);

                IVaultClient vaultClient = new VaultClient(vaultClientSettings);

                // Use client to read a v1 key-value secret.
                var kv2Secret = vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(env + "/" + bucket, mountPoint: "kv").Result;

                var secrets = kv2Secret.Data.Data;
                foreach (var item in secrets)
                {
                    vaultSecrets.Add(item.Key, item.Value.ToString());
                }

            }
            catch (Exception ex)
            {
                _logger.Warn("Unable to load vault settings: " + ex);
            }

            return vaultSecrets;
        }

        private string GetVaultPath()
        {
            string vaultPath = GetSecret("VAULT_URI");

            if (vaultPath == null)
            {
                _logger.Warn("VAULT_URI not set, defaulting to https://vault.crossroads.net");
                vaultPath = "https://vault.crossroads.net";
            }

            return vaultPath;
        }

        private void AlertDuplicateKey(string key, string source)
        {
            _logger.Warn("Duplicate key found: " + key + ". Overwrote with source: " + source);
        }

        private void AlertKeyNotFound(string key)
        {
            _logger.Warn("Key Not Found: " + key);
        }

    }
}

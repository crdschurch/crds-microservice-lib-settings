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
using System.Security;
using System.Diagnostics.Contracts;

namespace Crossroads.Microservice.Services
{
    public class SettingsService: ISettingsService
    {
        //TODO: Make sure this is testable = interface
        private static NLog.ILogger _logger;

        private Dictionary<string,string> appSettings;

        private static readonly HttpClient client = new HttpClient();

        public SettingsService(NLog.ILogger logger = null)
        { 
            // TODO: Double check null is the only way we need to check for this / Check if it is valid? 
            if (logger == null)
            {
                logger = GetLogger();
            }

            //TODO: check to make sure this is legit
            _logger = (NLog.Logger)logger;

            appSettings = new Dictionary<string, string>();

            var envVarSettings = GetSettingsFromEnvironmentVariables();
            AddSettings(envVarSettings, "Environment Variables");

            //Check for env, if it doesn't exist default to local
            string crdsEnv = GetCrdsEnv();

            //TODO: refactor for clarity 
            if (VaultCredentialsAvailable())
            {
                var secretServiceCommonSettings = GetSettingsFromVault(crdsEnv, "common");
                AddSettings(secretServiceCommonSettings, "Vault Common");

                var nameOfThisApplication = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
                var secretServiceAppSettings = GetSettingsFromVault(crdsEnv, nameOfThisApplication);
                AddSettings(secretServiceAppSettings, "Vault App");

                AddSetting("APP_NAME", nameOfThisApplication, "App Name");
            }
        }

        public string GetSetting(string key)
        {
            //TODO: See how this works in a test
            Contract.Requires(!string.IsNullOrEmpty(key), "Key can not be null or empty");

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

        public bool TryGetSetting(string key, out string value)
        {
            var result = appSettings.TryGetValue(key, out string settingValue);

            value = settingValue;

            return result;
        }


        public void AddSettings(Dictionary<string, string> settings, string source)
        {
            //TODO: Test for nulls

            foreach (var setting in settings)
            {
                AddSetting(setting.Key, setting.Value, null);
            }

            _logger.Info("Added settings from " + source);

        }

        public void AddSetting(string key, string value, string source)
        {
            if (appSettings.ContainsKey(key))
            {
                AlertDuplicateKey(key, source);

            }
            //TODO: Check to see what happens when this exists
            appSettings[key] = value;

            if (!string.IsNullOrEmpty(source))
            {
                _logger.Info("Added setting from " + source);
            }

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



        private bool VaultCredentialsAvailable()
        {
            bool vaultCredentialsAvailable = true;

            if (GetSetting("VAULT_ROLE_ID") == null)
            {
                vaultCredentialsAvailable = false;
                _logger.Warn("VAULT_ROLE_ID not set, unable to get vault secrets");
            }

            if (GetSetting("VAULT_SECRET_ID") == null)
            {
                vaultCredentialsAvailable = false;
                _logger.Warn("VAULT_SECRET_ID not set, unable to get vault secrets");
            }



            return vaultCredentialsAvailable;
        }

        private string GetCrdsEnv()
        {
            //Check for env, if it doesn't exist default to local
            string crdsEnv = "local";

            //TODO: replace this use dogfood
            if (!appSettings.TryGetValue("CRDS_ENV", out crdsEnv))
            {
                crdsEnv = "local";
            }
            _logger.Info("Getting settings for " + crdsEnv + " environment");

            return crdsEnv;
        }


        private Dictionary<string, string> GetSettingsFromEnvironmentVariables()
        {
            var envSettings = new Dictionary<string, string>();

            try
            { 
                var envVars = Environment.GetEnvironmentVariables();

                foreach(DictionaryEntry envVar in envVars)
                {
                    envSettings.Add(envVar.Key.ToString(), envVar.Value.ToString());
                }

            }
            catch(SecurityException ex)
            {
                //TODO: Hey, there were issues reading env vars, woops
                var foo = ex;
            }
            catch(OutOfMemoryException ex) 
            {
                //TODO: Hey, there were issues reading env vars, woops
                var foo = ex;
            }

            return envSettings;
        }

        private Dictionary<string, string> GetCommonSettingsFromVault()
        {
            var vaultSecrets = new Dictionary<string, string>();

            return vaultSecrets;
        }

        private Dictionary<string, string> GetApplicationSettingsFromVault()
        {
            var vaultSecrets = new Dictionary<string, string>();

            return vaultSecrets;
        }

        private Dictionary<string, string> GetSettingsFromVault(string env, string bucket)
        {
            var vaultSecrets = new Dictionary<string, string>();

            try
            {
                var vaultRoleId = GetSetting("VAULT_ROLE_ID");
                var vaultSecretId = GetSetting("VAULT_SECRET_ID");
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
            string vaultPath = GetSetting("VAULT_URI");

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

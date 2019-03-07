﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security;
using Crossroads.Microservice.Services;
using NLog.Config;
using NLog.Targets;
using NLog.Web;
using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.AppRole;

[assembly: InternalsVisibleTo("Crossroads.Microservice.Settings.Tests")]

namespace Crossroads.Microservice.Settings.Services
{
    public class SettingsService: ISettingsService
    {
        //TODO: Make sure this is testable = interface
        private static NLog.ILogger _logger;

        private readonly Dictionary<string,string> _appSettings;


        public SettingsService(NLog.ILogger logger = null)
        { 

            if (logger == null)
            {
                logger = GetLogger();
            }

            //TODO: check to make sure this is legit
            _logger = (NLog.Logger)logger;

            _appSettings = new Dictionary<string, string>();

            var envVarSettings = GetSettingsFromEnvironmentVariables();
            AddSettings(envVarSettings, "Environment Variables");

            var vaultCommonSettings = GetCommonSettingsFromVault();
            AddSettings(vaultCommonSettings, "Vault Common Settings");

            var vaultApplicationSettings = GetApplicationSettingsFromVault();
            AddSettings(vaultApplicationSettings, "Vault Application Settings");

        }

        public string GetSetting(string key)
        {
            if (_appSettings.TryGetValue(key, out var value))
            {
                return value;
            }

            AlertKeyNotFound(key);
            return null;
        }

        public bool TryGetSetting(string key, out string value)
        {
            var result = _appSettings.TryGetValue(key, out var settingValue);

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
            if (_appSettings.ContainsKey(key))
            {
                AlertDuplicateKey(key, source);

            }
            //TODO: Check to see what happens when this exists
            _appSettings[key] = value;

            if (!string.IsNullOrEmpty(source))
            {
                _logger.Info("Added setting from " + source);
            }

        }

        private static NLog.Logger GetLogger()
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
            var vaultCredentialsAvailable = true;

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

        internal string GetCrdsEnv()
        {
            //Check for env, if it doesn't exist default to local
            if (!TryGetSetting("CRDS_ENV", out var environment))
            {
                environment = "local";
            }

            _logger.Info($"Getting settings for {environment} environment");

            return environment;
        }


        private static Dictionary<string, string> GetSettingsFromEnvironmentVariables()
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
                _logger.Warn($"Unable to load vault settings: {ex}");
            }
            catch(OutOfMemoryException ex) 
            {
                _logger.Warn($"Unable to load vault settings: {ex}");
            }

            return envSettings;
        }

        private Dictionary<string, string> GetCommonSettingsFromVault()
        {
            var vaultSecrets = new Dictionary<string, string>();
 
            vaultSecrets = GetSettingsFromVault("common");

            return vaultSecrets;
        }

        private Dictionary<string, string> GetApplicationSettingsFromVault()
        {
            var vaultSettings = new Dictionary<string, string>();

            var nameOfThisApplication = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;


            vaultSettings = GetSettingsFromVault(nameOfThisApplication);

            return vaultSettings;
        }

        internal Dictionary<string, string> GetSettingsFromVault(string bucket)
        {
            var vaultSecrets = new Dictionary<string, string>();

            if (!VaultCredentialsAvailable())
            {
                return vaultSecrets;
            }

            var environment = GetCrdsEnv();

            try
            {
                var vaultRoleId = GetSetting("VAULT_ROLE_ID");
                var vaultSecretId = GetSetting("VAULT_SECRET_ID");
                var vaultPath = GetVaultPath();

                IAuthMethodInfo authMethod = new AppRoleAuthMethodInfo(vaultRoleId, vaultSecretId);
                var vaultClientSettings = new VaultClientSettings(vaultPath, authMethod);

                IVaultClient vaultClient = new VaultClient(vaultClientSettings);

                // Use client to read a v1 key-value secret.
                var kv2Secret = vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(environment + "/" + bucket, mountPoint: "kv").Result;

                var secrets = kv2Secret.Data.Data;
                foreach (var item in secrets)
                {
                    vaultSecrets.Add(item.Key, item.Value.ToString());
                }

            }
            catch (Exception ex)
            {
                _logger.Warn($"Unable to load vault settings: {ex}");
            }

            return vaultSecrets;
        }

        internal string GetVaultPath()
        {
            var vaultPath = GetSetting("VAULT_URI");

            if (vaultPath != null) return vaultPath;

            _logger.Warn("VAULT_URI not set, defaulting to https://vault.crossroads.net");
            vaultPath = "https://vault.crossroads.net";

            return vaultPath;
        }

        private static void AlertDuplicateKey(string key, string source)
        {
            _logger.Warn($"Duplicate key found: {key}. Overwrote with source: {source}");
        }

        private static void AlertKeyNotFound(string key)
        {
            _logger.Warn($"Key Not Found: {key}");
        }

    }
}

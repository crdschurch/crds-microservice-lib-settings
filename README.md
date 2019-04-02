# crds-microservice-lib-settings
Dotnet microservice library for settings

This settings library is meant to standardize the access of application settings (env variables, other sources) in both production-like and local environments.

# Setup
The library can be initialized by pasting the following lines into the `ConfigureServices` function in your StartUp file.

```
SettingsConfiguration.Register();
```
or if you want to explicitly define the vault role id and secret
```
SettingsConfiguration.Register("vault_role_id", "vault_secret");
```
or if you also want to pass in a logger to log initialization errors. By default it will log to stdout
```
SettingsConfiguration.Register("vault_role_id", "vault_secret", logger);
```


You will need to use the `Crossroads.Microservice.Settings` namespace.

You will also need two environment variables set so that the settings service can reach out to vault:

`VAULT_ROLE_ID`
`VAULT_SECRET_ID`

Alternatively you can pass them into the register function as shown above.
If you don't have those values reach out to other developers.

Optionally you can provide the `VAULT_URI`, however it defaults to the current server location.

# Usage
Once this is set up you can use the settings service by injecting the `ISettingsService` interface into your classes.

Injection example:

```
private ISettingsService _settingsService;
public OktaRegistrationService(ISettingsService settingsService)
{
    _settingsService = settingsService;
}
```

Functions available are:

- `string GetSetting(string setting)`
- `bool TryGetSetting(string setting, out string value)`
- `void AddSettings(Dictionary<string, string> settings, string source)`
- `void AddSetting(string key, string value, string source)`

Usage example:
`var secretSetting = _settingsService.GetSetting('SECRET_KEY');`
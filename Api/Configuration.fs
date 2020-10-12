namespace Api

open System.IO
open Microsoft.Extensions.Configuration
open FsConfig

type Configuration =
    { DbConnectionString: string
      Database: string
      Container: string }

module Configuration =
    let build (appSettingsFilePath: string) =
        let configurationRoot =
            ConfigurationBuilder().AddJsonFile(appSettingsFilePath).Build()

        let appConfig =
            AppConfig(configurationRoot).Get<Configuration>(fun _ s -> s)

        match appConfig with
        | Ok config -> config
        | Error error ->
            match error with
            | NotFound envVarName -> failwithf "Environment variable %s not found" envVarName
            | BadValue (envVarName, value) -> failwithf "Environment variable %s has invalid value: %s" envVarName value
            | NotSupported msg -> failwith msg

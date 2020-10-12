open System.IO
open Expecto
open Test.Common.Helpers
open Api

let appSettingsFilenameTest = "appsettings-test.json"
let appSettingsFilePathTest = Path.Combine(Directory.GetCurrentDirectory (), "..", "..", appSettingsFilenameTest)

let config = Configuration.build appSettingsFilePathTest
let db = config.Database
let container = config.Container
let connStr = config.DbConnectionString

[<Tests>]
let tests = 
    let testListUnit =
        testList "Unit" <|testListAppend  [ ]
    let testListIntegration =
        // testList "Integration" <|testListAppend  [ Test.Integration.Common.Infrastructure.Persistence.ComosDb.tests db container connStr ]
        testList "Integration" <|testListAppend  []

    testList "Common" <| testListAppend [ testListUnit; testListIntegration ]

[<EntryPoint>]
let main argv =
    Tests.runTestsInAssemblyWithCLIArgs [] argv
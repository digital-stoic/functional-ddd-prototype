open System.IO
open Expecto
open Test.Common.Helpers
open Api

let appSettingsFilenameTest = "appsettings-test.json"
let appSettingsFilePathTest = Path.Combine(Directory.GetCurrentDirectory (), "..", "..", appSettingsFilenameTest)

let config = Configuration.build appSettingsFilePathTest

[<Tests>]
let tests = 
    let testListUnit =
        testList "Unit" <|testListAppend  [ ]
    let testListIntegration =
        testList "Integration" <|testListAppend  [ Test.Integration.Common.Infrastructure.Persistence.ComosDb.tests ]

    testList "Api" <| testListAppend [ testListUnit; testListIntegration ]

[<EntryPoint>]
let main argv =
    printfn "%A" config.DbConnectionString
    Tests.runTestsInAssemblyWithCLIArgs [] argv
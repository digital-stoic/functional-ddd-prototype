open Expecto
open Test.Common.Helpers

[<Tests>]
let tests = 
    let testListUnit =
        testList "Unit" <|testListAppend  [ Test.Unit.Api.Configuration.tests ]
    let testListIntegration =
        testList "Integration" <|testListAppend  [ Test.Integration.Api.Ping.tests ]

    testList "Api" <| testListAppend [ testListUnit; testListIntegration ]

[<EntryPoint>]
let main argv =
    printfn "Reminder: Web App must be running before launching the Integration tests"
    Tests.runTestsInAssemblyWithCLIArgs [] argv

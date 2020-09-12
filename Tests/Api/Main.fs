﻿open Expecto
open Test.Common.Helpers

[<Tests>]
let tests = 
    let testListUnit =
        testList "Unit" <|testListAppend  [ Test.Unit.Api.Configuration.tests ]
    let testListIntegration =
        testList "Integration" <|testListAppend  [ ]

    testList "Api" <| testListAppend [ testListUnit; testListIntegration ]

[<EntryPoint>]
let main argv =
    Tests.runTestsInAssemblyWithCLIArgs [] argv

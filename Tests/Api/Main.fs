open Expecto
open Test.Common.Helpers

[<Tests>]
let tests = 
    testList "Api" <| testListAppend [ Test.Unit.Api.Configuration.tests ]

[<EntryPoint>]
let main argv =
    Tests.runTestsInAssemblyWithCLIArgs [] argv

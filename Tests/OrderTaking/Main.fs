open Expecto
open Test.Common.Helpers

[<Tests>]
let tests =
    let testListUnit =
        testList "Unit" <|testListAppend [
            Test.Unit.OrderTaking.Common.Types.Simple.tests
            Test.Unit.OrderTaking.PlaceOrder.Domain.tests
            Test.Unit.OrderTaking.PlaceOrder.Dto.tests ]
    let testListIntegration =
        testList "Integration" <|testListAppend  [ ]

    testList "OrderTaking" <| testListAppend [ testListUnit; testListIntegration ]

[<EntryPoint>]
let main argv =
    Tests.runTestsInAssemblyWithCLIArgs [] argv

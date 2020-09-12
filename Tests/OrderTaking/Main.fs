open Expecto
open Test.Common.Helpers

[<Tests>]
let tests =
    testList "OrderTaking"
    <| testListAppend [ Test.Unit.OrderTaking.Common.Types.Simple.tests
                        Test.Unit.OrderTaking.PlaceOrder.Domain.tests
                        Test.Unit.OrderTaking.PlaceOrder.Dto.tests ]

[<EntryPoint>]
let main argv =
    // Tests.runTestsInAssembly defaultConfig argv
    Tests.runTestsInAssemblyWithCLIArgs [] argv

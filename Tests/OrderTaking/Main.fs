open Expecto
open Test.Common.Helpers

[<Tests>]
let tests =
    testList "OrderTaking"
    <| testListAppend [ Test.OrderTaking.Common.Types.Simple.tests
                        Test.OrderTaking.PlaceOrder.Domain.tests
                        Test.OrderTaking.PlaceOrder.Dto.tests ]

[<EntryPoint>]
let main argv =
    // Tests.runTestsInAssembly defaultConfig argv
    Tests.runTestsInAssemblyWithCLIArgs [] argv

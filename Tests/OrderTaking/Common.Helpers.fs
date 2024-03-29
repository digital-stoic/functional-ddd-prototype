//==============================================================================
// Test Helpers
//==============================================================================

module Test.OrderTaking.Common.Helpers

open Expecto.Flip.Expect

// equal for Result
let equalR message expected actual =
    match actual with
    | Ok a -> equal message expected a
    | Error e -> isTrue "Force fail" false

// equal for constrained types
let equalRV typeValue message expected actual =
    match actual with
    | Ok a -> equal message expected (a |> typeValue)
    | Error _ -> isTrue "Force fail" false

// equal for optional constrained types
let equalRVO typeValue message expected actual =
    match actual with
    | Ok None -> equal message expected null
    | Ok (Some a) -> equal message expected (a |> typeValue)
    | Error _ -> isTrue "Force fail" false

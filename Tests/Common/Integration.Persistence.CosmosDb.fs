module Test.Integration.Common.Infrastructure.Persistence.ComosDb


open Expecto
open Expecto.Flip.Expect
open Test.Common.Helpers
open Common.Infrastructure.Persistence.ComosDb

//==============================================================================
// Data
//==============================================================================



//==============================================================================
// Helpers
//==============================================================================



//==============================================================================
// Tests
//==============================================================================

// let testListCosmosDb
//     testList
//         "WithoutDefault"
//         [ test "DbConnectionString" {
//               let config =
//                   Configuration.build appSettingsFilenameOK1

//               config.DbConnectionString
//               |> equal "Config parsed" dbConnectionStringOK1
//           } ]

let tests =
    testList "Common.Infrastructure.Persistence.CosmosDb"
    <| testListAppend []

module Test.Unit.Api.Configuration

open Expecto
open Expecto.Flip.Expect
open Test.Common.Helpers
open Api

//==============================================================================
// Data
//==============================================================================

let appSettingsFilenameOK1 = "appsettings.json"

let dbConnectionStringOK1 =
    "AccountEndpoint=https://dummy.documents.azure.com:443/;AccountKey=dummy==;"

//==============================================================================
// Helpers
//==============================================================================

// TODO: Helper with record field name as parameter. See:
//   - https://fpish.net/topic/None/57493
//   - http://tomasp.net/blog/fsharp-dynamic-lookup.aspx/

//==============================================================================
// Tests
//==============================================================================

let testListWithoutDefault =
    testList
        "WithoutDefault"
        [ test "DbConnectionString" {
              let config =
                  Configuration.build appSettingsFilenameOK1

              config.DbConnectionString
              |> equal "Config parsed" dbConnectionStringOK1
          } ]

let tests =
    testList "Configuration"
    <| testListAppend [ testListWithoutDefault ]

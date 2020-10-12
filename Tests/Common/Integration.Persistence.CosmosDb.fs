module Test.Integration.Common.Infrastructure.Persistence.ComosDb

open System
open Expecto
open Expecto.Flip.Expect
open Test.Common.Helpers
open Common.Infrastructure.Persistence.ComosDb

//==============================================================================
// Data
//==============================================================================

[<CLIMutable>]
type ItemType =
    { id: string
      key: string
      Question: string
      Answer: int }

let guidOK1 = Guid.NewGuid().ToString()

let partitionKeyOK1 = "key1"

let itemOK1 =
    { id = guidOK1
      key = partitionKeyOK1
      Question = "Life, Universe and Everything"
      Answer = 42 }

let itemOK2 = { itemOK1 with Answer = 43 }

//==============================================================================
// Helpers
//==============================================================================


//==============================================================================
// Tests
//==============================================================================

let tests database container connectionString =
    testList
        "PersistencesDb.CosmosDb"
        [ testAsync "Insert an item" {
              let! result =
                  async {
                      return insert<ItemType> database container connectionString itemOK1
                             |> Async.RunSynchronously
                  }

              result |> equal "Item inserted" [ itemOK1 ]
          }

          testAsync "Update an item by ID" {
              printfn "ID: %A" guidOK1
              let! result =
                  async {
                      //   return updateById<ItemType> database container connectionString guidOK1 partitionKeyOK1 itemOK2
                      return updateById<ItemType>
                                 database
                                 container
                                 connectionString
                                 "af829c89-358b-434c-9260-b52b11c57622"
                                 partitionKeyOK1
                                 itemOK2
                             |> Async.RunSynchronously
                  }

              result |> equal "Item inserted" [ itemOK2 ]
          }

          test "Dummy" { equal "dummy" 42 42 } ]

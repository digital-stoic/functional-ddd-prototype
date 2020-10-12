module Common.Infrastructure.Persistence.ComosDb

open FSharp.Control
open FSharp.CosmosDb

let insert<'T> database container dbConnectionString item =
    dbConnectionString
    |> Cosmos.fromConnectionString
    |> Cosmos.database database
    |> Cosmos.container container
    |> Cosmos.insert<'T> item
    |> Cosmos.execAsync<'T>
    |> AsyncSeq.toListAsync

let updateById<'T> database container dbConnectionString id partitionKey newItem =
    dbConnectionString
    |> Cosmos.fromConnectionString
    |> Cosmos.database database
    |> Cosmos.container container
    |> Cosmos.update<'T> id partitionKey (fun _ -> newItem)
    |> Cosmos.execAsync<'T>
    |> AsyncSeq.toListAsync

// TODO: updateByCondition?

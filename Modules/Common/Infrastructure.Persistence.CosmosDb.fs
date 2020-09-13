module Common.Infrastructure.Persistence.ComosDb

open FSharp.Control
open FSharp.CosmosDb

let insert<'T> database container dbConnectionString data =
    dbConnectionString
    |> Cosmos.fromConnectionString
    |> Cosmos.database database
    |> Cosmos.container container
    |> Cosmos.insert<'T> data
    |> Cosmos.execAsync<'T>
    |> AsyncSeq.toListAsync

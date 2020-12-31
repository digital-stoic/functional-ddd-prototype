module Test.Integration.Api.Ping

open Expecto
open Expecto.Flip.Expect
// open Test.Common.Helpers
open Hopac
open HttpFs.Client

//==============================================================================
// Data
//==============================================================================

let req =
    Request.createUrl Get "http://localhost:8085/ping"
    |> Request.setHeader (Accept "application/json")

//==============================================================================
// Helpers
//==============================================================================

//==============================================================================
// Tests
//==============================================================================

// TODO: Other Ping tests
let tests =
    testList
        "Ping"
        [ testAsync "Base" {
            use! res = req |> getResponse |> Alt.toAsync
            let! b = Response.readBodyAsString res |> Job.toAsync
            res.statusCode |> equal "Status checked" 200
            b |> equal "Body checked" @"""Pong!"""
          } ]

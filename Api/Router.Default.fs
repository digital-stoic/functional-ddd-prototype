module Router.Default

open Saturn
open Giraffe.ResponseWriters
open Giraffe.HttpStatusCodeHandlers

let defaultPipeline = pipeline { plug acceptJson }

type PingAnswer =
    { Question: string
      Answer: int
      Result: Result<string, string>
      Comments: string option }

let defaultPingAnswer =
    { Question = "Ultimate Question of Life, the Universe and Everything"
      Answer = 42
      Result = Ok "Right answer"
      Comments = None }

let checkPingAnswer answer =
    match answer with
    | 42 -> Successful.OK defaultPingAnswer
    | _ ->
        let comments = Some $"Not {answer}..."
        let result = Error "Wrong answer"

        Successful.OK
            { defaultPingAnswer with
                  Result = result
                  Comments = comments }

let Ping =
    // TODO: Add POST with payload
    router {
        not_found_handler (text "Unknown resource")
        pipe_through defaultPipeline
        get "/ping" (Successful.OK "Pong!")
        get "/ping/text" (Successful.OK $"{defaultPingAnswer}")
        get "/ping/json" (Successful.OK defaultPingAnswer)
        getf "/ping/%i" checkPingAnswer
    }

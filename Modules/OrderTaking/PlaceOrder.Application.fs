module OrderTaking.PlaceOrder.Application

open Newtonsoft.Json
open OrderTaking.PlaceOrder

type JsonString = string

//==============================================================================
// Dependencies
//==============================================================================

// let chekProductExists: OrderTaking.PlaceOrder.Domain.CheckProductCodeExists = fun productCode -> true
let chekProductExists: Domain.CheckProductCodeExists = fun productCode -> true

//==============================================================================
// Workflow
//==============================================================================

// let commandJsonStr : JsonString =
//     """{ "command": 'placeOrder; }"""

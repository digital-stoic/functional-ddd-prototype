//
// Simple and constrained types
//
namespace OrderTaking.Common.Types.Simple

// TODO:
// - EntityId with UUIDs?
//

open System
// FIXME: Is the right Result used?
// FIXME: How to open by default in all the files?
open FsToolkit.ErrorHandling

type String50 = private String50 of string

type EmailAddress = private EmailAddress of string

type ZipCode = private ZipCode of string

type OrderId = private OrderId of string

type OrderLineId = private OrderLineId of string

type WidgetCode = private WidgetCode of string

type GizmoCode = private GizmoCode of string

type ProductCode =
    | Widget of WidgetCode
    | Gizmo of GizmoCode

type UnitQuantity = private UnitQuantity of int

type KilogramQuantity = private KilogramQuantity of decimal

type OrderQuantity =
    | Unit of UnitQuantity
    | Kilogram of KilogramQuantity

type Price = private Price of decimal

type BillingAmount = private BillingAmount of decimal

module ConstrainedTypes =
    let createString fieldName ctor maxLen str =
        if String.IsNullOrEmpty(str) then
            let msg =
                sprintf "%s must not be null or empty" fieldName

            Error msg
        elif str.Length > maxLen then
            let msg =
                sprintf "%s must not be more than %i chars" fieldName maxLen

            Error msg
        else
            Ok(ctor str)

    let createStringOption fieldName ctor maxLen str =
        if String.IsNullOrEmpty(str) then
            Ok None
        elif str.Length > maxLen then
            let msg =
                sprintf "%s must not be more than %i chars" fieldName maxLen

            Error msg
        else
            Ok <| Some(ctor str)

    let createLike fieldName ctor pattern str =
        if String.IsNullOrEmpty(str) then
            let msg =
                sprintf "%s: Must not be null or empty" fieldName

            Error msg
        elif System.Text.RegularExpressions.Regex.IsMatch(str, pattern) then
            Ok <| ctor str
        else
            let msg =
                sprintf "%s: '%s' must match the pattern '%s'" fieldName str pattern

            Error msg

    let createInt fieldName ctor minVal maxVal i =
        if i < minVal then
            let msg =
                sprintf "%s: Must not be less than %i" fieldName i

            Error msg
        elif i > maxVal then
            let msg =
                sprintf "%s: Must not be greater than %i" fieldName i

            Error msg
        else
            Ok <| ctor i

    let createDecimal fieldName ctor minVal maxVal x =
        if x < minVal then
            let msg =
                sprintf "%s: Must not be less than %M" fieldName x

            Error msg
        elif x > maxVal then
            let msg =
                sprintf "%s: Must not be greater than %M" fieldName x

            Error msg
        else
            Ok <| ctor x

    let unsafeCreate create fieldName str =
        create fieldName str
        |> function
        | Ok v -> v
        | Error err -> failwithf "%s: Out of bounds '%s'" fieldName err

// TODO: String50 vs String50NotEmpty
module String50 =
    let value (String50 str) = str

    let create fieldName str =
        ConstrainedTypes.createString fieldName String50 50 str

    let createOption fieldName str =
        ConstrainedTypes.createStringOption fieldName String50 50 str

    let unsafeCreate fieldName str =
        ConstrainedTypes.unsafeCreate create fieldName str

    let unsafeCreateOption fieldName str =
        ConstrainedTypes.unsafeCreate createOption fieldName str

module EmailAddress =
    let value (EmailAddress str) = str

    let create fieldName str =
        // From http://emailregex.com/
        let pattern =
            """^(?(")(".+?(?<!\\)"@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$"""

        ConstrainedTypes.createLike fieldName EmailAddress pattern str

    let unsafeCreate fieldName str =
        ConstrainedTypes.unsafeCreate create fieldName str

module ZipCode =
    let value (ZipCode str) = str

    let create fieldName str =
        let pattern = @"^\d{5}$"
        ConstrainedTypes.createLike fieldName ZipCode pattern str

    let unsafeCreate fieldName str =
        ConstrainedTypes.unsafeCreate create fieldName str

module OrderId =
    let value (OrderId str) = str

    let create fieldName str =
        ConstrainedTypes.createString fieldName OrderId 50 str

    let unsafeCreate fieldName str =
        ConstrainedTypes.unsafeCreate create fieldName str

module OrderLineId =
    let value (OrderLineId str) = str

    let create fieldName str =
        ConstrainedTypes.createString fieldName OrderLineId 50 str

    let unsafeCreate fieldName str =
        ConstrainedTypes.unsafeCreate create fieldName str

module WidgetCode =
    let value (WidgetCode str) = str

    let create fieldName str =
        let pattern = @"^W\d{4}$"
        ConstrainedTypes.createLike fieldName WidgetCode pattern str

module GizmoCode =
    let value (GizmoCode str) = str

    let create fieldName str =
        let pattern = @"^G\d{3}$"
        ConstrainedTypes.createLike fieldName GizmoCode pattern str

module ProductCode =

    let value productCode =
        match productCode with
        | Widget (WidgetCode str) -> str
        | Gizmo (GizmoCode str) -> str

    let create fieldName str =
        if String.IsNullOrEmpty(str) then
            let msg =
                sprintf "%s: Must not be null or empty" fieldName

            Error msg
        else if str.StartsWith("W") then
            WidgetCode.create fieldName str
            |> Result.map Widget
        else if str.StartsWith("G") then
            GizmoCode.create fieldName str |> Result.map Gizmo
        else
            let msg =
                sprintf "%s: Format not recognized '%s'" fieldName str

            Error msg

    let unsafeCreate fieldName str =
        ConstrainedTypes.unsafeCreate create fieldName str

module UnitQuantity =
    let value (UnitQuantity i) = i

    let create fieldName i =
        ConstrainedTypes.createInt fieldName UnitQuantity 1 1000 i

    let unsafeCreate fieldName str =
        ConstrainedTypes.unsafeCreate create fieldName str

module KilogramQuantity =
    let value (KilogramQuantity x) = x

    let create fieldName x =
        ConstrainedTypes.createDecimal fieldName KilogramQuantity 0.5M 100M x

    let unsafeCreate fieldName str =
        ConstrainedTypes.unsafeCreate create fieldName str

module OrderQuantity =
    let value qty =
        match qty with
        | Unit uq -> uq |> UnitQuantity.value |> decimal
        | Kilogram kq -> kq |> KilogramQuantity.value

    let create fieldName productCode quantity =
        match productCode with
        | Widget _ ->
            UnitQuantity.create fieldName (int quantity)
            |> Result.map OrderQuantity.Unit
        | Gizmo _ ->
            KilogramQuantity.create fieldName quantity
            |> Result.map OrderQuantity.Kilogram

    let unsafeCreate fieldName productCode quantity =
        match productCode with
        | Widget _ ->
            Unit
            <| UnitQuantity.unsafeCreate fieldName (int quantity)
        | Gizmo _ ->
            Kilogram
            <| KilogramQuantity.unsafeCreate fieldName quantity

module Price =
    let value (Price p) = p

    let create fieldName price =
        ConstrainedTypes.createDecimal fieldName Price 0M 1000M price

    let unsafeCreate fieldName str =
        ConstrainedTypes.unsafeCreate create fieldName str

    let multiply fieldName qty (Price p) = create fieldName (qty * p)

module BillingAmount =
    let value (BillingAmount x) = x

    let create fieldName price =
        ConstrainedTypes.createDecimal fieldName BillingAmount 0M 10000M price

    let unsafeCreate fieldName str =
        ConstrainedTypes.unsafeCreate create fieldName str

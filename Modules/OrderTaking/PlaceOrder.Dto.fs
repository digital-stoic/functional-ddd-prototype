namespace OrderTaking.PlaceOrder.Dto

open System.Collections.Generic
open FsToolkit.ErrorHandling
open OrderTaking.Common.Types.Simple
open OrderTaking.Common.Types.Compound
open OrderTaking.PlaceOrder.Types.Public

type CustomerInfoDto =
    { FirstName: string
      LastName: string
      EmailAddress: string }

module internal CustomerInfoDto =
    let toUnvalidatedCustomerInfo (dto: CustomerInfoDto): UnvalidatedCustomerInfo =
        let domainObj: UnvalidatedCustomerInfo =
            { FirstName = dto.FirstName
              LastName = dto.LastName
              EmailAddress = dto.EmailAddress }

        domainObj

    let toCustomerInfo (dto: CustomerInfoDto): Result<CustomerInfo, string> =
        result {
            let! first = dto.FirstName |> String50.create "FirstName"
            let! last = dto.LastName |> String50.create "LastName"

            let! email =
                dto.EmailAddress
                |> EmailAddress.create "EmailAddress"

            let name = { FirstName = first; LastName = last }

            let info = { Name = name; EmailAddress = email }

            return info
        }

    let fromDomain (domainObj: CustomerInfo): CustomerInfoDto =
        { FirstName = domainObj.Name.FirstName |> String50.value
          LastName = domainObj.Name.LastName |> String50.value
          EmailAddress = domainObj.EmailAddress |> EmailAddress.value }

type AddressDto =
    { AddressLine1: string
      AddressLine2: string
      AddressLine3: string
      AddressLine4: string
      City: string
      ZipCode: string }

module internal AddressDto =
    let toUnvalidatedAddress (dto: AddressDto): UnvalidatedAddress =
        { AddressLine1 = dto.AddressLine1
          AddressLine2 = dto.AddressLine2
          AddressLine3 = dto.AddressLine3
          AddressLine4 = dto.AddressLine4
          City = dto.City
          ZipCode = dto.ZipCode }

    let toAddress (dto: AddressDto): Result<Address, string> =
        result {
            let! addressLine1 = dto.AddressLine1 |> String50.create "AddressLine1"

            let! addressLine2 =
                dto.AddressLine2
                |> String50.createOption "AddressLine1"

            let! addressLine3 =
                dto.AddressLine3
                |> String50.createOption "AddressLine1"

            let! addressLine4 =
                dto.AddressLine4
                |> String50.createOption "AddressLine1"

            let! city = dto.City |> String50.create "City"

            let! zipCode = dto.ZipCode |> ZipCode.create "ZipCode"

            let address: Address =
                { AddressLine1 = addressLine1
                  AddressLine2 = addressLine2
                  AddressLine3 = addressLine3
                  AddressLine4 = addressLine4
                  City = city
                  ZipCode = zipCode }

            return address
        }

    let fromDomain (domainObj: Address): AddressDto =
        { AddressLine1 = domainObj.AddressLine1 |> String50.value
          AddressLine2 =
              domainObj.AddressLine2
              |> Option.map String50.value
              |> Option.defaultValue null
          AddressLine3 =
              domainObj.AddressLine3
              |> Option.map String50.value
              |> Option.defaultValue null
          AddressLine4 =
              domainObj.AddressLine4
              |> Option.map String50.value
              |> Option.defaultValue null
          City = domainObj.City |> String50.value
          ZipCode = domainObj.ZipCode |> ZipCode.value }

type OrderLineDto =
    { OrderLineId: string
      ProductCode: string
      Quantity: decimal }

module internal OrderLineDto =
    let toUnvalidatedOrderLine (dto: OrderLineDto): UnvalidatedOrderLine =
        { OrderLineId = dto.OrderLineId
          ProductCode = dto.ProductCode
          Quantity = dto.Quantity }

type PricedOrderLineDto =
    { OrderLineId: string
      ProductCode: string
      Quantity: decimal
      LinePrice: decimal }

module internal PricedOrderLineDto =
    let fromDomain (domainObj: PricedOrderLine): PricedOrderLineDto =
        { OrderLineId = domainObj.OrderLineId |> OrderLineId.value
          ProductCode = domainObj.ProductCode |> ProductCode.value
          Quantity = domainObj.Quantity |> OrderQuantity.value
          LinePrice = domainObj.LinePrice |> Price.value }

type OrderDto =
    { OrderId: string
      CustomerInfo: CustomerInfoDto
      ShippingAddress: AddressDto
      BillingAddress: AddressDto
      Lines: OrderLineDto list }

module internal OrderDto =
    let toUnvalidatedOrder (dto: OrderDto): UnvalidatedOrder =
        { OrderId = dto.OrderId
          CustomerInfo =
              dto.CustomerInfo
              |> CustomerInfoDto.toUnvalidatedCustomerInfo
          ShippingAddress =
              dto.ShippingAddress
              |> AddressDto.toUnvalidatedAddress
          BillingAddress =
              dto.BillingAddress
              |> AddressDto.toUnvalidatedAddress
          Lines =
              dto.Lines
              |> List.map OrderLineDto.toUnvalidatedOrderLine }

type PlacedOrderDto =
    { OrderId: string
      CustomerInfo: CustomerInfoDto
      ShippingAddress: AddressDto
      BillingAddress: AddressDto
      AmountToBill: decimal
      Lines: PricedOrderLineDto list }

module internal PlacedOrderDto =
    let fromDomain (domainObj: PlacedOrder): PlacedOrderDto =
        { OrderId = domainObj.OrderId |> OrderId.value
          CustomerInfo =
              domainObj.CustomerInfo
              |> CustomerInfoDto.fromDomain
          ShippingAddress = domainObj.ShippingAddress |> AddressDto.fromDomain
          BillingAddress = domainObj.BillingAddress |> AddressDto.fromDomain
          AmountToBill = domainObj.AmountToBill |> BillingAmount.value
          Lines =
              domainObj.Lines
              |> List.map PricedOrderLineDto.fromDomain }

type BillableOrderPlacedDto =
    { OrderId: string
      BillingAddress: AddressDto
      AmountToBill: decimal }

module internal BillableOrderPlacedDto =
    let fromDomain (domainObj: BillableOrderPlaced): BillableOrderPlacedDto =
        { OrderId = domainObj.OrderId |> OrderId.value
          BillingAddress = domainObj.BillingAddress |> AddressDto.fromDomain
          AmountToBill = domainObj.AmountToBill |> BillingAmount.value }

type OrderAcknowledgementSentDto =
    { OrderId: string
      EmailAddress: string }

module internal OrderAcknowledgementSentDto =
    let fromOrderAcknowledgementSent (domainObj: OrderAcknowledgementSent): OrderAcknowledgementSentDto =
        { OrderId = domainObj.OrderId |> OrderId.value
          EmailAddress = domainObj.EmailAddress |> EmailAddress.value }

// TODO: Check if Map works for JSON serialization (need to define equality for IDictionary)
// type OrderPlacedEventDto = IDictionary<string, obj>
type OrderPlacedEventDto = Map<string, obj>

module OrderPlacedEventDto =
    let fromDomain (domainEvent: OrderPlacedEvent): OrderPlacedEventDto =
        match domainEvent with
        | OrderPlaced e ->
            let obj = e |> PlacedOrderDto.fromDomain |> box
            let key = "OrderPlaced"
            // [ (key, obj) ] |> dict
            Map.empty.Add(key, obj)
        | BillableOrderPlaced e ->
            let obj =
                e |> BillableOrderPlacedDto.fromDomain |> box

            let key = "BillableOrderPlaced"
            // [ (key, obj) ] |> dict
            Map.empty.Add(key, obj)
        | AcknowledgementSent e ->
            let obj =
                e
                |> OrderAcknowledgementSentDto.fromOrderAcknowledgementSent
                |> box

            let key = "OrderAcknowledgementSent"
            // [ (key, obj) ] |> dict
            Map.empty.Add(key, obj)

type PlaceOrderErrorDto = { Code: string; Message: string }

module internal PlaceOrderErrorDto =
    let fromDomain (domainObj: PlaceOrderError): PlaceOrderErrorDto =
        match domainObj with
        | Validation (ValidationError msg) ->
            { Code = "ValidationError"
              Message = msg }
        | Pricing (PricingError msg) -> { Code = "PricingError"; Message = msg }
        | RemoteService err ->
            let msg =
                sprintf "%s: %s" err.Service.Name err.Exception.Message

            { Code = "RemoteServiceError"
              Message = msg }

module Test.OrderTaking.Common.Data

open Test.OrderTaking.Common.Stubs
open OrderTaking.Common.Types.Simple
open OrderTaking.Common.Types.Compound
open OrderTaking.PlaceOrder.Types.Public
open OrderTaking.PlaceOrder.Domain
open OrderTaking.PlaceOrder.Dto

//==============================================================================
// Test Data
//==============================================================================

//==============================================================================
// Simple Types
//==============================================================================

let string50OK1 = "This string is < 50"

let stringLonger50NOK1 = String.init 51 (fun _ -> "x")

let emailStrOK1 = "joe.black@example.com"

let emailStrNOK1 = "joe"

let zipCodeStrOK1 = "12345"

let zipCodeNotEnoughDigitsStrNOK1 = "1234"

let zipCodeNotOnlyDigitsStrNOK1 = "1234X"

let orderIdStrOK1 = "O1234"

let orderIdStrNOK1 = stringLonger50NOK1

let orderLineIdStrOK1 = "OL12345"

let orderLineIdStrOK2 = "OL67890"

let orderLineIdStrNOK1 = stringLonger50NOK1

let productCodeWidgetStrOK1 = "W1234"

let productCodeGizmoStrOK1 = "G123"

let productCodeWidgetStrNOK1 = "G123"

let productCodeGizmoStrNOK1 = "W1234"

let productCodeStrNOK1 = "X4567"

let unitQuantityIntOK1 = 10

let unitQuantityLess1IntNOK1 = 0

let unitQuantityLarger1000IntNOK1 = 1001

let kilogramQuantityDecOK1 = 10M

let kilogramQuantityLess05DecNOK1 = 0.49M

let kilogramQuantityLarger100DecNOK1 = 101M

let orderQuantityDecOK1 = decimal unitQuantityIntOK1

let orderQuantityDecOK2 = kilogramQuantityDecOK1

let orderQuantityLarger1000DecNOK1 = decimal unitQuantityLarger1000IntNOK1

let orderQuantityLess05DecNOK1 = kilogramQuantityLess05DecNOK1

let priceDecOK1 = 100M

let priceLess0NOK1 = -1M

let priceLarger1000NOK1 = 1001M

let billingAmountDecOK1 = 1000M

let billingAmountLess0DecNOK1 = -1M

let billingAmoungLarger10000DecNOK1 = 10001M

//==============================================================================
// Input DTO
//==============================================================================

let customerInfoDtoOK1: CustomerInfoDto =
    { FirstName = "Joe"
      LastName = "Black"
      EmailAddress = emailStrOK1 }

let customerInfoDtoEmptyFirstNameNOK1 =
    { customerInfoDtoOK1 with
          FirstName = "" }

let customerInfoDtoNullFirstNameNOK1 =
    { customerInfoDtoOK1 with
          FirstName = null }

let customerInfoDtoLonger50FirstNameNOK1 =
    { customerInfoDtoOK1 with
          FirstName = stringLonger50NOK1 }

// TODO: Many more edge cases with simple types. Try FsCheck?

let addressDtoOK1: AddressDto =
    { AddressLine1 = "42 Road Central"
      AddressLine2 = null
      AddressLine3 = null
      AddressLine4 = null
      City = "Nowhere"
      ZipCode = "42424" }

let orderLineDtoOK1: OrderLineDto =
    { OrderLineId = "OL123"
      ProductCode = productCodeWidgetStrOK1
      Quantity = 10M }

let orderLineDtoOK2: OrderLineDto =
    { OrderLineId = "OL456"
      ProductCode = productCodeGizmoStrOK1
      Quantity = 20.5M }

let orderNoLineDtoOK1 =
    { OrderId = orderIdStrOK1
      CustomerInfo = customerInfoDtoOK1
      ShippingAddress = addressDtoOK1
      BillingAddress = addressDtoOK1
      Lines = [] }

let orderOneLineDtoOK1 =
    { orderNoLineDtoOK1 with
          Lines = [ orderLineDtoOK1 ] }

let orderTwoLinesDtoOK1 =
    { orderNoLineDtoOK1 with
          Lines = [ orderLineDtoOK1; orderLineDtoOK2 ] }

//==============================================================================
// Domain Types
//==============================================================================

let orderIdOK1 =
    OrderId.unsafeCreate "OrderId" orderIdStrOK1

let unvalidatedCustomerInfoOK1: UnvalidatedCustomerInfo =
    { FirstName = customerInfoDtoOK1.FirstName
      LastName = customerInfoDtoOK1.LastName
      EmailAddress = emailStrOK1 }

let customerInfoOK1 =
    { Name =
          { FirstName = String50.unsafeCreate "FirstName" unvalidatedCustomerInfoOK1.FirstName
            LastName = String50.unsafeCreate "LastName" unvalidatedCustomerInfoOK1.LastName }
      EmailAddress = EmailAddress.unsafeCreate "EmailAddress" unvalidatedCustomerInfoOK1.EmailAddress }

let unvalidatedAddressOK1: UnvalidatedAddress =
    { AddressLine1 = addressDtoOK1.AddressLine1
      AddressLine2 = addressDtoOK1.AddressLine2
      AddressLine3 = addressDtoOK1.AddressLine3
      AddressLine4 = addressDtoOK1.AddressLine4
      City = addressDtoOK1.City
      ZipCode = addressDtoOK1.ZipCode }

let addressOK1: Address =
    { AddressLine1 = String50.unsafeCreate "AddressLine1" unvalidatedAddressOK1.AddressLine1
      AddressLine2 = String50.unsafeCreateOption "AddressLine2" unvalidatedAddressOK1.AddressLine2
      AddressLine3 = String50.unsafeCreateOption "AddressLine3" unvalidatedAddressOK1.AddressLine3
      AddressLine4 = String50.unsafeCreateOption "AddressLine4" unvalidatedAddressOK1.AddressLine4
      City = String50.unsafeCreate "City" unvalidatedAddressOK1.City
      ZipCode = ZipCode.unsafeCreate "ZipCode" unvalidatedAddressOK1.ZipCode }

let unvalidatedOrderLineOK1: UnvalidatedOrderLine =
    { OrderLineId = orderLineDtoOK1.OrderLineId
      ProductCode = orderLineDtoOK1.ProductCode
      Quantity = orderLineDtoOK1.Quantity }

let unvalidatedOrderLineOK2: UnvalidatedOrderLine =
    { OrderLineId = orderLineDtoOK2.OrderLineId
      ProductCode = orderLineDtoOK2.ProductCode
      Quantity = orderLineDtoOK2.Quantity }

let unvalidatedOrderNoLineOK1: UnvalidatedOrder =
    { OrderId = orderIdStrOK1
      CustomerInfo = unvalidatedCustomerInfoOK1
      ShippingAddress = unvalidatedAddressOK1
      BillingAddress = unvalidatedAddressOK1
      Lines = [] }

let unvalidatedOrderOneLineOK1: UnvalidatedOrder =
    { unvalidatedOrderNoLineOK1 with
          Lines = [ unvalidatedOrderLineOK1 ] }

let unvalidatedOrderTwoLinesOK1: UnvalidatedOrder =
    { unvalidatedOrderNoLineOK1 with
          Lines =
              [ unvalidatedOrderLineOK1
                unvalidatedOrderLineOK2 ] }

let unValidatedOrderOrderIdNOK1: UnvalidatedOrder =
    { unvalidatedOrderTwoLinesOK1 with
          OrderId = orderIdStrNOK1 }

// TODO: Add cases for invalid customer info, address, etc. Try FsCheck?

let productCodeWidgetOK1 =
    ProductCode.unsafeCreate "ProductCode" productCodeWidgetStrOK1

let productCodeGizmoOK1 =
    ProductCode.unsafeCreate "ProductCode" productCodeGizmoStrOK1

let validatedOrderLineOK1: ValidatedOrderLine =
    { OrderLineId = OrderLineId.unsafeCreate "OrderLinedId" unvalidatedOrderLineOK1.OrderLineId
      ProductCode = productCodeWidgetOK1
      Quantity = OrderQuantity.unsafeCreate "Quantity" productCodeWidgetOK1 unvalidatedOrderLineOK1.Quantity }

let validatedOrderLineOK2: ValidatedOrderLine =
    { OrderLineId = OrderLineId.unsafeCreate "OrderLinedId" unvalidatedOrderLineOK2.OrderLineId
      ProductCode = productCodeGizmoOK1
      Quantity = OrderQuantity.unsafeCreate "Quantity" productCodeGizmoOK1 unvalidatedOrderLineOK2.Quantity }

let validatedOrderNoLineOK1: ValidatedOrder =
    { OrderId = orderIdOK1
      CustomerInfo = customerInfoOK1
      ShippingAddress = addressOK1
      BillingAddress = addressOK1
      Lines = [] }

let validatedOrderOneLineOK1: ValidatedOrder =
    { validatedOrderNoLineOK1 with
          Lines = [ validatedOrderLineOK1 ] }

let validatedOrderTwoLinesOK1: ValidatedOrder =
    { validatedOrderNoLineOK1 with
          Lines =
              [ validatedOrderLineOK1
                validatedOrderLineOK2 ] }

let multiplyQuantityPrice getProductPrice fieldName quantity productCode =
    let priceProduct = getProductPriceOK productCode

    let quantityDecimal =
        match quantity with
        | Unit u -> u |> UnitQuantity.value |> decimal
        | Kilogram k -> k |> KilogramQuantity.value

    Price.unsafeCreate fieldName (quantityDecimal * (Price.value priceProduct))

let linePriceOK1 =
    multiplyQuantityPrice getProductPriceOK "LinePrice" validatedOrderLineOK1.Quantity validatedOrderLineOK1.ProductCode

let linePriceOK2 =
    multiplyQuantityPrice getProductPriceOK "LinePrice" validatedOrderLineOK2.Quantity validatedOrderLineOK2.ProductCode

let amountToBillNoLineOK1 =
    BillingAmount.unsafeCreate "AmountToBill" 0M

let amountToBillOneLineOK1 =
    BillingAmount.unsafeCreate "AmountToBill" (Price.value linePriceOK1)

let amountToBillTwoLinesOK1 =
    BillingAmount.unsafeCreate
        "AmountToBill"
        (Price.value linePriceOK1
         + Price.value linePriceOK2)

let pricedOrderLineOK1: PricedOrderLine =
    { OrderLineId = validatedOrderLineOK1.OrderLineId
      ProductCode = validatedOrderLineOK1.ProductCode
      Quantity = validatedOrderLineOK1.Quantity
      LinePrice = linePriceOK1 }

let pricedOrderLineOK2: PricedOrderLine =
    { OrderLineId = validatedOrderLineOK2.OrderLineId
      ProductCode = validatedOrderLineOK2.ProductCode
      Quantity = validatedOrderLineOK2.Quantity
      LinePrice = linePriceOK2 }

let pricedOrderNoLineOK1: PricedOrder =
    { OrderId = orderIdOK1
      CustomerInfo = customerInfoOK1
      ShippingAddress = addressOK1
      BillingAddress = addressOK1
      AmountToBill = amountToBillNoLineOK1
      Lines = [] }

let pricedOrderOneLineOK1 =
    { pricedOrderNoLineOK1 with
          AmountToBill = amountToBillOneLineOK1
          Lines = [ pricedOrderLineOK1 ] }

let pricedOrderTwoLinesOK1 =
    { pricedOrderNoLineOK1 with
          AmountToBill = amountToBillTwoLinesOK1
          Lines =
              [ pricedOrderLineOK1
                pricedOrderLineOK2 ] }

let orderAcknowledgementTwoLinesOK1 =
    { EmailAddress = pricedOrderTwoLinesOK1.CustomerInfo.EmailAddress
      Letter = createAcknowledgementLetterOK pricedOrderTwoLinesOK1 }

let orderAcknowledgementOneLineOK1 =
    { EmailAddress = pricedOrderOneLineOK1.CustomerInfo.EmailAddress
      Letter = createAcknowledgementLetterOK pricedOrderOneLineOK1 }

let orderAcknowledgementNoLineOK1 =
    { EmailAddress = pricedOrderNoLineOK1.CustomerInfo.EmailAddress
      Letter = createAcknowledgementLetterOK pricedOrderNoLineOK1 }

let orderAcknowledgementSentTwoLinesOK1: OrderAcknowledgementSent option =
    Some
        { OrderId = pricedOrderTwoLinesOK1.OrderId
          EmailAddress = pricedOrderNoLineOK1.CustomerInfo.EmailAddress }

let orderAcknowledgementSentNOK1 = None

let acknowledgmentSentTwoLinesOK1 =
    AcknowledgementSent
        { EmailAddress = pricedOrderTwoLinesOK1.CustomerInfo.EmailAddress
          OrderId = pricedOrderTwoLinesOK1.OrderId }

let acknowledgmentSentOneLineOK1 =
    AcknowledgementSent
        { EmailAddress = pricedOrderOneLineOK1.CustomerInfo.EmailAddress
          OrderId = pricedOrderOneLineOK1.OrderId }

let acknowledgmentSentNoLineOK1 =
    AcknowledgementSent
        { EmailAddress = pricedOrderNoLineOK1.CustomerInfo.EmailAddress
          OrderId = pricedOrderNoLineOK1.OrderId }

let placedOrderNoLineOK1: PlacedOrder = pricedOrderNoLineOK1

let placedOrderOneLineOK1: PlacedOrder = pricedOrderOneLineOK1

let placedOrderTwoLinesOK1: PlacedOrder = pricedOrderTwoLinesOK1

let orderPlacedNoLineOK1 = OrderPlaced placedOrderNoLineOK1

let orderPlacedOneLineOK1 = OrderPlaced placedOrderOneLineOK1

let orderPlacedTwoLinesOK1 = OrderPlaced placedOrderTwoLinesOK1

let billableOrderPlacedTwoLinesOK1 =
    BillableOrderPlaced
        { AmountToBill = pricedOrderTwoLinesOK1.AmountToBill
          BillingAddress = pricedOrderTwoLinesOK1.BillingAddress
          OrderId = pricedOrderTwoLinesOK1.OrderId }

let billableOrderPlacedOneLineOK1 =
    BillableOrderPlaced
        { AmountToBill = pricedOrderOneLineOK1.AmountToBill
          BillingAddress = pricedOrderOneLineOK1.BillingAddress
          OrderId = pricedOrderOneLineOK1.OrderId }

let eventsTwoLinesOK1 =
    [ acknowledgmentSentTwoLinesOK1
      orderPlacedTwoLinesOK1
      billableOrderPlacedTwoLinesOK1 ]

let eventsOneLineOK1 =
    [ acknowledgmentSentOneLineOK1
      orderPlacedOneLineOK1
      billableOrderPlacedOneLineOK1 ]

let eventsNoLineOK1 =
    [ acknowledgmentSentNoLineOK1
      orderPlacedNoLineOK1 ]

let eventsTwoLinesNoAcknowledgementSentOK1 =
    [ orderPlacedTwoLinesOK1
      billableOrderPlacedTwoLinesOK1 ]

let placedOrderErrorValidationStrOK1 = "Validation error: placed order"

let placedOrderErrorPricingStrOK1 = "Pricing error: placed order"

let placedOrderErrorRemotServiceStrOK1 = "Remote service: placed order"

let placedOrderErrorPricingOK1 =
    Pricing
    <| PricingError "Pricing error: placed order"

let placedOrderErrorValidationOK1 =
    Validation
    <| ValidationError placedOrderErrorValidationStrOK1

let remoteServiceError =
    { Exception = System.TimeoutException()
      Service =
          { Name = "Service test"
            Endpoint = System.Uri("https://example.com") } }

let placedOrderErrorRemoteServiceOK1 = RemoteService remoteServiceError

//==============================================================================
// Output DTO
//==============================================================================

let pricedOrderLineDtoOK1: PricedOrderLineDto =
    { OrderLineId =
          pricedOrderLineOK1.OrderLineId
          |> OrderLineId.value
      ProductCode =
          pricedOrderLineOK1.ProductCode
          |> ProductCode.value
      Quantity = pricedOrderLineOK1.Quantity |> OrderQuantity.value
      LinePrice = pricedOrderLineOK1.LinePrice |> Price.value }

let placedOrderNoLineDtoOK1: PlacedOrderDto =
    { OrderId = pricedOrderNoLineOK1.OrderId |> OrderId.value
      CustomerInfo =
          pricedOrderNoLineOK1.CustomerInfo
          |> CustomerInfoDto.fromDomain
      ShippingAddress =
          pricedOrderNoLineOK1.ShippingAddress
          |> AddressDto.fromDomain
      BillingAddress =
          pricedOrderNoLineOK1.BillingAddress
          |> AddressDto.fromDomain
      AmountToBill =
          pricedOrderNoLineOK1.AmountToBill
          |> BillingAmount.value
      Lines =
          pricedOrderNoLineOK1.Lines
          |> List.map (PricedOrderLineDto.fromDomain) }

let placedOrderOneLineDtoOK1: PlacedOrderDto =
    { placedOrderNoLineDtoOK1 with
          AmountToBill =
              pricedOrderOneLineOK1.AmountToBill
              |> BillingAmount.value
          Lines =
              pricedOrderOneLineOK1.Lines
              |> List.map (PricedOrderLineDto.fromDomain) }

let placedOrderTwoLinesDtoO1: PlacedOrderDto =
    { placedOrderNoLineDtoOK1 with
          AmountToBill =
              pricedOrderTwoLinesOK1.AmountToBill
              |> BillingAmount.value
          Lines =
              pricedOrderTwoLinesOK1.Lines
              |> List.map (PricedOrderLineDto.fromDomain) }

// TODO: Check if Map works for JSON serialization
let orderPlacedNoLineDtoOK1: OrderPlacedEventDto =
    let key = "OrderPlaced"
    let obj = placedOrderNoLineDtoOK1 |> box
    // [ (key, obj) ] |> dict
    Map.empty.Add(key, obj)

let orderPlacedOneLineDtoOK1: OrderPlacedEventDto =
    let key = "OrderPlaced"
    let obj = placedOrderOneLineDtoOK1 |> box
    // [ (key, obj) ] |> dict
    Map.empty.Add(key, obj)

let orderPlacedTwoLinesDtoOK1: OrderPlacedEventDto =
    let key = "OrderPlaced"
    let obj = placedOrderTwoLinesDtoO1 |> box
    // [ (key, obj) ] |> dict
    Map.empty.Add(key, obj)

let placedOrderErrorDtoValidationOK1 =
    { Code = "ValidationError"
      Message = placedOrderErrorValidationStrOK1 }

let placedOrderErrorDtoPricingOK1 =
    { Code = "PricingError"
      Message = placedOrderErrorPricingStrOK1 }

let placedOrderErrorDtoRemoteServiceOK1msg =
    sprintf "%s: %s" remoteServiceError.Service.Name remoteServiceError.Exception.Message

let placedOrderErrorDtoRemoteServiceOK1 =
    { Code = "RemoteServiceError"
      Message = placedOrderErrorDtoRemoteServiceOK1msg }

// FIXME: Better formatting for worklow self-documentation
// FIXME: internal prevents testing
// module internal OrderTaking.PlaceOrder.Domain
module OrderTaking.PlaceOrder.Domain

open FsToolkit.ErrorHandling
open OrderTaking.Common.Types.Simple
open OrderTaking.Common.Types.Compound
open OrderTaking.PlaceOrder.Types.Public

//==============================================================================
// 1 Definitions
//==============================================================================

//==============================================================================
// Step: Validate Order
//==============================================================================

// Dependency
type CheckProductCodeExists = ProductCode -> bool

type AddressValidationError =
    | InvalidFormat
    | AddressNotFound

type CheckedAddress = CheckedAddress of UnvalidatedAddress

// Dependency
type CheckAddressExists = UnvalidatedAddress -> Async<Result<CheckedAddress, AddressValidationError>>

type ValidatedOrderLine =
    { OrderLineId: OrderLineId
      ProductCode: ProductCode
      Quantity: OrderQuantity }

type ValidatedOrder =
    { OrderId: OrderId
      CustomerInfo: CustomerInfo
      ShippingAddress: Address
      BillingAddress: Address
      Lines: ValidatedOrderLine list }

// Workflow step
type ValidateOrder =
    CheckProductCodeExists -> CheckAddressExists -> UnvalidatedOrder -> Async<Result<ValidatedOrder, ValidationError>>

//==============================================================================
// Step: Price Order
//==============================================================================

// Dependency
type GetProductPrice = ProductCode -> Price

// Workflow step
type PriceOrder = GetProductPrice -> ValidatedOrder -> Result<PricedOrder, PricingError>

//==============================================================================
// Step: Acknowledge Order
//==============================================================================

type HtmlString = HtmlString of string

type OrderAcknowledgement =
    { EmailAddress: EmailAddress
      Letter: HtmlString }

type CreateOrderAcknowledgementLetter = PricedOrder -> HtmlString

type SendResult =
    | Sent
    | NotSent

type SendOrderAcknowledgement = OrderAcknowledgement -> SendResult

// Workflow step
type AcknowledgeOrder =
    CreateOrderAcknowledgementLetter -> SendOrderAcknowledgement -> PricedOrder -> OrderAcknowledgementSent option

//==============================================================================
// Events
//==============================================================================

type CreateEvents = PricedOrder -> OrderAcknowledgementSent option -> OrderPlacedEvent list

//==============================================================================
// 2 Implementation
//==============================================================================

//==============================================================================
// Helpers
//==============================================================================

let createString50MapError fieldName str =
    String50.create fieldName str
    |> Result.mapError ValidationError

let createString50OptionMapError fieldName str =
    String50.createOption fieldName str
    |> Result.mapError ValidationError

// Adapters
let toOrderId str =
    str
    |> OrderId.create "OrderId"
    |> Result.mapError ValidationError

let toCustomerInfo (unvalidatedCustomerInfo: UnvalidatedCustomerInfo) =
    result {
        let! firstName =
            unvalidatedCustomerInfo.FirstName
            |> createString50MapError "FirstName"

        let! lastName =
            unvalidatedCustomerInfo.LastName
            |> createString50MapError "LastName"

        let! emailAddress =
            unvalidatedCustomerInfo.EmailAddress
            |> EmailAddress.create "EmailAddress"
            |> Result.mapError ValidationError

        let customerInfo =
            { Name =
                  { FirstName = firstName
                    LastName = lastName }
              EmailAddress = emailAddress }

        return customerInfo
    }

let toCheckedAddress (checkAddress: CheckAddressExists) address =
    address
    |> checkAddress
    |> AsyncResult.mapError (fun addrError ->
        match addrError with
        | AddressNotFound -> ValidationError "Address not found"
        | InvalidFormat -> ValidationError "Address has bad format")

let toAddress (CheckedAddress unvalidatedAddress) =

    result {
        let! addressLine1 =
            unvalidatedAddress.AddressLine1
            |> createString50MapError "AddressLine1"

        let! addressLine2 =
            unvalidatedAddress.AddressLine2
            |> createString50OptionMapError "AddressLine2"

        let! addressLine3 =
            unvalidatedAddress.AddressLine3
            |> createString50OptionMapError "AddressLine3"

        let! addressLine4 =
            unvalidatedAddress.AddressLine4
            |> createString50OptionMapError "AddressLine4"

        let! city =
            unvalidatedAddress.City
            |> createString50MapError "City"

        let! zipCode =
            unvalidatedAddress.ZipCode
            |> ZipCode.create "ZipCode"
            |> Result.mapError ValidationError

        let address: Address =
            { AddressLine1 = addressLine1
              AddressLine2 = addressLine2
              AddressLine3 = addressLine3
              AddressLine4 = addressLine4
              City = city
              ZipCode = zipCode }

        return address
    }

let toOrderLineId lineId =
    lineId
    |> OrderLineId.create "OrderLineId"
    |> Result.mapError ValidationError

let toProductCode (checkProductCodeExists: CheckProductCodeExists) code =
    let checkProduct code =
        if checkProductCodeExists code then
            Ok code
        else
            let msg = sprintf "Invalid product code: %A" code
            Error(ValidationError msg)

    code
    |> ProductCode.create "ProductCode"
    |> Result.mapError ValidationError
    |> Result.bind checkProduct

let toOrderQuantity productCode quantity =
    OrderQuantity.create "OrderQuantity" productCode quantity
    |> Result.mapError ValidationError

let toValidateOrderLine checkProductExists (unvalidatedOrderLine: UnvalidatedOrderLine) =
    result {
        let l = unvalidatedOrderLine
        let! orderLineId = l.OrderLineId |> toOrderLineId

        let! productCode = l.ProductCode |> toProductCode checkProductExists

        let! quantity = l.Quantity |> toOrderQuantity productCode

        let validatedOrderLine: ValidatedOrderLine =
            { OrderLineId = orderLineId
              ProductCode = productCode
              Quantity = quantity }

        return validatedOrderLine
    }

let toResultOfList listR =
    let initialValue: Result<'a list, 'b> = Ok []

    let cons head tail = head :: tail

    // Alternative 1: Inelegant but most explicit
    let consR headR tailR =
        Result.apply (Result.map cons headR) tailR

    // Alternative 2: Classical monadic style
    let consR' headR tailR =
        let (<*>) = Result.apply
        let (<!>) = Result.map
        cons <!> headR <*> tailR

    // Alternative 3: FsToolkit.ErrorHandling
    let consR'' headR tailR = Result.map2 cons headR tailR

    List.foldBack consR'' listR initialValue

let toPricedOrderLine getProductPrice validatedOrderLine =
    result {
        let l = validatedOrderLine
        let qty = l.Quantity |> OrderQuantity.value
        let price = l.ProductCode |> getProductPrice

        let! linePrice =
            Price.multiply "LinePrice" qty price
            |> Result.mapError PricingError

        let pricedLine =
            { OrderLineId = l.OrderLineId
              ProductCode = l.ProductCode
              Quantity = l.Quantity
              LinePrice = linePrice }

        return pricedLine
    }


//==============================================================================
// Step: Validate Order
//==============================================================================

let validateOrder: ValidateOrder =
    fun checkProductCodeExists checkAddressExists unvalidatedOrder ->
        asyncResult {
            let! orderId = unvalidatedOrder.OrderId |> toOrderId

            let! customerInfo = unvalidatedOrder.CustomerInfo |> toCustomerInfo

            let! checkedShippingAddress =
                unvalidatedOrder.ShippingAddress
                |> toCheckedAddress checkAddressExists

            let! shippingAddress = checkedShippingAddress |> toAddress

            let! checkedBillingAddress =
                unvalidatedOrder.BillingAddress
                |> toCheckedAddress checkAddressExists

            let! billingAddress = checkedBillingAddress |> toAddress

            let! lines =
                unvalidatedOrder.Lines
                |> List.map (toValidateOrderLine checkProductCodeExists)
                |> toResultOfList

            let validatedOrder =
                { OrderId = orderId
                  CustomerInfo = customerInfo
                  ShippingAddress = shippingAddress
                  BillingAddress = billingAddress
                  Lines = lines }

            return validatedOrder
        }

//==============================================================================
// Step: Price Order
//==============================================================================

let priceOrder: PriceOrder =
    fun getProductPrice validatedOrder ->
        result {
            let o = validatedOrder

            let! lines =
                o.Lines
                |> List.map (toPricedOrderLine getProductPrice)
                |> toResultOfList

            let! amountToBill =
                lines
                |> List.map (fun l -> l.LinePrice)
                |> List.fold (fun sum p -> sum + (Price.value p)) 0M
                |> BillingAmount.create "AmountToBill"
                |> Result.mapError PricingError

            let pricedOrder =
                { OrderId = o.OrderId
                  CustomerInfo = o.CustomerInfo
                  ShippingAddress = o.ShippingAddress
                  BillingAddress = o.BillingAddress
                  AmountToBill = amountToBill
                  Lines = lines }

            return pricedOrder
        }

//==============================================================================
// Step: Acknowledge Order
//==============================================================================

let acknowledgeOrder: AcknowledgeOrder =
    fun createAcknowledgementLetter sendAcknowledgement pricedOrder ->
        let letter = createAcknowledgementLetter pricedOrder

        let acknowledgement =
            { EmailAddress = pricedOrder.CustomerInfo.EmailAddress
              Letter = letter }

        match sendAcknowledgement acknowledgement with
        | Sent ->
            let event =
                { OrderId = pricedOrder.OrderId
                  EmailAddress = pricedOrder.CustomerInfo.EmailAddress }

            Some event
        | NotSent -> None

//==============================================================================
// Events
//==============================================================================

let createOrderPlacedEvent (placedOrder: PricedOrder): PlacedOrder = placedOrder

let createBillingEvent (placedOrder: PricedOrder): BillableOrderPlaced option =
    let billingAmount =
        placedOrder.AmountToBill |> BillingAmount.value

    if billingAmount > 0M then
        { OrderId = placedOrder.OrderId
          BillingAddress = placedOrder.BillingAddress
          AmountToBill = placedOrder.AmountToBill }
        |> Some
    else
        None

let createEvents: CreateEvents =
    fun pricedOrder acknowledgementSentOpt ->
        let toList opt =
            match opt with
            | Some x -> [ x ]
            | None -> []

        let acknowledgementEvents =
            acknowledgementSentOpt
            |> Option.map OrderPlacedEvent.AcknowledgementSent
            |> toList

        let orderPlacedEvents =
            pricedOrder
            |> createOrderPlacedEvent
            |> OrderPlacedEvent.OrderPlaced
            |> List.singleton

        let billingEvents =
            pricedOrder
            |> createBillingEvent
            |> Option.map OrderPlacedEvent.BillableOrderPlaced
            |> toList

        [ yield! acknowledgementEvents
          yield! orderPlacedEvents
          yield! billingEvents ]

//==============================================================================
// End-To-End Workflow
//==============================================================================

let placeOrder checkProductCodeExists
               checkAddressExists
               getProductPrice
               createOrderAcknowledgementLetter
               sendOrderAcknowledgement
               : PlaceOrder =
    fun unvalidatedOrder ->
        let toAsyncResult (r: Result<'a, 'b>) = async { return r }

        asyncResult {
            let! validatedOrder =
                validateOrder checkProductCodeExists checkAddressExists unvalidatedOrder
                |> AsyncResult.mapError PlaceOrderError.Validation

            let! pricedOrder =
                priceOrder getProductPrice validatedOrder
                |> toAsyncResult
                |> AsyncResult.mapError PlaceOrderError.Pricing

            let acknowledgementOption =
                acknowledgeOrder createOrderAcknowledgementLetter sendOrderAcknowledgement pricedOrder

            let events =
                createEvents pricedOrder acknowledgementOption

            return events
        }

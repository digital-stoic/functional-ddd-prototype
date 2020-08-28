module OrderTaking.PlaceOrder.Types.Public

open OrderTaking.Common.Types.Simple
open OrderTaking.Common.Types.Compound

//==============================================================================
// Types exposed at the boundary of the Bounded Context
//==============================================================================

//==============================================================================
// Workflow input
//==============================================================================

type UnvalidatedCustomerInfo =
    { FirstName: string
      LastName: string
      EmailAddress: string }

type UnvalidatedAddress =
    { AddressLine1: string
      AddressLine2: string
      AddressLine3: string
      AddressLine4: string
      City: string
      ZipCode: string }

type UnvalidatedOrderLine =
    { OrderLineId: string
      ProductCode: string
      Quantity: decimal }

type UnvalidatedOrder =
    { OrderId: string
      CustomerInfo: UnvalidatedCustomerInfo
      ShippingAddress: UnvalidatedAddress
      BillingAddress: UnvalidatedAddress
      Lines: UnvalidatedOrderLine list }

//==============================================================================
// Workflow success output
//==============================================================================

type OrderAcknowledgementSent =
    { OrderId: OrderId
      EmailAddress: EmailAddress }

type PricedOrderLine =
    { OrderLineId: OrderLineId
      ProductCode: ProductCode
      Quantity: OrderQuantity
      LinePrice: Price }

type PricedOrder =
    { OrderId: OrderId
      CustomerInfo: CustomerInfo
      ShippingAddress: Address
      BillingAddress: Address
      AmountToBill: BillingAmount
      Lines: PricedOrderLine list }

type PlacedOrder = PricedOrder

type BillableOrderPlaced =
    { OrderId: OrderId
      BillingAddress: Address
      AmountToBill: BillingAmount }

type OrderPlacedEvent =
    | OrderPlaced of PlacedOrder
    | BillableOrderPlaced of BillableOrderPlaced
    | AcknowledgementSent of OrderAcknowledgementSent

//
// Workflow error output
//
type ValidationError = ValidationError of string

type PricingError = PricingError of string

type ServiceInfo = { Name: string; Endpoint: System.Uri }

type RemoteServiceError =
    { Service: ServiceInfo
      Exception: System.Exception }

type PlaceOrderError =
    | Validation of ValidationError
    | Pricing of PricingError
    | RemoteService of RemoteServiceError

//
// Workflow
//
// TODO: Use AsyncResult type alias?
type PlaceOrder = UnvalidatedOrder -> Async<Result<OrderPlacedEvent list, PlaceOrderError>>

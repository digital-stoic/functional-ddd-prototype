module Test.OrderTaking.Common.Stubs

open OrderTaking.Common.Types.Simple
open OrderTaking.PlaceOrder.Types.Public
open OrderTaking.PlaceOrder.Domain

//==============================================================================
// Test Stubs
//==============================================================================

let field = "test"

let checkProductCodeExistsOK code = true

let checkProductCodeExistsNOK code = false

let checkAddressExistsOK address =
    async { return Ok <| CheckedAddress address }

let checkAddressExistsNOK address = async { return Error AddressNotFound }

let getProductPriceOK productCode = Price.unsafeCreate field 42M

let createAcknowledgementLetterOK (pricedOrder: PricedOrder) =
    let amount =
        BillingAmount.value pricedOrder.AmountToBill

    let letter =
        sprintf "<h1>Acknowledgment</h1><p>Amount to bill:%f</p>" amount

    HtmlString letter

let sendAcknowledgementOK acknowledgement = Sent

let sendAcknowledgementNOK acknowledgement = NotSent

module Test.OrderTaking.PlaceOrder.Domain

open Expecto
open Expecto.Flip.Expect
open Test.Common.Helpers
open Test.OrderTaking.Common.Helpers
open Test.OrderTaking.Common.Stubs
open Test.OrderTaking.Common.Data
open OrderTaking.Common.Types.Simple
open OrderTaking.Common.Types.Compound
open OrderTaking.PlaceOrder.Types.Public
open OrderTaking.PlaceOrder.Domain

//==============================================================================
// Helpers
//==============================================================================

let testAsyncValidateOrderOK testName checkProductCodeExists checkAddressExists unvalidatedOrder expectedValidatedOrder =
    testAsync testName {
        let! order = async { return! (validateOrder checkProductCodeExists checkAddressExists unvalidatedOrder) }

        order
        |> equalR "ValidatedOrder created" expectedValidatedOrder
    }

let testAsyncValidateOrderNOK testName checkProductCodeExists checkAddressExists unvalidatedOrder =
    testAsync testName {
        let! order = async { return! (validateOrder checkProductCodeExists checkAddressExists unvalidatedOrder) }

        order |> isError "Failed to create ValidatedOrder"
    }

let testPriceOrderOK testName getProductPrice unvalidatedOrder expectedPricedOrder =
    test testName {
        priceOrder getProductPrice unvalidatedOrder
        |> equalR "PricedOrder created" expectedPricedOrder
    }

let testAcknowledgeOrderOK testName createAcknowledgementLetter sendAcknowledgement pricedOrder expectedAcknowledgement =
    test testName {
        let events =
            acknowledgeOrder createAcknowledgementLetter sendAcknowledgement pricedOrder

        events
        |> equal "Order acknowledged" expectedAcknowledgement
    }

let testCreateEventsOK testName pricedOrder acknowledgementOpt expectedEvents =
    test testName {
        let events =
            createEvents pricedOrder acknowledgementOpt

        events |> equal "Events Created" expectedEvents
    }

let testPlaceOrderOK testName
                     checkProductCodeExists
                     checkAddressExists
                     getProductPrice
                     createOrderAcknowledgementLetter
                     sendOrderAcknowledgement
                     unvalidatedOrder
                     expectedEvents
                     =
    testAsync testName {
        let! events =
            placeOrder
                checkProductCodeExists
                checkAddressExists
                getProductPrice
                createOrderAcknowledgementLetter
                sendOrderAcknowledgement
                unvalidatedOrder

        events |> equalR "OrderPlaced" expectedEvents
    }

let testPlaceOrderNOK testName
                      checkProductCodeExists
                      checkAddressExists
                      getProductPrice
                      createOrderAcknowledgementLetter
                      sendOrderAcknowledgement
                      unvalidatedOrder
                      =
    testAsync testName {
        let! res =
            placeOrder
                checkProductCodeExists
                checkAddressExists
                getProductPrice
                createOrderAcknowledgementLetter
                sendOrderAcknowledgement
                unvalidatedOrder

        res |> isError "OrderPlaced failed"
    }

//==============================================================================
// Tests
//==============================================================================

let testListValidateOrder =
    testList
        "ValidateOrder"
        [ testAsyncValidateOrderOK
            "Validate an unvalidated order with no order line"
              checkProductCodeExistsOK
              checkAddressExistsOK
              unvalidatedOrderNoLineOK1
              validatedOrderNoLineOK1

          testAsyncValidateOrderOK
              "Validate an unvalidated order with one order line"
              checkProductCodeExistsOK
              checkAddressExistsOK
              unvalidatedOrderOneLineOK1
              validatedOrderOneLineOK1

          testAsyncValidateOrderOK
              "Validate an unvalidated order with two order lines"
              checkProductCodeExistsOK
              checkAddressExistsOK
              unvalidatedOrderTwoLinesOK1
              validatedOrderTwoLinesOK1

          testAsyncValidateOrderNOK
              "Fail to validate an unvalidated order with inexisting product code"
              checkProductCodeExistsNOK
              checkAddressExistsOK
              unvalidatedOrderTwoLinesOK1

          testAsyncValidateOrderNOK
              "Fail to validate an unvalidated order with inexisting address"
              checkProductCodeExistsOK
              checkAddressExistsNOK
              unvalidatedOrderTwoLinesOK1

          testAsyncValidateOrderNOK
              "Fail to validate an unvalidated order with length OrderId > 50"
              checkProductCodeExistsOK
              checkAddressExistsOK
              unValidatedOrderOrderIdNOK1 ]

let testListPriceOrder =
    testList
        "PriceOrder"
        [ testPriceOrderOK
            "Price a validated order with no order line"
              getProductPriceOK
              validatedOrderNoLineOK1
              pricedOrderNoLineOK1

          testPriceOrderOK
              "Price a validated order with one order line"
              getProductPriceOK
              validatedOrderOneLineOK1
              pricedOrderOneLineOK1

          testPriceOrderOK
              "Price a validated order with two order lines"
              getProductPriceOK
              validatedOrderTwoLinesOK1
              pricedOrderTwoLinesOK1 ]

let testListAcknowledgeOrder =
    testList
        "AcknowledgeOrder"
        [ testAcknowledgeOrderOK
            "Acknowledge a priced order and send it"
              createAcknowledgementLetterOK
              sendAcknowledgementOK
              pricedOrderTwoLinesOK1
              orderAcknowledgementSentTwoLinesOK1

          testAcknowledgeOrderOK
              "Acknowledge a priced order and fail to send it"
              createAcknowledgementLetterOK
              sendAcknowledgementNOK
              pricedOrderTwoLinesOK1
              orderAcknowledgementSentNOK1 ]

let testListCreateEvents =
    testList
        "CreateEvents"
        [ testCreateEventsOK
            "Create events from a priced order and an acknowledgment"
              pricedOrderTwoLinesOK1
              orderAcknowledgementSentTwoLinesOK1
              eventsTwoLinesOK1

          testCreateEventsOK
              "Create events from a priced order without acknowledgment"
              pricedOrderTwoLinesOK1
              orderAcknowledgementSentNOK1
              eventsTwoLinesNoAcknowledgementSentOK1 ]

let testListPlaceOrder =
    testList
        "PlaceOrder"
        [ testPlaceOrderOK
            "Place order with two lines"
              checkProductCodeExistsOK
              checkAddressExistsOK
              getProductPriceOK
              createAcknowledgementLetterOK
              sendAcknowledgementOK
              unvalidatedOrderTwoLinesOK1
              eventsTwoLinesOK1

          testPlaceOrderOK
              "Place order with one line"
              checkProductCodeExistsOK
              checkAddressExistsOK
              getProductPriceOK
              createAcknowledgementLetterOK
              sendAcknowledgementOK
              unvalidatedOrderOneLineOK1
              eventsOneLineOK1

          testPlaceOrderOK
              "Place order with no line"
              checkProductCodeExistsOK
              checkAddressExistsOK
              getProductPriceOK
              createAcknowledgementLetterOK
              sendAcknowledgementOK
              unvalidatedOrderNoLineOK1
              eventsNoLineOK1

          testPlaceOrderOK
              "Place order without sending acknowledgement"
              checkProductCodeExistsOK
              checkAddressExistsOK
              getProductPriceOK
              createAcknowledgementLetterOK
              sendAcknowledgementNOK
              unvalidatedOrderTwoLinesOK1
              eventsTwoLinesNoAcknowledgementSentOK1

          testPlaceOrderNOK
              "Fail to place order with inexisting product code"
              checkProductCodeExistsNOK
              checkAddressExistsOK
              getProductPriceOK
              createAcknowledgementLetterOK
              sendAcknowledgementOK
              unvalidatedOrderTwoLinesOK1

          testPlaceOrderNOK
              "Fail to place order with inexisting address"
              checkProductCodeExistsOK
              checkAddressExistsNOK
              getProductPriceOK
              createAcknowledgementLetterOK
              sendAcknowledgementOK
              unvalidatedOrderTwoLinesOK1

          testPlaceOrderNOK
              "Fail to place order with invalid OrderId"
              checkProductCodeExistsOK
              checkAddressExistsNOK
              getProductPriceOK
              createAcknowledgementLetterOK
              sendAcknowledgementOK
              unValidatedOrderOrderIdNOK1 ]

let tests =
    testList "PlaceOrder.Domain"
    <| testListAppend [ testListValidateOrder
                        testListPriceOrder
                        testListAcknowledgeOrder
                        testListCreateEvents
                        testListPlaceOrder ]

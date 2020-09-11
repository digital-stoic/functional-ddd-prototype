module Test.OrderTaking.PlaceOrder.Dto

open Expecto
open Expecto.Flip.Expect
open Test.Common.Helpers
open Test.OrderTaking.Common.Helpers
open Test.OrderTaking.Common.Data
open OrderTaking.PlaceOrder.Dto

//==============================================================================
// Helpers
//==============================================================================

let testDeserOK equals testName deser obj1 expectedObj2 =
    test testName { deser obj1 |> equals "Deser" expectedObj2 }

let testDeserNOK testName deser obj1 =
    test testName { deser obj1 |> isError "Deser" }

let testDeserializeOK testName deser obj1 expectedObj2 =
    testDeserOK equal testName deser obj1 expectedObj2

let testDeserializeROK testName deser obj1 expectedObj2 =
    testDeserOK equalR testName deser obj1 expectedObj2

let testSerializeOK testName deser obj1 expectedObj2 =
    testDeserOK equal testName deser obj1 expectedObj2

let testDeserializeNOK = testDeserNOK

//==============================================================================
// Tests
//==============================================================================

// TODO: Complete other tests
let testListInputDto =
    testList
        "Input"
        [ testDeserializeOK
            "Deserialize DTO to unvalidated customer info"
              CustomerInfoDto.toUnvalidatedCustomerInfo
              customerInfoDtoOK1
              unvalidatedCustomerInfoOK1

          testDeserializeROK
              "Deserialize DTO to validated customer info"
              CustomerInfoDto.toCustomerInfo
              customerInfoDtoOK1
              customerInfoOK1

          testDeserializeNOK
              "Fail to deserialize DTO to validated customer info with empty first name"
              CustomerInfoDto.toCustomerInfo
              customerInfoDtoEmptyFirstNameNOK1

          testDeserializeNOK
              "Fail to deserialize DTO to validated customer info with null first name"
              CustomerInfoDto.toCustomerInfo
              customerInfoDtoNullFirstNameNOK1

          testDeserializeNOK
              "Fail to deserialize DTO to validated customer info with first name > 50"
              CustomerInfoDto.toCustomerInfo
              customerInfoDtoLonger50FirstNameNOK1

          testDeserializeOK
              "Deserialize DTO to unvalidated address"
              AddressDto.toUnvalidatedAddress
              addressDtoOK1
              unvalidatedAddressOK1

          testDeserializeROK "Deserialize DTO to validated address" AddressDto.toAddress addressDtoOK1 addressOK1

          testDeserializeOK
              "Deserialize DTO to unvalidated order line"
              OrderLineDto.toUnvalidatedOrderLine
              orderLineDtoOK1
              unvalidatedOrderLineOK1

          testDeserializeOK
              "Deserialize DTO to unvalidated order without line"
              OrderDto.toUnvalidatedOrder
              orderNoLineDtoOK1
              unvalidatedOrderNoLineOK1

          testDeserializeOK
              "Deserialize DTO to unvalidated order with one line"
              OrderDto.toUnvalidatedOrder
              orderOneLineDtoOK1
              unvalidatedOrderOneLineOK1

          testDeserializeOK
              "Deserialize DTO to unvalidated order with two lines"
              OrderDto.toUnvalidatedOrder
              orderTwoLinesDtoOK1
              unvalidatedOrderTwoLinesOK1

          test "dummy" { isTrue "dummy" true } ]

let testListOutputDto =
    testList
        "Output"
        [ testSerializeOK "Serialize customer info to DTO" CustomerInfoDto.fromDomain customerInfoOK1 customerInfoDtoOK1

          testSerializeOK "Serialize address to DTO" AddressDto.fromDomain addressOK1 addressDtoOK1

          testSerializeOK
              "Serialize priced order line to DTO"
              PricedOrderLineDto.fromDomain
              pricedOrderLineOK1
              pricedOrderLineDtoOK1

          testSerializeOK
              "Serialize order placed with no line to DTO"
              OrderPlacedEventDto.fromDomain
              orderPlacedNoLineOK1
              orderPlacedNoLineDtoOK1

          testSerializeOK
              "Serialize order placed with one line to DTO"
              OrderPlacedEventDto.fromDomain
              orderPlacedOneLineOK1
              orderPlacedOneLineDtoOK1

          testSerializeOK
              "Serialize order placed with two lines to DTO"
              OrderPlacedEventDto.fromDomain
              orderPlacedTwoLinesOK1
              orderPlacedTwoLinesDtoOK1

          testSerializeOK
              "Serialize order placed validation error to DTO"
              PlaceOrderErrorDto.fromDomain
              placedOrderErrorValidationOK1
              placedOrderErrorDtoValidationOK1

          testSerializeOK
              "Serialize order placed pricing error to DTO"
              PlaceOrderErrorDto.fromDomain
              placedOrderErrorPricingOK1
              placedOrderErrorDtoPricingOK1

          testSerializeOK
              "Serialize order placed remote service error to DTO"
              PlaceOrderErrorDto.fromDomain
              placedOrderErrorRemoteServiceOK1
              placedOrderErrorDtoRemoteServiceOK1

          test "dummy" { isTrue "dummy" true } ]


let tests =
    testList "PlaceOrder.Dto"
    <| testListAppend [ testListInputDto
                        testListOutputDto ]

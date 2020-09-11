module Test.OrderTaking.Common.Types.Simple

open Expecto
open Expecto.Flip.Expect
open Test.Common.Helpers
open Test.OrderTaking.Common.Helpers
open Test.OrderTaking.Common.Stubs
open Test.OrderTaking.Common.Data
open OrderTaking.Common.Types.Simple

//==============================================================================
// Helpers
//==============================================================================

let _testSimpleTypeOK equals testName create value x =
    test testName {
        create "test" x
        |> equals value "Simple type created" x
    }

let testSimpleTypeNOK testName create x =
    test testName {
        create "test" x
        |> isError "Simple type creation failed"
    }

let testSimpleTypeOK testName create value x =
    _testSimpleTypeOK equalRV testName create value x

let testSimpleTypeOptionOK testName create x =
    _testSimpleTypeOK equalRVO testName create x

//==============================================================================
// Tests
//==============================================================================

let testListString50 =
    testList
        "String50"
        [ testSimpleTypeOK "Create a String50 of length < 50" String50.create String50.value string50OK1

          testSimpleTypeNOK "Fail to create a String50 of length > 50" String50.create stringLonger50NOK1

          testSimpleTypeNOK "Fail to create a null String50" String50.create null

          testSimpleTypeOptionOK
              "Create an optional String50 of length < 50"
              String50.createOption
              String50.value
              string50OK1

          testSimpleTypeOptionOK "Create a null optional String50" String50.createOption String50.value null

          testSimpleTypeNOK
              "Fail to create an optional String50 of length > 50"
              String50.createOption
              stringLonger50NOK1 ]

let testListEmailAddress =
    testList
        "EmailAddress"
        [ testSimpleTypeOK "Create an EmailAddress" EmailAddress.create EmailAddress.value emailStrOK1

          testSimpleTypeNOK "Fail to create an EmailAddress without '@'" EmailAddress.create emailStrNOK1 ]

let testListZipCode =
    testList
        "ZipCode"
        [ testSimpleTypeOK "Create a ZipCode" ZipCode.create ZipCode.value zipCodeStrOK1

          testSimpleTypeNOK
              "Fail to create a ZipCode with wrong number of digits"
              ZipCode.create
              zipCodeNotEnoughDigitsStrNOK1

          testSimpleTypeNOK "Fail to create a ZipCode with non-digits" ZipCode.create zipCodeNotOnlyDigitsStrNOK1 ]

let testListOrderid =
    testList
        "OrderId"
        [ testSimpleTypeOK "Create an OrderId of length < 50" OrderId.create OrderId.value orderIdStrOK1

          testSimpleTypeNOK "Fail to create a OrderId of length > 50" OrderId.create orderIdStrNOK1

          testSimpleTypeNOK "Fail to create a null OrderId" OrderId.create null ]

let testListOrderLineId =
    testList
        "OrderLineId"
        [ testSimpleTypeOK "Create an OrderLineId of length < 50" OrderId.create OrderId.value orderLineIdStrOK1

          testSimpleTypeNOK "Fail to create a OrderLineId of length > 50" OrderLineId.create orderLineIdStrNOK1 ]

let testListProductCode =
    testList
        "ProductCode"
        [ testSimpleTypeOK "Create a WidgetCode" WidgetCode.create WidgetCode.value productCodeWidgetStrOK1

          testSimpleTypeNOK "Fail to create an invalid WidgetCode" WidgetCode.create productCodeWidgetStrNOK1

          testSimpleTypeNOK "Fail to create a null WidgetCode" WidgetCode.create null

          testSimpleTypeOK "Create a GizmoCode" GizmoCode.create GizmoCode.value productCodeGizmoStrOK1

          testSimpleTypeNOK "Fail to create an invalid GizmoCode" GizmoCode.create productCodeGizmoStrNOK1

          testSimpleTypeNOK "Fail to create a null GizmoCode" GizmoCode.create null

          testSimpleTypeOK
              "Create a ProductCode WidgetCode"
              ProductCode.create
              ProductCode.value
              productCodeWidgetStrOK1

          testSimpleTypeOK "Create a ProductCode GizmoCode" ProductCode.create ProductCode.value productCodeGizmoStrOK1

          testSimpleTypeNOK "Fail to create an invalid ProductCode" ProductCode.create productCodeStrNOK1

          testSimpleTypeNOK "Fail to create a null ProductCode" ProductCode.create null ]

let testListOrderQuantity =
    testList
        "OrderQuantity"
        [ testSimpleTypeOK "Create a UnitQuantity" UnitQuantity.create UnitQuantity.value unitQuantityIntOK1

          testSimpleTypeNOK "Fail to create a UnitQuantity < 1" UnitQuantity.create unitQuantityLess1IntNOK1

          testSimpleTypeNOK "Fail to create a UnitQuantity > 1000" UnitQuantity.create unitQuantityLarger1000IntNOK1

          testSimpleTypeOK
              "Create a KilogramQuantity"
              KilogramQuantity.create
              KilogramQuantity.value
              kilogramQuantityDecOK1

          testSimpleTypeNOK
              "Fail to create a KilogramQuantity < 0.5"
              KilogramQuantity.create
              kilogramQuantityLess05DecNOK1

          testSimpleTypeNOK
              "Fail to create a KilogramQuantity > 100"
              KilogramQuantity.create
              kilogramQuantityLarger100DecNOK1

          testSimpleTypeOK "Create a Unit OrderQuantity from a WidgetCode" (fun s q ->
              OrderQuantity.create field productCodeWidgetOK1 q) OrderQuantity.value orderQuantityDecOK1

          testSimpleTypeOK "Create a Kilogram OrderQuantity from a GizmoCode" (fun s q ->
              OrderQuantity.create field productCodeGizmoOK1 q) OrderQuantity.value orderQuantityDecOK2

          testSimpleTypeNOK "Fail to create a Unit OrderQuantity > 1000 from a WidgetCode" (fun s q ->
              OrderQuantity.create field productCodeWidgetOK1 q) orderQuantityLarger1000DecNOK1

          testSimpleTypeNOK "Fail to create a Kilogram OrderQuantity < 0.5 from a GizmoCode" (fun s q ->
              OrderQuantity.create field productCodeGizmoOK1 q) orderQuantityLarger1000DecNOK1 ]

let testListPrice =
    testList
        "Price"
        [ testSimpleTypeOK "Create Price" Price.create Price.value priceDecOK1

          testSimpleTypeNOK "Fail to create Price < 0" Price.create priceLess0NOK1

          testSimpleTypeNOK "Fail to create Price > 1000" Price.create priceLarger1000NOK1 ]

let testListBillingAmount =
    testList
        "BillingAmount"
        [ testSimpleTypeOK "Create BillingAmount" BillingAmount.create BillingAmount.value billingAmountDecOK1

          testSimpleTypeNOK "Fail to create BillingAmount < 0" BillingAmount.create billingAmountLess0DecNOK1

          testSimpleTypeNOK "Fail to create BillingAmount > 10000" BillingAmount.create billingAmoungLarger10000DecNOK1 ]

let tests =
    testList "Common.Types.Simple"
    <| testListAppend [ testListString50
                        testListEmailAddress
                        testListZipCode
                        testListOrderid
                        testListOrderLineId
                        testListProductCode
                        testListOrderQuantity
                        testListPrice
                        testListBillingAmount ]

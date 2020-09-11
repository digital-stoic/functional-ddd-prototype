module Test.Common.Helpers

// Append test lists
let testListAppend tests =
    List.fold (fun testList test -> List.append testList [ test ]) [] tests

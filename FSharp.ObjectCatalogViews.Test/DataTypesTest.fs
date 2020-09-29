namespace FSharp.ObjectCatalogViews.Test

open Xunit
open Xunit.Abstractions

open FSharp.xUnit
open FSharp.ObjectCatalogViews

type DataTypesTest(output: ITestOutputHelper) =
    [<Fact>]
    member this.``convert from sql to targets``() =
        let type_name = "int"
        let is_nullable = true

        let pt = DataTypes.fsharpPrimitiveType type_name
        let tp = DataTypes.fsharpType is_nullable type_name
        let rd = DataTypes.readerMethodName type_name
                
        Should.equal pt "int"
        Should.equal tp "Nullable<int>"
        Should.equal rd "GetInt32"


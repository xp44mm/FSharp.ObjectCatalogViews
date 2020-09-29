namespace FSharp.ObjectCatalogViews.Test

open Xunit
open Xunit.Abstractions
open FSharp.ObjectCatalogViews


type OcvGeneratorTest(output: ITestOutputHelper) =
    [<Fact>]
    member this.``generate Ocv Test``() =
        let db_name = "Cuisl"
        let code = OcvGenerator.ocvDefinition ConnectionString.connstr db_name
        output.WriteLine(code)
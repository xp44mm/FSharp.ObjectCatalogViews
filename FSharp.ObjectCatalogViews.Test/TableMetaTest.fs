namespace FSharp.ObjectCatalogViews.Test

open Xunit
open Xunit.Abstractions

open System
open System.IO
open FSharp.ObjectCatalogViews

type TableMetaTest(output: ITestOutputHelper) =
    [<Fact>]
    member this.``get Structural Schemas``() =
        let db_name = "Lake"
        let data = TableMeta.getStructuralSchemas ConnectionString.connstr db_name
        output.WriteLine(FSharp.Literals.Render.stringify data)

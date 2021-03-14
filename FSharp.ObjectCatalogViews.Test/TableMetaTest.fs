namespace FSharp.ObjectCatalogViews

open Xunit
open Xunit.Abstractions

open System
open System.IO

open FSharp.Literals

type TableMetaTest(output: ITestOutputHelper) =
    [<Fact>]
    member this.``get Structural Schemas``() =
        let db_name = "Lake"
        let schemas = TableMeta.getStructuralSchemas ConnectionString.connstr db_name
        output.WriteLine(FSharp.Literals.Render.stringify schemas)

    [<Fact>]
    member this.``read table``() =
        let db_name = "Lake"
        let lake = TableMeta.getCatalog ConnectionString.connstr db_name
        let table = lake.["ShapeSteel"].["工字钢"]

        output.WriteLine(Render.stringify table)

    [<Fact>]
    member this.``read table data``() =
        let db_name = "Lake"
        let lake = TableMeta.getCatalog ConnectionString.connstr db_name
        let table = lake.["ShapeSteel"].["工字钢"]
        let data = TableMeta.readTable ConnectionString.connstr db_name "ShapeSteel" table
        output.WriteLine(Render.stringify data)


namespace FSharp.ObjectCatalogViews.Test

open Xunit
open Xunit.Abstractions

open System
open System.IO
open System.Reflection
open FSharp.ObjectCatalogViews

type SchemaValidTest(output: ITestOutputHelper) =
    [<Fact>]
    member this.``valid schema Test``() =
        let db_name = "Lake"
        let schemas = TableMeta.getStructuralSchemas ConnectionString.connstr db_name

        let ass = Assembly.LoadFile(
                    Path.Combine(ConnectionString.outputPath, @"bin\Release\netstandard2.0\Databases.dll"))
        schemas
        |> Array.iter(fun sch ->
            sch.tables
            |> Array.iter(fun table ->
                let dbColumns = 
                    table.columns
                    |> Array.map(fun col -> col.name, SchemaValid.buildType col.is_nullable col.type_name)
                let fileColumns = SchemaValid.getRecordFields ass db_name sch.name table.name
                FSharp.xUnit.Should.equal dbColumns fileColumns
            )
        )




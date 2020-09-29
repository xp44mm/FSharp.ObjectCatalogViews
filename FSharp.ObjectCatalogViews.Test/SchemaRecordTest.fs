namespace FSharp.ObjectCatalogViews.Test

open Xunit
open Xunit.Abstractions

open FSharp.ObjectCatalogViews

type SchemaRecordTest(output: ITestOutputHelper) =

    [<Fact>]
    member this.``SchemaRecord databaseDefinition Test``() =
        let db_name = "Lake"
        let code = ReadOnlyRecord.databaseDefinition ConnectionString.connstr db_name
        output.WriteLine(code)

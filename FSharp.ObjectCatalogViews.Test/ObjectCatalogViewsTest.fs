namespace FSharp.ObjectCatalogViews.Test

open Xunit
open Xunit.Abstractions
open FSharp.ObjectCatalogViews

type ObjectCatalogViewsTest(output: ITestOutputHelper) =

    [<Fact>]
    member this.``get user table and view``() =
        let db_name = "Lake"

        let data = Database(ConnectionString.connstr, db_name)
        output.WriteLine(FSharp.Literals.Render.stringify data.Columns)


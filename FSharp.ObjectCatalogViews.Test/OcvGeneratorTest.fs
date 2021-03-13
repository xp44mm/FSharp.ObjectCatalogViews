namespace FSharp.ObjectCatalogViews

open Xunit
open Xunit.Abstractions
open FSharp.ObjectCatalogViews
open System.IO

type OcvGeneratorTest(output: ITestOutputHelper) =

    [<Fact(Skip="done!")>]
    member this.``generate Ocv Test``() =
        let db_name = "Cuisl"
        let code = OcvGenerator.ocvDefinition ConnectionString.connstr db_name
    
        let path = Path.Combine(ConnectionString.LakeOutputPath, "ObjectCatalogViews.fs")
        File.WriteAllText(path, code)
        output.WriteLine("ocv done!")
    
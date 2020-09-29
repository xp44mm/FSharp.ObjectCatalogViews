namespace FSharp.ObjectCatalogViews.Test

open Xunit
open Xunit.Abstractions

open System
open System.IO
open System.Reflection
open FSharp.ObjectCatalogViews

type DataValidTest(output: ITestOutputHelper) =
    [<Fact>]
    member this.``valid data array Test``() =
        let db_name = "Lake"
        let schemas = TableMeta.getStructuralSchemas ConnectionString.connstr db_name


        let ass = Assembly.LoadFile(
                    Path.Combine(ConnectionString.outputPath, @"bin\Release\netstandard2.0\Databases.dll"))

        schemas
        |> Array.iter(fun sch ->
            sch.tables
            |> Array.iter(fun table ->
                let dbData = DataValid.getDataFromDatabase ConnectionString.connstr db_name sch.name table
                let fileData = DataValid.getDataFromReflection ass db_name sch.name table.name

                Array.zip dbData fileData
                |> Array.iter(fun (a,b) -> 
                    Array.zip a b
                    |> Array.iter(fun ((n1,is_nullable,type_name,dbv),(n2,tp,flv)) -> 
                        if n1 <> n2 then
                            output.WriteLine(sprintf "%s != %s" n1 n2)

                        if DataValid.equals is_nullable type_name dbv flv then
                            ()
                        else
                            output.WriteLine(sprintf "%s.%s.%s not equal" sch.name table.name n1)
                    )
                    )

            )
        )

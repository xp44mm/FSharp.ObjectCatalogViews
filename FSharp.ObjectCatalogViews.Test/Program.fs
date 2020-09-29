module FSharp.ObjectCatalogViews.Program

open System
open System.IO

let ocv () =
    let db_name = "Cuisl"
    let code = OcvGenerator.ocvDefinition ConnectionString.connstr db_name

    let path = Path.Combine(ConnectionString.outputPath, "ObjectCatalogViews.fs")
    File.WriteAllText(path, code)
    Console.WriteLine("ocv done!")

let lake () =
    let db_name = "Lake"
    let code = ReadOnlyRecord.databaseDefinition ConnectionString.connstr db_name

    let path = Path.Combine(ConnectionString.outputPath, sprintf @"%s.fs" db_name)
    File.WriteAllText(path, code)
    Console.WriteLine("lake done!")

let [<EntryPoint>] main _ = 
    lake()
    0

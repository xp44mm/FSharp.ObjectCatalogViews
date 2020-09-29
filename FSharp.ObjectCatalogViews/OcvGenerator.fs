module FSharp.ObjectCatalogViews.OcvGenerator

open TableMeta
open FSharp.Literals

let ( ** ) str i = String.replicate i str
let space4 = " " ** 4

let readFieldValue (col:Column) =
    let ftype = DataTypes.fsharpPrimitiveType col.type_name
    if col.is_nullable && DataTypes.fsValueTypes.Contains(ftype) then 
        sprintf 
            "match reader.[%s] with :? DBNull -> Nullable() | v -> Nullable(unbox<%s> v)"
            (Render.stringify col.name) ftype
    elif col.is_nullable then 
        sprintf 
            "match reader.[%s] with :? DBNull -> null | v -> unbox<%s> v" 
            (Render.stringify col.name) ftype
    else 
        sprintf "unbox<%s> reader.[%s]" ftype (Render.stringify col.name)

///Object Catalog Views Class
let databaseRecord (table:Table) =

    let columnNames = 
        table.columns 
        |> Array.map(fun col -> "[" + col.name + "]") 
        |> String.concat ", "

    [|
        yield! ReadOnlyRecord.recordDefinition table
        yield space4 + "static member GetArray (connstr) ="
        yield space4 ** 2 + sprintf "let sql = \"SELECT %s FROM master.sys.%s\"" columnNames table.name
        yield space4 ** 2 + "[|"
        yield space4 ** 3 + "use conn = new SqlConnection(connstr)"
        yield space4 ** 3 + "do conn.Open()                       "
        yield space4 ** 3 + "use comm = new SqlCommand(sql, conn) "
        yield space4 ** 3 + "use reader = comm.ExecuteReader()    "
        yield space4 ** 3 + "while reader.Read() do"
        yield space4 ** 4 + "yield {"
        for col in table.columns do
            yield space4 ** 5 + col.name + " = " + readFieldValue col
        yield space4 ** 4 + "}"
        yield space4 ** 2 + "|]"
    |]

let ocvRecord (table:Table) =
    let columnNames = 
        table.columns 
        |> Array.map(fun col -> "[" + col.name + "]") 
        |> String.concat ", "

    [|
        yield! ReadOnlyRecord.recordDefinition table
        yield space4 + "static member GetArray(connstr) ="
        yield space4 ** 2 + sprintf "let sql = sprintf \"SELECT %s FROM sys.%s\"" columnNames table.name
        yield space4 ** 2 + "[|"
        yield space4 ** 3 + "use conn = new SqlConnection(connstr)"
        yield space4 ** 3 + "do conn.Open()                       "
        yield space4 ** 3 + "use comm = new SqlCommand(sql, conn) "
        yield space4 ** 3 + "use reader = comm.ExecuteReader()    "
        yield space4 ** 3 + "while reader.Read() do"
        yield space4 ** 4 + "yield {"
        for col in table.columns do
            yield space4 ** 5 + col.name + " = " + readFieldValue col
        yield space4 ** 4 + "}"
        yield space4 ** 2 + "|]"

    |]

let ocvDefinition connstr db_name =
    let schemas = TableMeta.getStructuralSchemas connstr db_name

    let databseTable = 
        schemas
        |> Array.find(fun sc -> sc.name = "cui")
        |> fun cui -> cui.tables.[0]

    let cslSchema = 
        schemas
        |> Array.find(fun sc -> sc.name = "csl")

            
    [|
        yield "namespace ObjectCatalogViews"
        yield "open System"
        yield "open System.Data.SqlClient"

        yield! databaseRecord databseTable

        for table in cslSchema.tables do
            yield! ocvRecord table

    |]
    |> String.concat System.Environment.NewLine


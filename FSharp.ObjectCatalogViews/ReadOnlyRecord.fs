/// 用记录定义数据库模式
module FSharp.ObjectCatalogViews.ReadOnlyRecord

open System
open FSharp.Literals
open FSharp.Idioms.StringOps

let space4 = " " ** 4

let fsDataTypeDefinition (col: TableMeta.Column) = 
    let ty = SqlTypeUtils.getType col.is_nullable col.type_name
    Render.printTypeObj ty

let fieldDefinition (col: TableMeta.Column) =     
    let datatype = fsDataTypeDefinition col
    sprintf "%s: %s;" col.name datatype

/// 记录定义行
let recordDefinition (tbl: TableMeta.Table) = 
    [|
        yield sprintf "type %s = " tbl.name
        yield space4 + "{"
        for col in tbl.columns do
            yield space4 ** 2 + fieldDefinition col
        yield space4 + "}"
    |]

let renderRecords connstr db_name schema_name (table:TableMeta.Table) =
    let tableData = TableMeta.readTable connstr db_name schema_name table

    let outp = 
        tableData
        |> Array.mapi(fun r row ->
            row
            |> Array.mapi(fun c (name,ty,value) ->
                sprintf "%s = %s" name (Render.stringifyObj ty value)
            )
            |> String.concat ";"
            |> sprintf "{%s};"
        )
    outp

let renderDataRecordsProperty connstr db_name schema_name (table:TableMeta.Table) =
    [|
        yield "[|"
        yield! renderRecords connstr db_name schema_name table |> Array.map(fun ln -> space4 + ln)
        yield "|]"
    |]

let renderRecord connstr db_name schema_name (table:TableMeta.Table) =
    [|
        yield! recordDefinition table
        yield space4 + "static member DataRecords ="
        yield! renderDataRecordsProperty connstr db_name schema_name table |> Array.map(fun ln -> space4 ** 2 + ln)
    |]

let schemacode connstr db_name (sc:TableMeta.Schema) =
    [|
        yield sprintf "namespace %s.%s" db_name sc.name
        yield "open System"
        yield ""
        for table in sc.tables do
            yield! renderRecord connstr db_name sc.name table
    |]

/// 生成Lake数据库的代码，连接字符串不要带初始目录
let databaseDefinition connstr db_name =
    let schemas = TableMeta.getStructuralSchemas connstr db_name
            
    [|
        yield "//本代码由SchemaRecord.databaseDefinition自动生成"
        for sc in schemas do
            if Array.isEmpty sc.tables then
                ()
            else yield! schemacode connstr db_name sc
    |]
    |> String.concat Environment.NewLine



/// 用记录定义数据库模式
module FSharp.ObjectCatalogViews.ReadOnlyRecord

open System
open System.Data.SqlClient

open TableMeta

let ( ** ) str i = String.replicate i str
let space4 = " " ** 4

let fsDataTypeDefinition (col: Column) = 
    let ftype = DataTypes.fsharpPrimitiveType col.type_name

    if col.is_nullable && DataTypes.fsValueTypes.Contains(ftype) then 
        //可空值类型
        sprintf "Nullable<%s>" ftype
    else 
        //不可空值类型或者引用类型
        ftype

let fieldDefinition (col: Column) =     
    let datatype = fsDataTypeDefinition col
    sprintf "%s: %s;" col.name datatype

/// 记录定义行
let recordDefinition (tbl: Table) = 
    [|
        yield sprintf "type %s = " tbl.name
        yield space4 + "{"
        for col in tbl.columns do
            yield space4 ** 2 + fieldDefinition col
        yield space4 + "}"
    |]

///sql reader讀出的值打印成fsharp data
let renderSql is_nullable type_name (value:obj) =
    let primitiveType = DataTypes.fsharpPrimitiveType type_name
    if is_nullable && DataTypes.fsValueTypes.Contains primitiveType then
        //可空值类型
        match value with :? DBNull -> "Nullable()" | _ -> "Nullable " + FSharp.Literals.Render.stringify value
    elif is_nullable then
        match value with :? DBNull -> "null" | _ ->(FSharp.Literals.Render.stringify value)
    else
        FSharp.Literals.Render.stringify value

let renderRecords connstr db_name schema_name (table:Table) =
    let connstr = 
        [
            connstr
            "Initial Catalog=" + db_name
        ] |> String.concat ";"

    let sql = 
        let fields = table.columns |> Array.map(fun col -> "[" + col.name + "]") |> String.concat ", "
        sprintf "SELECT %s FROM %s.%s.%s" fields db_name schema_name table.name
    [|  
        use conn = new SqlConnection(connstr)
        do conn.Open()
        use comm = new SqlCommand(sql, conn)
        use reader = comm.ExecuteReader()
        while reader.Read() do
            yield [|
                for col in table.columns do
                    let literal = renderSql col.is_nullable col.type_name (reader.[col.name])
                    yield sprintf "%s = %s;" col.name literal
            |]
            |> String.concat " "
            |> sprintf "{ %s };"
    |]

let renderDataRecordsProperty connstr db_name schema_name (table:Table) =
    [|
        yield "[|"
        yield! renderRecords connstr db_name schema_name table |> Array.map(fun ln -> space4 + ln)
        yield "|]"
    |]

let renderRecord connstr db_name schema_name (table:Table) =
    [|
        yield! recordDefinition table
        yield space4 + "static member DataRecords ="
        yield! renderDataRecordsProperty connstr db_name schema_name table |> Array.map(fun ln -> space4 ** 2 + ln)
    |]

let schemacode connstr db_name (sc:Schema) =
    [|
        yield sprintf "namespace %s.%s" db_name sc.name
        yield "open System"
        yield ""
        for table in sc.tables do
            yield! renderRecord connstr db_name sc.name table
    |]

/// 生成Lake数据库的代码，连接字符串不要带初始目录
let databaseDefinition connstr db_name =
    let schemas = getStructuralSchemas connstr db_name
            
    [|
        yield "//本代码由SchemaRecord.databaseDefinition自动生成"
        for sc in schemas do
            if Array.isEmpty sc.tables |> not then
                yield! schemacode connstr db_name sc
    |]
    |> String.concat System.Environment.NewLine



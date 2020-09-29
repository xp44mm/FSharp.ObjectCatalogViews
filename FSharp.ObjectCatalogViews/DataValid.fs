module FSharp.ObjectCatalogViews.DataValid

open TableMeta
open System.Data.SqlClient
open System.Reflection
open System

/// 从数据库中获得表的数据，连接字符串不要带初始目录
let getDataFromDatabase connstr db_name schema_name (table:Table) =
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
                    yield (col.name,col.is_nullable,col.type_name,reader.[col.name])
            |]
    |]

///用反射从FSharp源文件中获得表的数据
let getDataFromReflection (ass:Assembly) db_name schema_name table_name =
    let fullName = sprintf "%s.%s.%s" db_name schema_name table_name
    let tp = ass.GetType(fullName)
    let pi = tp.GetProperty("DataRecords",BindingFlags.Static ||| BindingFlags.Public)
    let dr = pi.GetValue(null)

    let array = FSharp.Literals.Readers.arrayReader (typeof<obj[]>) dr

    let records = 
        array
        |> Array.map(fun record -> FSharp.Literals.Readers.recordReader (record.GetType()) record)

    records

let unboxequal<'a when 'a: equality> (x:obj) (y:obj) =
    let x = unbox<'a> x
    let y = unbox<'a> y
    x = y

let nullableEqual<'a when 'a: equality> (db:obj) (fl:obj) =
    if db :? DBNull && isNull fl then
        true
    elif db :? DBNull || isNull fl then
        false
    else
        unboxequal<'a> db fl

let equal<'a when 'a: equality> is_nullable (db:obj) (fl:obj) =
    if is_nullable then
        nullableEqual<'a> db fl
    else
        unboxequal<'a> db fl

/// 比較兩個值是否相等，一個從數據庫來，一個從文件來，并且給定了類型信息，用字符串
let equals is_nullable type_name (db:obj) (fl:obj) =
    let primitiveType = DataTypes.fsharpPrimitiveType type_name
    match primitiveType with
    |"bool" -> equal<bool> is_nullable db fl
    |"byte" -> equal<byte> is_nullable db fl
    |"decimal"          -> equal<decimal> is_nullable db fl
    |"float"            -> equal<float> is_nullable db fl
    |"float32"          -> equal<float32> is_nullable db fl
    |"int"              -> equal<int> is_nullable db fl
    |"int16"            -> equal<int16> is_nullable db fl
    |"int64"            -> equal<int64> is_nullable db fl
    |"DateTime"         -> equal<DateTime> is_nullable db fl
    |"DateTimeOffset"   -> equal<DateTimeOffset> is_nullable db fl
    |"TimeSpan"         -> equal<TimeSpan> is_nullable db fl
    |"Guid"             -> equal<Guid> is_nullable db fl

    |"byte[]" -> equal<byte[]> is_nullable db fl       
    |"string" -> equal<string> is_nullable db fl       
    |"obj"    -> equal<obj> is_nullable db fl       
    | _ -> failwithf "unexpected type: %s" primitiveType


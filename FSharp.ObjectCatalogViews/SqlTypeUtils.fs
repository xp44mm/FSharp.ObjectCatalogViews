module FSharp.ObjectCatalogViews.SqlTypeUtils

open System
open System.Collections.Generic

/// 获取不可空时的底层类型
let getUnderlyingType type_name =
    match type_name with
    | "bigint"          -> typeof<int64>
    | "binary"          -> typeof<byte[]>
    | "bit"             -> typeof<bool>
    | "char"            -> typeof<string>
    | "date"            -> typeof<DateTime>
    | "datetime"        -> typeof<DateTime>
    | "datetime2"       -> typeof<DateTime>
    | "datetimeoffset"  -> typeof<DateTimeOffset>
    | "decimal"         -> typeof<decimal>
    | "float"           -> typeof<float>
    | "image"           -> typeof<byte[]>
    | "int"             -> typeof<int>
    | "money"           -> typeof<decimal>
    | "nchar"           -> typeof<string>
    | "ntext"           -> typeof<string>
    | "numeric"         -> typeof<decimal>
    | "nvarchar"        -> typeof<string>
    | "real"            -> typeof<float32>
    | "rowversion"      -> typeof<byte[]>
    | "smalldatetime"   -> typeof<DateTime>
    | "smallint"        -> typeof<int16>
    | "smallmoney"      -> typeof<decimal>
    | "sql_variant"     -> typeof<obj>
    | "text"            -> typeof<string>
    | "time"            -> typeof<TimeSpan>
    | "timestamp"       -> typeof<byte[]>
    | "tinyint"         -> typeof<byte>
    | "uniqueidentifier"-> typeof<Guid>
    | "varbinary"       -> typeof<byte[]>
    | "varchar"         -> typeof<string>
    | "xml" | _         -> null

//值类型
let ValueTypes = HashSet [|
    typeof<bool>
    typeof<byte>
    typeof<DateTime>
    typeof<DateTimeOffset>
    typeof<Guid>
    typeof<float>
    typeof<float32>
    typeof<int16>
    typeof<int>
    typeof<int64>
    typeof<decimal>
    typeof<TimeSpan>
    |]

/// 获取.net类型
let getType is_nullable type_name =
    let ty = getUnderlyingType type_name
    if is_nullable && ValueTypes.Contains ty then
        typedefof<Nullable<_>>.MakeGenericType(ty)
    else ty


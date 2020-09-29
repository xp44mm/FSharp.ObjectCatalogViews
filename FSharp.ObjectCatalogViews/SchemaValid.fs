module FSharp.ObjectCatalogViews.SchemaValid

open System
open System.Reflection

/// 从类型名获得类型
let buildPrimitiveTypeFromName primitiveType =
    match primitiveType with
    |"bool" -> typeof<bool>
    |"byte" -> typeof<byte>
    |"decimal"        -> typeof<decimal>
    |"float"          -> typeof<float>
    |"float32"        -> typeof<float32>
    |"int"            -> typeof<int>
    |"int16"          -> typeof<int16>
    |"int64"          -> typeof<int64>
    |"DateTime"       -> typeof<DateTime>
    |"DateTimeOffset" -> typeof<DateTimeOffset>
    |"TimeSpan"       -> typeof<TimeSpan>
    |"Guid"           -> typeof<Guid>
    |"byte[]" -> typeof<byte[]>
    |"string" -> typeof<string>
    |"obj"    -> typeof<obj>
    | _ -> failwithf "unexpected type: %s" primitiveType

let buildType is_nullable type_name =
    let primitiveType = DataTypes.fsharpPrimitiveType type_name
    let pType = buildPrimitiveTypeFromName primitiveType
    if is_nullable && DataTypes.fsValueTypes.Contains(primitiveType) then 
        let openType = typeof<Nullable<int>>.GetGenericTypeDefinition()
        openType.MakeGenericType(pType)
    else 
        pType

let getRecordFields (assembly:Assembly) db_name schema_name table_name =
    let fullName = sprintf "%s.%s.%s" db_name schema_name table_name
    let tp = assembly.GetType(fullName)
    let fields = 
        FSharp.Reflection.FSharpType.GetRecordFields(tp)
        |> Array.map(fun pi -> pi.Name, pi.PropertyType)
    fields

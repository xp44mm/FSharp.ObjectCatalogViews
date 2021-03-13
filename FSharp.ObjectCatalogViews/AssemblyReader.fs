module FSharp.ObjectCatalogViews.AssemblyReader

open System
open System.Reflection
open FSharp.Reflection
open FSharp.Idioms

///用反射得到数据类型定义
let getRecordFields (assembly:Assembly) db_name schema_name table_name =
    let fullName = sprintf "%s.%s.%s" db_name schema_name table_name
    let tp = assembly.GetType(fullName)
    let fields = 
        FSharpType.GetRecordFields(tp)
        |> Array.map(fun pi -> pi.Name, pi.PropertyType)
    fields

///用反射从FSharp源文件中获得表的数据
let getDataFromReflection (ass:Assembly) db_name schema_name table_name =
    /// 记录的全名
    let fullName = sprintf "%s.%s.%s" db_name schema_name table_name
    // 记录的类型
    let tp = ass.GetType(fullName)
    let pi = tp.GetProperty("DataRecords",BindingFlags.Static ||| BindingFlags.Public)
    let dr = pi.GetValue(null)

    let elementType, elements = ArrayType.readArray typeof<obj[]> dr
    
    let records = 
        elements
        |> Array.map(fun record -> 
            RecordType.readRecord (record.GetType()) record
            |> Array.map(fun(pi,value)->
                pi.Name, pi.PropertyType, value
            ))

    records


module FSharp.ObjectCatalogViews.AssemblyReader

open System
open System.Reflection
open FSharp.Reflection
open FSharp.Idioms
open FSharp.Idioms.Reflection

///用反射得到数据类型定义
let getRecordFields (assembly:Assembly) db_name schema_name table_name =
    let fullName = sprintf "%s.%s.%s" db_name schema_name table_name
    let tp = assembly.GetType(fullName)
    let fields = 
        FSharpType.GetRecordFields(tp)
        |> Array.map(fun pi -> pi.Name, pi.PropertyType)
    fields

///用反射从FSharp源文件中获得表的数据
///Fsharp源文件：LakeFunctions\Databases\Lake.fs
let getDataFromReflection (ass:Assembly) db_name schema_name table_name =
    /// 记录的全名
    let fullName = sprintf "%s.%s.%s" db_name schema_name table_name
    // 记录的类型
    let tp = ass.GetType(fullName)
    let pis = FSharpType.GetRecordFields tp
    let reader = FSharpValue.PreComputeRecordReader tp

    // 记录的静态属性DataRecords相当于表中保存的数据,注意数组元素类型是记录类型
    let records = 
        let pi = tp.GetProperty("DataRecords", BindingFlags.Static ||| BindingFlags.Public)
        let dr = pi.GetValue(null)
        let elements = ArrayType.toArray dr
        elements
        |> Array.map(fun record -> 
            let fields = reader record

            //RecordType.readRecord (record.GetType()) record
            Array.zip pis fields
            |> Array.map(fun(pi,value)->
                pi.Name, pi.PropertyType, value
            ))
    records


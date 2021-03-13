module FSharp.ObjectCatalogViews.DataTypes

open System
open System.Collections.Generic
open FSharp.Idioms.StringOps

type private DataTypeEntry = { 
    Sql:string; // ocv 里面的 type_name
    Clr:string; 
    Enumeration:string; 
    Reader:string; 
    Fsharp:string
    }


let private newRec(s,c,e,r,f) ={Sql=s;Clr=c;Enumeration=e;Reader=r;Fsharp=f}

let private ls =
    [
        newRec("bigint"          ,"Int64"           ,"BigInt"          ,"GetInt64"          ,"int64"         )
        newRec("binary"          ,"Byte[]"          ,"VarBinary"       ,"GetBytes"          ,"byte[]"        )
        newRec("bit"             ,"Boolean"         ,"Bit"             ,"GetBoolean"        ,"bool"          )
        newRec("char"            ,"String"          ,"Char"            ,"GetString"         ,"string"        )
        newRec("date"            ,"DateTime"        ,"Date"            ,"GetDateTime"       ,"DateTime"      )
        newRec("datetime"        ,"DateTime"        ,"DateTime"        ,"GetDateTime"       ,"DateTime"      )
        newRec("datetime2"       ,"DateTime"        ,"DateTime2"       ,"GetDateTime"       ,"DateTime"      )
        newRec("datetimeoffset"  ,"DateTimeOffset"  ,"DateTimeOffset"  ,"GetDateTimeOffset" ,"DateTimeOffset")
        newRec("decimal"         ,"Decimal"         ,"Decimal"         ,"GetDecimal"        ,"decimal"       )
        newRec("float"           ,"Double"          ,"Float"           ,"GetDouble"         ,"float"         )
        newRec("image"           ,"Byte[]"          ,"Binary"          ,"GetBytes"          ,"byte[]"        )
        newRec("int"             ,"Int32"           ,"Int"             ,"GetInt32"          ,"int"           )
        newRec("money"           ,"Decimal"         ,"Money"           ,"GetDecimal"        ,"decimal"       )
        newRec("nchar"           ,"String"          ,"NChar"           ,"GetString"         ,"string"        )
        newRec("ntext"           ,"String"          ,"NText"           ,"GetString"         ,"string"        )
        newRec("numeric"         ,"Decimal"         ,"Decimal"         ,"GetDecimal"        ,"decimal"       )
        newRec("nvarchar"        ,"String"          ,"NVarChar"        ,"GetString"         ,"string"        )
        newRec("real"            ,"Single"          ,"Real"            ,"GetFloat"          ,"float32"       )
        newRec("rowversion"      ,"Byte[]"          ,"Timestamp"       ,"GetBytes"          ,"byte[]"        )
        newRec("smalldatetime"   ,"DateTime"        ,"DateTime"        ,"GetDateTime"       ,"DateTime"      )
        newRec("smallint"        ,"Int16"           ,"SmallInt"        ,"GetInt16"          ,"int16"         )
        newRec("smallmoney"      ,"Decimal"         ,"SmallMoney"      ,"GetDecimal"        ,"decimal"       )
        newRec("sql_variant"     ,"Object"          ,"Variant"         ,"GetValue"          ,"obj"           )
        newRec("text"            ,"String"          ,"Text"            ,"GetString"         ,"string"        )
        newRec("time"            ,"TimeSpan"        ,"Time"            ,"GetDateTime"       ,"TimeSpan"      )
        newRec("timestamp"       ,"Byte[]"          ,"Timestamp"       ,"GetBytes"          ,"byte[]"        )
        newRec("tinyint"         ,"Byte"            ,"TinyInt"         ,"GetByte"           ,"byte"          )
        newRec("uniqueidentifier","Guid"            ,"UniqueIdentifier","GetGuid"           ,"Guid"          )
        newRec("varbinary"       ,"Byte[]"          ,"VarBinary"       ,"GetBytes"          ,"byte[]"        )
        newRec("varchar"         ,"String"          ,"VarChar"         ,"GetString"         ,"string"        )
        newRec("xml"             ,"Xml"             ,"Xml"             ,"#N/A"              ,"#N/A"          )
    ]

//值类型
let fsValueTypes = set [
    "bool"; 
    "byte"; 
    "DateTime"; 
    "DateTimeOffset"; 
    "Guid"; 
    "float"; 
    "float32"; 
    "int16"; 
    "int"; 
    "int64"; 
    "decimal"; 
    "TimeSpan"
    ]

let sqlValueTypes =
    set [
        "bigint"; "bit"; "date"; "datetime"; "datetime2"; "datetimeoffset";
        "decimal"; "float"; "int"; "money"; "numeric"; "real"; "smalldatetime";
        "smallint"; "smallmoney"; "time"; "tinyint"; 
        "uniqueidentifier";
    ]

let clr sqlType = ls |> List.find(fun rcd -> rcd.Sql == sqlType) |> fun rcd -> rcd.Clr

///不考虑可空的初步对应fsharp类型,type_name
let fsharpPrimitiveType sqlType = ls |> List.find(fun rcd -> rcd.Sql == sqlType) |> fun rcd -> rcd.Fsharp

///FSharp类型定义
let fsharpType nullable sqlType =
    let fs = fsharpPrimitiveType sqlType
    if nullable && fsValueTypes.Contains(fs) then
        sprintf "Nullable<%s>" fs
    else
        fs
///ado.net reader方法的读取方法名称
let readerMethodName sqlType = ls |> List.find(fun rcd -> rcd.Sql == sqlType) |> fun rcd -> rcd.Reader
let enum sqlType = ls |> List.find(fun rcd -> rcd.Sql == sqlType) |> fun rcd -> rcd.Enumeration
//变长类型
let private sizableTypes = set [
    "binary"; "char"; "datetime2"; "datetimeoffset"; "nchar"; "nvarchar"; "time"; "varbinary"; "varchar"]
///当使用unicode存储时，每个字符占两个byte
let sqlDataType typeName maxLength =
    if sizableTypes.Contains typeName then
        if maxLength < 0s then 
            String.Format("{0}(MAX)", typeName)
        else
            let size = 
                match typeName with
                | "nvarchar"|"nchar" -> maxLength / 2s
                | _ -> maxLength
            String.Format("{0}({1})", typeName, size)
    else
        typeName

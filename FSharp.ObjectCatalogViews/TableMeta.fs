module FSharp.ObjectCatalogViews.TableMeta

type Column = 
    {
        name : string
        is_nullable : bool
        type_name : string
    }
    static member read() = ()

type Table =
    {
        name : string
        columns : Column[]
    }

type Schema =
    {
        name : string
        tables : Table[]
    }

/// 获取结构化的表数据，表在模式下，连接字符串不要带初始目录
let getStructuralSchemas connstr db_name =
    let db = Database(connstr,db_name)

    let schemas = 
        db.Schemas
        |> Array.filter(fun sch -> db.Objects.ContainsKey sch.schema_id)
        |> Array.map(fun sch -> 
            {| 
                name = sch.name 
                schema_id = sch.schema_id 
            |}
        )

    let objects schema_id =
        db.Objects.[schema_id]
        |> Seq.map(fun o ->
            {|
                name = o.name
                object_id = o.object_id
            |}
        )
        |> Array.ofSeq

    let columns object_id =
        db.Columns.[object_id]
        |> Seq.map(fun col ->
            {
                name = col.name
                is_nullable = col.is_nullable.HasValue && col.is_nullable.Value
                type_name = col.type_name
            }
        )
        |> Array.ofSeq

    schemas
    |> Array.map(fun sc ->
        {
            name = sc.name
            tables = 
                objects sc.schema_id
                |> Array.map(fun tbl ->
                    {
                        name = tbl.name
                        columns = columns tbl.object_id
                    }
                )
        }
    )


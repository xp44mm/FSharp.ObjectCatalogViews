module FSharp.ObjectCatalogViews.TableMeta

open System.Data.SqlClient
open System

type Column = 
    {
        name : string
        is_nullable : bool
        type_name : string
    }


type Table =
    {
        name : string
        columns : Column[]
    }

    member this.Item with get(key:string) =
        this.columns |> Array.find(fun tbl -> tbl.name = key)

type Schema =
    {
        name : string
        tables : Table[]
    }

    member this.Item with get(key:string) =
        this.tables |> Array.find(fun tbl -> tbl.name = key)

type Catalog =
    {
        name:string
        schemas: Schema[]
    }

    member this.Item with get(key:string) =
        this.schemas |> Array.find(fun x -> x.name = key)


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

let getCatalog connstr db_name =
    {
        name = db_name
        schemas = getStructuralSchemas connstr db_name
    }

let readTable connstr db_name schema_name (table:Table) =
    let connstr = 
        [
            connstr
            "Initial Catalog=" + db_name
        ] |> String.concat ";"

    let sql = 
        let fields = table.columns |> Array.map(fun col -> "[" + col.name + "]") |> String.concat ", "
        sprintf "SELECT %s FROM [%s].[%s].[%s]" fields db_name schema_name table.name

    let tablColumns =
        table.columns
        |> Array.map(fun col ->
            let ty = SqlTypeUtils.getType col.is_nullable col.type_name
            col.name, ty
        )

    [|  
        use conn = new SqlConnection(connstr)
        do conn.Open()
        use comm = new SqlCommand(sql, conn)        
        use reader = comm.ExecuteReader()
        while reader.Read() do
            yield [|
                for (nm,ty) in tablColumns do
                    let value = 
                        match reader.[nm] with 
                        | :? DBNull -> null
                        | v when v.GetType() = typeof<float> -> Math.Round(unbox<float> v, 15) |> box
                        | v -> v
                    yield (nm, ty, value)
            |]
    |]

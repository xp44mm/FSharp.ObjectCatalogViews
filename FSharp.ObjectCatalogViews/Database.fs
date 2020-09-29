namespace FSharp.ObjectCatalogViews

/// 获取数据库中的表，连接字符串不要带初始目录
type Database(connstr, db_name) =
    static let typeset = set ["USER_TABLE";"VIEW"]
    let connstr = 
        [
            connstr
            "Initial Catalog=" + db_name
        ] |> String.concat ";"


    member ocv.Schemas = schemas.GetArray(connstr)
        
    member ocv.Objects = 
        objects.GetArray(connstr)
        |> Seq.filter(fun obj -> typeset.Contains obj.type_desc)
        |> Seq.groupBy(fun obj -> obj.schema_id)
        |> Map.ofSeq
    
    member ocv.Columns =
        columns.GetArray(connstr)
        |> Seq.groupBy(fun col -> col.object_id)
        |> Map.ofSeq
    
    member ocv.primaryKeys =
        indexes.GetArray(connstr)
        |> Seq.filter(fun idx -> idx.is_primary_key.HasValue && idx.is_primary_key.Value)
        |> Seq.groupBy(fun key -> key.object_id)
        |> Seq.map(fun(k,v) -> 
            let v = v |> Seq.exactlyOne
            k,v)
        |> Map.ofSeq
    
    member ocv.KeyColumns =
        index_columns.GetArray(connstr)
        |> Seq.filter(fun col -> 
            ocv.primaryKeys.ContainsKey(col.object_id) && 
            ocv.primaryKeys.[col.object_id].index_id = col.index_id)
        |> Seq.groupBy(fun col -> col.object_id)
        |> Map.ofSeq

    member ocv.DefaultConstraints =
        default_constraints.GetArray(connstr)
        |> Seq.groupBy(fun d -> d.parent_object_id,d.parent_column_id)
        |> Seq.map(fun(k,v) -> 
            let v = (v |> Seq.exactlyOne).definition
            k,v)
        |> Map.ofSeq


namespace FSharp.ObjectCatalogViews //数据结构更接近数据库的形状。将数据库元素据表一对一映射到类型中。

open System
open Microsoft.Data.SqlClient

type databases = 
    {
        database_id: int;
        name: string;
    }
    static member GetArray(connstr) =
        let sql = "SELECT [database_id], [name] FROM master.sys.databases"
        [|
            use conn = new SqlConnection(connstr)
            do conn.Open()                       
            use comm = new SqlCommand(sql, conn) 
            use reader = comm.ExecuteReader()    
            while reader.Read() do
                yield {
                    database_id = unbox<int> reader.["database_id"]
                    name = unbox<string> reader.["name"]
                }
        |]
type columns = 
    {
        object_id: int;
        name: string;
        column_id: int;
        type_name: string;
        max_length: int16;
        is_nullable: Nullable<bool>;
        is_identity: bool;
        is_computed: bool;
    }
    static member GetArray(connstr) =
        let sql = sprintf "SELECT [object_id], [name], [column_id], type_name(system_type_id) as [type_name], [max_length], [is_nullable], [is_identity], [is_computed] FROM sys.columns"
        [|
            use conn = new SqlConnection(connstr)
            do conn.Open()                       
            use comm = new SqlCommand(sql, conn) 
            use reader = comm.ExecuteReader()    
            while reader.Read() do
                yield {
                    object_id = unbox<int> reader.["object_id"]
                    name = match reader.["name"] with :? DBNull -> null | v -> unbox<string> v
                    column_id = unbox<int> reader.["column_id"]
                    type_name = match reader.["type_name"] with :? DBNull -> null | v -> unbox<string> v
                    max_length = unbox<int16> reader.["max_length"]
                    is_nullable = match reader.["is_nullable"] with :? DBNull -> Nullable() | v -> Nullable(unbox<bool> v)
                    is_identity = unbox<bool> reader.["is_identity"]
                    is_computed = unbox<bool> reader.["is_computed"]
                }
        |]
type default_constraints = 
    {
        parent_object_id: int;
        parent_column_id: int;
        definition: string;
    }
    static member GetArray(connstr) =
        let sql = sprintf "SELECT [parent_object_id], [parent_column_id], [definition] FROM sys.default_constraints"
        [|
            use conn = new SqlConnection(connstr)
            do conn.Open()                       
            use comm = new SqlCommand(sql, conn) 
            use reader = comm.ExecuteReader()    
            while reader.Read() do
                yield {
                    parent_object_id = unbox<int> reader.["parent_object_id"]
                    parent_column_id = unbox<int> reader.["parent_column_id"]
                    definition = match reader.["definition"] with :? DBNull -> null | v -> unbox<string> v
                }
        |]
type index_columns = 
    {
        object_id: int;
        index_id: int;
        column_id: int;
    }
    static member GetArray(connstr) =
        let sql = sprintf "SELECT [object_id], [index_id], [column_id] FROM sys.index_columns"
        [|
            use conn = new SqlConnection(connstr)
            do conn.Open()                       
            use comm = new SqlCommand(sql, conn) 
            use reader = comm.ExecuteReader()    
            while reader.Read() do
                yield {
                    object_id = unbox<int> reader.["object_id"]
                    index_id = unbox<int> reader.["index_id"]
                    column_id = unbox<int> reader.["column_id"]
                }
        |]
type indexes = 
    {
        object_id: int;
        index_id: int;
        is_unique: Nullable<bool>;
        is_primary_key: Nullable<bool>;
        is_unique_constraint: Nullable<bool>;
    }
    static member GetArray(connstr) =
        let sql = sprintf "SELECT [object_id], [index_id], [is_unique], [is_primary_key], [is_unique_constraint] FROM sys.indexes"
        [|
            use conn = new SqlConnection(connstr)
            do conn.Open()                       
            use comm = new SqlCommand(sql, conn) 
            use reader = comm.ExecuteReader()    
            while reader.Read() do
                yield {
                    object_id = unbox<int> reader.["object_id"]
                    index_id = unbox<int> reader.["index_id"]
                    is_unique = match reader.["is_unique"] with :? DBNull -> Nullable() | v -> Nullable(unbox<bool> v)
                    is_primary_key = match reader.["is_primary_key"] with :? DBNull -> Nullable() | v -> Nullable(unbox<bool> v)
                    is_unique_constraint = match reader.["is_unique_constraint"] with :? DBNull -> Nullable() | v -> Nullable(unbox<bool> v)
                }
        |]
type objects = 
    {
        name: string;
        object_id: int;
        schema_id: int;
        parent_object_id: int;
        type_desc: string;
    }
    static member GetArray(connstr) =
        let sql = sprintf "SELECT [name], [object_id], [schema_id], [parent_object_id], [type_desc] FROM sys.objects"
        [|
            use conn = new SqlConnection(connstr)
            do conn.Open()                       
            use comm = new SqlCommand(sql, conn) 
            use reader = comm.ExecuteReader()    
            while reader.Read() do
                yield {
                    name = unbox<string> reader.["name"]
                    object_id = unbox<int> reader.["object_id"]
                    schema_id = unbox<int> reader.["schema_id"]
                    parent_object_id = unbox<int> reader.["parent_object_id"]
                    type_desc = match reader.["type_desc"] with :? DBNull -> null | v -> unbox<string> v
                }
        |]
type parameters = 
    {
        object_id: int;
        name: string;
        parameter_id: int;
        type_name: string;
        max_length: int16;
        is_output: bool;
        has_default_value: bool;
        default_value: obj;
    }
    static member GetArray(connstr) =
        let sql = sprintf "SELECT [object_id], [name], [parameter_id], [type_name], [max_length], [is_output], [has_default_value], [default_value] FROM sys.parameters"
        [|
            use conn = new SqlConnection(connstr)
            do conn.Open()                       
            use comm = new SqlCommand(sql, conn) 
            use reader = comm.ExecuteReader()    
            while reader.Read() do
                yield {
                    object_id = unbox<int> reader.["object_id"]
                    name = match reader.["name"] with :? DBNull -> null | v -> unbox<string> v
                    parameter_id = unbox<int> reader.["parameter_id"]
                    type_name = match reader.["type_name"] with :? DBNull -> null | v -> unbox<string> v
                    max_length = unbox<int16> reader.["max_length"]
                    is_output = unbox<bool> reader.["is_output"]
                    has_default_value = unbox<bool> reader.["has_default_value"]
                    default_value = match reader.["default_value"] with :? DBNull -> null | v -> unbox<obj> v
                }
        |]
type schemas = 
    {
        name: string;
        schema_id: int;
    }
    static member GetArray(connstr) =
        let sql = sprintf "SELECT [name], [schema_id] FROM sys.schemas"
        [|
            use conn = new SqlConnection(connstr)
            do conn.Open()                       
            use comm = new SqlCommand(sql, conn) 
            use reader = comm.ExecuteReader()    
            while reader.Read() do
                yield {
                    name = unbox<string> reader.["name"]
                    schema_id = unbox<int> reader.["schema_id"]
                }
        |]

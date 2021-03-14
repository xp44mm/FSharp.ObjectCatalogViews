# FSharp.ObjectCatalogViews

将SQL数据库打印成F#记录类型。支持SqlServer数据库。

## Getting FSharp.Literals over NuGet

The simplest way of integrating FSharp.ObjectCatalogViews to your project is by using NuGet. You can install it by opening the package manager console (PM) and typing in the following statement:

```
Install-Package FSharp.ObjectCatalogViews
```

You can also use the graphical library package manager ("Manage NuGet Packages for Solution"). Searching for package's name in the official NuGet online feed will find this library.

## Get Started

### 获取数据库模式

获取数据库中所有表和视图的元数据。

```F#
let connstr = "Data Source=.;Integrated Security=True"
let db_name = "cook"
let schemas = TableMeta.getStructuralSchemas connstr db_name
```

sqlserver数据库包含若干个模式，每个模式包含若干个表或视图。我们可以从preceding code索引具体某个表的元数据。

```F#
let schema_name = "dbo"
let table_name = "book"
let table = schemas.[schema_name].[table_name]
```

### 获取数据库中的值

有了表的元数据，我们可以读取表：

```F#
let data:(string*Type*obj)[][] = TableMeta.readTable connstr db_name schema_name table
```

读取到的数据是一个交错数组，第一层表示数据有许多行，第二层表示每行有许多字段，元组的分量依次表示字段的名称，字段的类型，和字段的值。有了这些数据我们就可以利用反射将数据映射到更专用的类型中。

字段类型的映射规则：

* 不可空的列，简单按照映射表映射。

* 可为空的列，引用类型简单按照映射表映射。值类型映射为对应的`Nullable<>`类型。如：is null int 映射为`Nullable<int>`。

字段返回值映射规则：

* `DBNull.Value`映射为`null`。

* 其他按照映射表映射值。

### 应用于代码自动生成

The basic usage:

```F#
let code = ReadOnlyRecord.databaseDefinition connstr db_name
```

以上代码将会生成表字段组成的记录，并且有一个静态数组，装有数据库表内的所有数据。

有两个方法用于验证数据库文件是否过期：

`SchemaValidTest`用于验证记录类型定义是否与数据库一致。

`DataValidTest`用于验证静态数组是否与数据库中的数组一致。

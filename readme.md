# FSharp.ObjectCatalogViews

将SQL数据库打印成F#记录类型。

## Getting FSharp.Literals over NuGet

The simplest way of integrating FSharp.ObjectCatalogViews to your project is by using NuGet. You can install FSharp.ObjectCatalogViews by opening the package manager console (PM) and typing in the following statement:

```
Install-Package FSharp.ObjectCatalogViews
```

You can also use the graphical library package manager ("Manage NuGet Packages for Solution"). Searching for "FSharp.ObjectCatalogViews" in the official NuGet online feed will find this library.

## Get Started

The basic usage:

```F#
let code = ReadOnlyRecord.databaseDefinition connstr db_name
```

以上代码将会生成表字段组成的记录，并且有一个静态数组，装有数据库表内的所有数据。

有两个方法用于验证数据库文件是否过期：

`SchemaValidTest`用于验证记录类型定义是否与数据库一致。

`DataValidTest`用于验证静态数组是否与数据库中的数组一致。

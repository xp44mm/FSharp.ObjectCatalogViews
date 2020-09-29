# FSharp.ObjectCatalogViews

��SQL���ݿ��ӡ��F#��¼���͡�

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

���ϴ��뽫�����ɱ��ֶ���ɵļ�¼��������һ����̬���飬װ�����ݿ���ڵ��������ݡ�

����������������֤���ݿ��ļ��Ƿ���ڣ�

`SchemaValidTest`������֤��¼���Ͷ����Ƿ������ݿ�һ�¡�

`DataValidTest`������֤��̬�����Ƿ������ݿ��е�����һ�¡�

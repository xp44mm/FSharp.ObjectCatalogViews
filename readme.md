# FSharp.ObjectCatalogViews

��SQL���ݿ��ӡ��F#��¼���͡�֧��SqlServer���ݿ⡣

## Getting FSharp.Literals over NuGet

The simplest way of integrating FSharp.ObjectCatalogViews to your project is by using NuGet. You can install it by opening the package manager console (PM) and typing in the following statement:

```
Install-Package FSharp.ObjectCatalogViews
```

You can also use the graphical library package manager ("Manage NuGet Packages for Solution"). Searching for package's name in the official NuGet online feed will find this library.

## Get Started

### ��ȡ���ݿ�ģʽ

��ȡ���ݿ������б����ͼ��Ԫ���ݡ�

```F#
let connstr = "Data Source=.;Integrated Security=True"
let db_name = "cook"
let schemas = TableMeta.getStructuralSchemas connstr db_name
```

sqlserver���ݿ�������ɸ�ģʽ��ÿ��ģʽ�������ɸ������ͼ�����ǿ��Դ�preceding code��������ĳ�����Ԫ���ݡ�

```F#
let schema_name = "dbo"
let table_name = "book"
let table = schemas.[schema_name].[table_name]
```

### ��ȡ���ݿ��е�ֵ

���˱��Ԫ���ݣ����ǿ��Զ�ȡ��

```F#
let data:(string*Type*obj)[][] = TableMeta.readTable connstr db_name schema_name table
```

��ȡ����������һ���������飬��һ���ʾ����������У��ڶ����ʾÿ��������ֶΣ�Ԫ��ķ������α�ʾ�ֶε����ƣ��ֶε����ͣ����ֶε�ֵ��������Щ�������ǾͿ������÷��佫����ӳ�䵽��ר�õ������С�

�ֶ����͵�ӳ�����

* ���ɿյ��У��򵥰���ӳ���ӳ�䡣

* ��Ϊ�յ��У��������ͼ򵥰���ӳ���ӳ�䡣ֵ����ӳ��Ϊ��Ӧ��`Nullable<>`���͡��磺is null int ӳ��Ϊ`Nullable<int>`��

�ֶη���ֵӳ�����

* `DBNull.Value`ӳ��Ϊ`null`��

* ��������ӳ���ӳ��ֵ��

### Ӧ���ڴ����Զ�����

The basic usage:

```F#
let code = ReadOnlyRecord.databaseDefinition connstr db_name
```

���ϴ��뽫�����ɱ��ֶ���ɵļ�¼��������һ����̬���飬װ�����ݿ���ڵ��������ݡ�

����������������֤���ݿ��ļ��Ƿ���ڣ�

`SchemaValidTest`������֤��¼���Ͷ����Ƿ������ݿ�һ�¡�

`DataValidTest`������֤��̬�����Ƿ������ݿ��е�����һ�¡�

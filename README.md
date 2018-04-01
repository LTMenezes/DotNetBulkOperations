# DotNet Bulk Operations

A high performance database bulk operations library.

This package is still under construction, the goal is to support a wide range of databases that have bulk operations capabilities.

Supported Databases:

* SQL Server
* PostgreSQL

Supported Operations:

* Bulk Insert

You can keep track of the upcoming features in the [changelog](https://github.com/LTMenezes/DotNetBulkOperations/CHANGELOG.md).

## Usage

You must define models that represent the table you wish to modify:
```csharp
    [Table(name: "ModelTable")]
    public class Model
    {
        [Column(name: "name")]
        public string Name { get; set; }

        [Column(name: "surname")]
        public string Surname { get; set; }

        [Column(name: "age")]
        public Nullable<int> OptionalAge { get; set; }
    }
```

After creating the models you can perform any bulk operation with them:
```csharp
connection.BulkInsert<Model>(models);
```

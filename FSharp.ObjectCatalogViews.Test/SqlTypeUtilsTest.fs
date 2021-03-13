namespace FSharp.ObjectCatalogViews

open Xunit
open Xunit.Abstractions

open System
open FSharp.xUnit

type SqlTypeUtilsTest(output: ITestOutputHelper) =
    [<Fact>]
    member this.``getUnderlyingType``() =                
        Should.equal <| SqlTypeUtils.getUnderlyingType "bigint"           <| typeof<int64>
        Should.equal <| SqlTypeUtils.getUnderlyingType "bit"              <| typeof<bool>
        Should.equal <| SqlTypeUtils.getUnderlyingType "datetimeoffset"   <| typeof<DateTimeOffset>
        Should.equal <| SqlTypeUtils.getUnderlyingType "float"            <| typeof<float>
        Should.equal <| SqlTypeUtils.getUnderlyingType "int"              <| typeof<int>
        Should.equal <| SqlTypeUtils.getUnderlyingType "numeric"          <| typeof<decimal>
        Should.equal <| SqlTypeUtils.getUnderlyingType "nvarchar"         <| typeof<string>
        Should.equal <| SqlTypeUtils.getUnderlyingType "time"             <| typeof<TimeSpan>
        Should.equal <| SqlTypeUtils.getUnderlyingType "uniqueidentifier" <| typeof<Guid>

    [<Fact>]
    member this.``getType``() =
        let test is_nullable type_name expected_type =
            let actual_type = SqlTypeUtils.getType is_nullable type_name
            Should.equal expected_type actual_type

        test false "bigint"            typeof<int64>
        test false "bit"               typeof<bool>
        test false "datetimeoffset"    typeof<DateTimeOffset>
        test false "float"             typeof<float>
        test false "int"               typeof<int>
        test false "numeric"           typeof<decimal>
        test false "nvarchar"          typeof<string>
        test false "time"              typeof<TimeSpan>
        test false "uniqueidentifier"  typeof<Guid>

        test true "bigint"            typeof<Nullable<int64>            >
        test true "bit"               typeof<Nullable<bool>             >
        test true "datetimeoffset"    typeof<Nullable<DateTimeOffset>   >
        test true "float"             typeof<Nullable<float>            >
        test true "int"               typeof<Nullable<int>              >
        test true "numeric"           typeof<Nullable<decimal>          >
        test true "nvarchar"          typeof<string>
        test true "time"              typeof<Nullable<TimeSpan>   >
        test true "uniqueidentifier"  typeof<Nullable<Guid>       >

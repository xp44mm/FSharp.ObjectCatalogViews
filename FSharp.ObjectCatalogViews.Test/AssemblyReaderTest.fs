namespace FSharp.ObjectCatalogViews

open Xunit
open Xunit.Abstractions
open FSharp.ObjectCatalogViews
open System.IO
open Xunit
open FSharp.Reflection
open System.Reflection
open FSharp.ObjectCatalogViews


open Lake.ASME

type AssemblyReaderTest(output: ITestOutputHelper) =
    
    [<Fact>]
    member this.``getDataFromReflection should correctly extract Elbow data`` () =

        // Arrange
        let testAssembly = Assembly.GetExecutingAssembly()        
        let dbName = "Lake"
        let schemaName = "ASME"
        let tableName = "Elbow"
        
        // Act
        let result = AssemblyReader.getDataFromReflection testAssembly dbName schemaName tableName
        
        // Assert - basic structure checks
        Assert.NotNull(result)
        Assert.NotEmpty(result)
        Assert.Equal(Elbow.DataRecords.Length, result.Length)
        
        for i in [0..result.Length-1] do
            let actual = result.[i]
            let expect = Elbow.DataRecords.[i]
                    
            Assert.Equal(3, actual.Length)

            let n,t,v = actual.[0]
            Assert.Equal("DN", n)
            Assert.Equal(typeof<float>, t)
            Assert.Equal(expect.DN, unbox<float> v)

            let n,t,v = actual.[1]        
            Assert.Equal("LS", n)
            Assert.Equal(typeof<string>, t)
            Assert.Equal(expect.LS, unbox<string> v)

            let n,t,v = actual.[2]        
            Assert.Equal("R", n)
            Assert.Equal(typeof<float>, t)
            Assert.Equal(expect.R, unbox<float> v)



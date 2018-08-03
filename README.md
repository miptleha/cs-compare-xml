Compare two xml: identical they or not.  
Not just say, that xml are equal or different (as XNode.DeepEquals), but say where is difference.  
You can ignore (or not) order of subelements in element, ignore (or not) cases, ignore (or not) namespaces.  
Very useful when need to check serialization/deserialization to xml.  

## How to use
```csharp
string xml1 = "<a></a>";
string xml2 = "<b/>";
try
{
    CompareXE.CompareDeep(xml1, xml2);
    Console.WriteLine("xml are equal");
}
catch (CompareXElementException ex)
{
    Console.WriteLine("xml are different: " + ex.Message);
}
```
 
For more samples, see [Program.cs](Program.cs)

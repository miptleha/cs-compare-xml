# csharp-compare-xml
User friendly xml comparison class

## Features
-   extend functionality of XNode.DeepEquals method
-   not just say, that xml are equal or different, but say where is difference
-   support namespaces
-   allow comparison that ignoring names cases (upper or lower)
-   nodes can be in the same order or any order (set settings how compare)
-   input xml in form of text or as XElement
-   very useful when need to check serialization/deserialization to xml

## How to use
```csharp
string xml1 = "<a></a>";
string xml2 = "<b/>";
try
{
  CompareXElements.CompareDeep(xml1, xml2);
  Console.WriteLine(”xml are equal”);
}
catch (CompareXElementException ex)
{
  Console.WriteLine(”xml are different: ” + ex.Message);
}
```
 
For more samples, see [Program.cs](Program.cs)

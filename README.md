Channel  | Status | 
-------- | :------------: | 
OpenTracing.DiagnosticTracer | [![Nuget](https://img.shields.io/nuget/v/OpenTracing.DiagnosticTracer)](https://www.nuget.org/packages/OpenTracing.DiagnosticTracer/)

# Uso
 ``` csharp
using static OpenTracing.DiagnosticTracer.Tracker;
namespace Sample
{
  public void Test()
  {
     ...
      var act = 
            WithTag("tag1", "value1")
           .WithTag("tag2", "value2")
           .Start("Sample trace");
      ...
      ...
      
      act.StopTrace();
     ...
  }
}

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

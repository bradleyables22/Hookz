![hookz3](https://github.com/user-attachments/assets/a5c9b00b-11b1-4bfd-a640-d0724a3858a6)
### ✨ What is Hookz?
**Hookz.Http** adds composable lifecycle hooks (`WithBefore`, `WithAfter`, `WithError`) to your ASP.NET Minimal API endpoints.  
It’s like middleware—but scoped, fluent, and inline.

Hookz.Http helps you:
- Short-circuit requests
- Inject logging, metrics, and headers
- Clean up and finalize logic after execution
- Chain multiple hooks per route

- ### 📦 Install

```bash
dotnet add package Captain.Hookz

Supports .NET 8, and .NET 9
```
🚀 Usage Examples

📤 Modify response headers
```
app.MapGet("/data", () => Results.Ok("Payload"))
   .WithAfter(ctx =>
   {
       ctx.Response.Headers.Append("X-Processed", "true");
   });
```

🧠 Inject and call other services
```
app.MapPost("/log", () => Results.Ok("Logged"))
   .WithAfter<ILogger<Program>>(async (ctx, logger) =>
   {
	   await Task.Delay(500);
	   logger.LogInformation("POST /log handled.");
   });
```

🔁 Chain multiple hooks
```
app.MapGet("/multi", () => Results.Ok("Chained"))
   .WithBefore(ctx => Console.WriteLine("Before 1"))
   .WithBefore(ctx => Console.WriteLine("Before 2"))
   .WithAfter(ctx => Console.WriteLine("After"));
```
🧪 Unit-Test Friendly
Hookz runs cleanly in WebApplicationFactory and supports mocking HttpContext to verify DI behavior or request filtering.


### 📚 Schemaless Table Helpers

The recent addition `Hookz.Tables` namespace introduces `LogTailKey`, a struct that generates lexicographically descending `RowKey` values based on UTC timestamps — ideal for querying the most recent records in schemaless data structures like Azure Table Storage.

#### 🧭 Why use LogTailKey?

- Produces **sortable RowKeys** that naturally order newest-to-oldest
- Encapsulates tick math
- Includes full conversion support:
  - `ToUtc()` for reverse conversion
  - Implicit conversion to/from `string`
  - Comparison operators

#### 🛠️ Example Usage

```csharp
using Captain.Hookz.Tables;
using Azure.Data.Tables;

var rowKey = DateTime.UtcNow.ToLogTailKey(); // extension method
var entity = new TableEntity("device-001", rowKey)
{
    ["Status"] = "Online"
};

await tableClient.AddEntityAsync(entity);

// Later, you can reverse it
DateTime originalTime = rowKey.ToUtc();



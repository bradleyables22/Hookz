
![hookz](https://github.com/user-attachments/assets/9ef6c8b4-ad5e-4258-a1c5-523c7a508558)

### ✨ What is Hookz?
**Hookz** adds composable lifecycle hooks (`WithBefore`, `WithAfter`, `WithError`) to your ASP.NET Minimal API endpoints.  
It’s like middleware—but scoped, fluent, and inline.

Hookz helps you:
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


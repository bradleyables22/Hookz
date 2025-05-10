![hookz3](https://github.com/user-attachments/assets/eccdf216-23d6-440e-991f-b7b96f142859)

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

🔐 Short-circuit unauthorized requests
```
app.MapGet("/secure", () => Results.Ok("Authorized"))
   .WithBefore(ctx =>
   {
       if (!ctx.Request.Headers.ContainsKey("X-Auth"))
           return Results.Unauthorized();

       return ValueTask.CompletedTask;
   });
```

📤 Modify response headers
```
app.MapGet("/secure", () => Results.Ok("Authorized"))
   .WithBefore(ctx =>
   {
       if (!ctx.Request.Headers.ContainsKey("X-Auth"))
           return Results.Unauthorized();

       return ValueTask.CompletedTask;
   });
```

🧠 Inject services
```
app.MapPost("/log", () => Results.Ok("Logged"))
   .WithAfter<ILogger<Program>>((ctx, logger) =>
   {
       logger.LogInformation("POST /log handled.");
       return ValueTask.CompletedTask;
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


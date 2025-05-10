using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Captain.Hookz.Http
{
	public static class MinimalHookz
	{
		/// <summary>
		/// Registers an asynchronous 'before hook' delegate to be invoked before the endpoint's main logic.
		/// </summary>
		/// <param name="builder">The route builder.</param>
		/// <param name="before">An asynchronous delegate that runs before the endpoint handler.</param>
		/// <returns>The same <see cref="RouteHandlerBuilder"/> for further configuration.</returns>
		public static RouteHandlerBuilder WithBefore(this RouteHandlerBuilder builder, Func<HttpContext, Task> before)
		{
			builder.Add(endpointBuilder =>
			{
				var original = endpointBuilder.RequestDelegate!;
				endpointBuilder.RequestDelegate = async context =>
				{
					await before(context);
					await original(context);
				};
			});
			return builder;
		}

		/// <summary>
		/// Registers a synchronous 'before hook' delegate to be invoked before the endpoint's main logic.
		/// </summary>
		/// <param name="builder">The route builder.</param>
		/// <param name="before">A delegate that runs before the endpoint handler.</param>
		/// <returns>The same <see cref="RouteHandlerBuilder"/> for further configuration.</returns>
		public static RouteHandlerBuilder WithBefore(this RouteHandlerBuilder builder, Action<HttpContext> before)
		{
			return builder.WithBefore(ctx =>
			{
				before(ctx);
				return Task.CompletedTask;
			});
		}

		/// <summary>
		/// Registers a 'before hook' delegate that resolves a service from dependency injection.
		/// </summary>
		/// <typeparam name="TService">The type of the service to inject.</typeparam>
		/// <param name="builder">The route builder.</param>
		/// <param name="before">The hook delegate with access to <see cref="HttpContext"/> and the injected service.</param>
		/// <returns>The same <see cref="RouteHandlerBuilder"/> for further configuration.</returns>
		public static RouteHandlerBuilder WithBefore<TService>(this RouteHandlerBuilder builder, Func<HttpContext, TService, Task> before) where TService : notnull
		{
			return builder.WithBefore(async ctx =>
			{
				var service = ctx.RequestServices.GetRequiredService<TService>();
				await before(ctx, service);
			});
		}

		/// <summary>
		/// Registers an asynchronous 'after hook' delegate to be invoked after the endpoint's main logic completes successfully.
		/// </summary>
		/// <param name="builder">The route builder.</param>
		/// <param name="after">An asynchronous delegate that runs after the endpoint handler.</param>
		/// <returns>The same <see cref="RouteHandlerBuilder"/> for further configuration.</returns>
		public static RouteHandlerBuilder WithAfter(this RouteHandlerBuilder builder, Func<HttpContext, Task> after)
		{
			builder.Add(endpointBuilder =>
			{
				var original = endpointBuilder.RequestDelegate!;
				endpointBuilder.RequestDelegate = async context =>
				{
					await original(context);
					await after(context);
				};
			});
			return builder;
		}

		/// <summary>
		/// Registers a synchronous 'after hook' delegate to be invoked after the endpoint's main logic completes successfully.
		/// </summary>
		/// <param name="builder">The route builder.</param>
		/// <param name="after">A delegate that runs after the endpoint handler.</param>
		/// <returns>The same <see cref="RouteHandlerBuilder"/> for further configuration.</returns>
		public static RouteHandlerBuilder WithAfter(this RouteHandlerBuilder builder, Action<HttpContext> after)
		{
			return builder.WithAfter(ctx =>
			{
				after(ctx);
				return Task.CompletedTask;
			});
		}

		/// <summary>
		/// Registers an 'after hook' delegate that resolves one service from dependency injection.
		/// </summary>
		/// <typeparam name="TService">The type of the service to inject.</typeparam>
		/// <param name="builder">The route builder.</param>
		/// <param name="after">The hook delegate with access to <see cref="HttpContext"/> and the injected service.</param>
		/// <returns>The same <see cref="RouteHandlerBuilder"/> for further configuration.</returns>
		public static RouteHandlerBuilder WithAfter<TService>(
			this RouteHandlerBuilder builder,
			Func<HttpContext, TService, Task> after)
			where TService : notnull
		{
			return builder.WithAfter(async ctx =>
			{
				var service = ctx.RequestServices.GetRequiredService<TService>();
				await after(ctx, service);
			});
		}

		/// <summary>
		/// Registers an asynchronous 'error handler hook' delegate to handle unhandled exceptions thrown by the endpoint's main logic.
		/// </summary>
		/// <param name="builder">The route builder.</param>
		/// <param name="onError">An asynchronous delegate to handle exceptions thrown by the endpoint.</param>
		/// <returns>The same <see cref="RouteHandlerBuilder"/> for further configuration.</returns>
		public static RouteHandlerBuilder WithError(this RouteHandlerBuilder builder, Func<HttpContext, Exception, Task> onError)
		{
			builder.Add(endpointBuilder =>
			{
				var original = endpointBuilder.RequestDelegate!;
				endpointBuilder.RequestDelegate = async context =>
				{
					try
					{
						await original(context);
					}
					catch (Exception ex)
					{
						await onError(context, ex);
					}
				};
			});
			return builder;
		}

		/// <summary>
		/// Registers a synchronous 'error handler hook' delegate to handle unhandled exceptions thrown by the endpoint's main logic.
		/// </summary>
		/// <param name="builder">The route builder.</param>
		/// <param name="onError">A delegate to handle exceptions thrown by the endpoint.</param>
		/// <returns>The same <see cref="RouteHandlerBuilder"/> for further configuration.</returns>
		public static RouteHandlerBuilder WithError(this RouteHandlerBuilder builder, Action<HttpContext, Exception> onError)
		{
			return builder.WithError((ctx, ex) =>
			{
				onError(ctx, ex);
				return Task.CompletedTask;
			});
		}

		/// <summary>
		/// Registers an 'error handler hook' that resolves one service from dependency injection.
		/// </summary>
		/// <typeparam name="TService">The type of the service to inject.</typeparam>
		/// <param name="builder">The route builder.</param>
		/// <param name="onError">The error hook delegate with access to <see cref="HttpContext"/>, the exception, and the injected service.</param>
		/// <returns>The same <see cref="RouteHandlerBuilder"/> for further configuration.</returns>
		public static RouteHandlerBuilder WithError<TService>(
			this RouteHandlerBuilder builder,
			Func<HttpContext, Exception, TService, Task> onError)
			where TService : notnull
		{
			return builder.WithError(async (ctx, ex) =>
			{
				var service = ctx.RequestServices.GetRequiredService<TService>();
				await onError(ctx, ex, service);
			});
		}
	}
}

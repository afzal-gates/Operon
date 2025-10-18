using MediatR;
using Microsoft.AspNetCore.Builder;
using Operon.Api.Filters;
using Operon.Application.Extensions;
using Operon.Application.Features.Dtos;
using Operon.Application.Handlers;
namespace Operon.Api.Endpoints
{
    public class TodoEndpoints
    {
        public static void Map(WebApplication app)
        {
            var v1Group = app.MapGroup("/api/v1/todos")
                             .WithTags("Todos V1")
                             .RequireRateLimiting("api")
                             .RequireAuthorization();

            v1Group.MapGet("/", async (IMediator mediator, CancellationToken ct) => await GetTodosV1(mediator, ct)).AllowAnonymous();
            v1Group.MapPost("/", async (IMediator mediator, CreateTodoDto dto, CancellationToken ct) => await PostTodosV1(mediator, dto, ct))
                .AddEndpointFilter<FluentValidationFilter<CreateTodoDto>>().AllowAnonymous();

            var v2Group = app.MapGroup("/api/v2/todos")
                             .WithTags("Todos V2")
                             .RequireRateLimiting("api")
                             .RequireAuthorization();

            v2Group.MapGet("/", async (IMediator mediator) => await GetTodosV2(mediator)); // newer version
            v2Group.MapPost("/", async (IMediator mediator, CreateTodoDto dto, CancellationToken ct) => await PostTodosV2(mediator, dto, ct)); // newer version
        }

        private static async Task<IResult> GetTodosV1(IMediator mediator, CancellationToken ct)
        {
            var todos = await mediator.Send(new Operon.Application.Handlers.GetTodosQuery(), ct).AsSuccess();
            return Results.Ok(todos);
        }

        private static async Task<IResult> PostTodosV1(IMediator mediator, CreateTodoDto dto, CancellationToken ct)
        {
            var todos = await mediator.Send(new Operon.Application.Handlers.CreateTodoCommand(dto.Title), ct).AsCreated();
            return Results.Ok(todos);
        }

        private static async Task<IResult> GetTodosV2(IMediator mediator)
        {
            var todos = await mediator.Send(new Operon.Application.Handlers.GetTodosQuery()).AsSuccess();
            return Results.Ok(todos);
        }

        private static async Task<IResult> PostTodosV2(IMediator mediator, CreateTodoDto dto, CancellationToken ct)
        {
            var todos = await mediator.Send(new Operon.Application.Handlers.CreateTodoCommand(dto.Title), ct).AsCreated();
            return Results.Ok(todos);
        }
    }

}

using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Operon.Domain.Entities;
using Operon.Infrastructure;
using System.Text.Json;

namespace Operon.Application.Handlers;

public record GetTodosCachedQuery() : IRequest<IEnumerable<Todo>>;

public class GetTodosCachedHandler(OperonDbContext db, IDistributedCache cache)
    : IRequestHandler<GetTodosCachedQuery, IEnumerable<Todo>>
{
    const string Key = "todos:all";
    public async Task<IEnumerable<Todo>> Handle(GetTodosCachedQuery _, CancellationToken ct)
    {
        var cached = await cache.GetStringAsync(Key, ct);
        if (cached is not null)
            return JsonSerializer.Deserialize<IEnumerable<Todo>>(cached)!;

        var data = await db.Todos.AsNoTracking().ToListAsync(ct);
        await cache.SetStringAsync(Key, JsonSerializer.Serialize(data),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2) }, ct);
        return data;
    }
}
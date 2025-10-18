using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Operon.Domain.Entities;
using Operon.Infrastructure;
using System.Net.NetworkInformation;
using ThirdParty.Json.LitJson;

namespace Operon.Application.Handlers;

public record GetTodosQuery : IRequest<IEnumerable<Todo>>;

public class GetTodosHandler : IRequestHandler<GetTodosQuery, IEnumerable<Todo>>
{
    private readonly OperonDbContext _db;
    public GetTodosHandler(OperonDbContext db) => _db = db;

    public async Task<IEnumerable<Todo>> Handle(GetTodosQuery request, CancellationToken cancellationToken)
    {
        var list = await _db.Todos.AsNoTracking().ToListAsync(cancellationToken);
        return list;
    }
}


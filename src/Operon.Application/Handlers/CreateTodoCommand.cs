using MediatR;
using Operon.Domain.Entities;
using Operon.Infrastructure;

namespace Operon.Application.Handlers;

public record CreateTodoCommand(string Title) : IRequest<Todo>;

public class CreateTodoHandler : IRequestHandler<CreateTodoCommand, Todo>
{
    private readonly OperonDbContext _db;
    public CreateTodoHandler(OperonDbContext db) => _db = db;

    public async Task<Todo> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
    {
        var t = new Todo { Title = request.Title };
        _db.Todos.Add(t);
        await _db.SaveChangesAsync(cancellationToken);
        return t;
    }
}

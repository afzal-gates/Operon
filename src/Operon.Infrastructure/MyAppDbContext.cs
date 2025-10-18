using Microsoft.EntityFrameworkCore;
using Operon.Domain.Entities;

namespace Operon.Infrastructure;
public class OperonDbContext : DbContext
{
    public OperonDbContext(DbContextOptions<OperonDbContext> options) : base(options) {}
    public DbSet<Todo> Todos => Set<Todo>();
}

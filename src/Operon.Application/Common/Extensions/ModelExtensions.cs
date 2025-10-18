using Operon.Domain.Common.Interfaces;

namespace Operon.Application.Extensions
{
    public static class ModelExtensions
    {

        public static IEnumerable<TEntity> MakeUnique<TEntity>(this IEnumerable<TEntity> entities) where TEntity : IEntity
        {
            int count = 0;
            foreach (var entity in entities)
            {
                entity.Id = count++;
                yield return entity;
            }
        }


        public static string GetExceptionDetails(this Exception ex)
        {
            var message = "(1)." + ex.Message;
            message += "(2)." + ex.InnerException?.Message;
            message += "(3)." + ex.InnerException?.InnerException?.Message;
            message += "(StackTrace)." + ex.StackTrace;

            return message;
        }

        public static string GetExceptionDetails(this Exception ex, string source = null)
        {
            string message = "(1)." + ex.Message;
            message += "(2)." + ex.InnerException?.Message;
            message += "(3)." + ex.InnerException?.InnerException?.Message;
            message += "STACK_TRACE ::: " + ex.StackTrace;

            if (!string.IsNullOrEmpty(source))
            {
                message = source + ": " + message;
            }
            return message;
        }
    }
}

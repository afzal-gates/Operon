using FluentValidation;

namespace Operon.Api.Filters
{
    public class FluentValidationFilter<T> : IEndpointFilter
    {
        private readonly IValidator<T> _validator;

        public FluentValidationFilter(IValidator<T> validator)
        {
            _validator = validator;
        }

        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            // Extract model from endpoint parameters
            var model = context.Arguments.OfType<T>().FirstOrDefault();

            if (model is null)
            {
                return Results.BadRequest(new
                {
                    data = (object?)null,
                    status = new { code = 400, message = "Invalid request payload." }
                });
            }

            var validationResult = await _validator.ValidateAsync(model);

            if (!validationResult.IsValid)
            {
                return Results.BadRequest(new
                {
                    data = validationResult.Errors.Select(e => new { field = e.PropertyName, error = e.ErrorMessage }),
                    status = new { code = 400, message = "Validation failed." }
                });
            }

            // If validation passed, move to the next filter/handler
            return await next(context);
        }
    }
}

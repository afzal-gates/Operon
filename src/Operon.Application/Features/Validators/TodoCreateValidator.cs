using FluentValidation;
using Operon.Application.Features.Dtos;
using Operon.Application.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Operon.Application.Features.Validators
{
    public class TodoCreateValidator : AbstractValidator<CreateTodoDto>
    {
        public TodoCreateValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title must be less than 100 characters.");
        }
    }
}

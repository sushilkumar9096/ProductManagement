using FluentValidation;
using Product.Application.DTOs;

namespace Product.Application.Validators
{
    public class CreateItemDtoValidator : AbstractValidator<CreateItemDto>
    {
        public CreateItemDtoValidator()
        {
            RuleFor(i => i.ProductId)
                .GreaterThan(0).WithMessage("Product ID must be greater than 0.");
            
            RuleFor(i => i.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0.");
        }
    }
}

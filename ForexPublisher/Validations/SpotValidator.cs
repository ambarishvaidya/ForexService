using FluentValidation;

namespace ForexPublisher.Validations;

public class SpotValidator : AbstractValidator<Domain.Spot>
{
    public SpotValidator()
    {
        RuleFor(spot => spot.CurrencyPair).NotEmpty().WithMessage("Currency pair cannot be empty.");

        RuleFor(spot => spot.Bid).NotEmpty().WithMessage("Bid cannot be empty.");
        RuleFor(spot => spot.Bid).GreaterThan(0.0d).WithMessage("Bid has to be a greater than 0.");

        RuleFor(spot => spot.Ask).NotEmpty().WithMessage("Ask cannot be empty.");
        RuleFor(spot => spot.Ask).GreaterThan(0.0d).WithMessage("Ask has to be a greater than 0.");

        RuleFor(spot => spot.Spread).NotEmpty().WithMessage("Spread cannot be empty.");
        RuleFor(spot => spot.Spread).GreaterThan(0.0d).WithMessage("Spread has to be a greater than 0.");

        RuleFor(spot => spot.Bid).LessThan(spot => spot.Ask).WithMessage("Bid cannot be more than Ask.");

        RuleFor(spot => spot.PublishFrequencyInMs).NotEmpty().WithMessage("Publish frequency cannot be empty.");
        RuleFor(spot => spot.PublishFrequencyInMs).GreaterThan(0).WithMessage("Publish frequency has to be a greater than 0.");
    }
}

using FluentValidation;

namespace Wordle.Application.Guesses.Commands.Create;

public class CreateGuessCommandValidator : AbstractValidator<CreateGuessCommand>
{
    public CreateGuessCommandValidator()
    {
        RuleFor(x => x.GuessText)
            .NotEmpty()
            .Length(5)
            .WithMessage("Tahmin tam olarak 5 harf olmalıdır.");
    }
}

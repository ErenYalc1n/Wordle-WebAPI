using MediatR;

namespace Wordle.Application.Guesses.Commands.Create;

public class CreateGuessCommand : IRequest<Guid>
{
    public string GuessText { get; set; } = string.Empty;
}

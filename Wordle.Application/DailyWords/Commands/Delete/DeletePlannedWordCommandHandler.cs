using MediatR;
using Wordle.Domain.DailyWords;

namespace Wordle.Application.DailyWords.Commands.Delete;

public class DeletePlannedWordCommandHandler : IRequestHandler<DeletePlannedWordCommand>
{
    private readonly IDailyWordRepository _repository;

    public DeletePlannedWordCommandHandler(IDailyWordRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(DeletePlannedWordCommand request, CancellationToken cancellationToken)
    {
        await _repository.DeleteByDateAsync(request.Date);
        return Unit.Value;
    }
}

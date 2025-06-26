using MediatR;
using Wordle.Domain.DailyWords;

namespace Wordle.Application.DailyWords.Commands.Add;

public class AddDailyWordCommandHandler : IRequestHandler<AddDailyWordCommand, Guid>
{
    private readonly IDailyWordRepository _repository;

    public AddDailyWordCommandHandler(IDailyWordRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(AddDailyWordCommand request, CancellationToken cancellationToken)
    {
        var date = request.DailyWord.Date.ToDateTime(TimeOnly.MinValue); 

        var isTaken = await _repository.IsDateTakenAsync(date);
        if (isTaken)
            throw new InvalidOperationException("Bu tarihe ait bir kelime zaten var.");

        var word = new DailyWord
        {
            Id = Guid.NewGuid(),
            Word = request.DailyWord.Word,
            Date = date
        };

        await _repository.AddAsync(word);
        return word.Id;
    }
}

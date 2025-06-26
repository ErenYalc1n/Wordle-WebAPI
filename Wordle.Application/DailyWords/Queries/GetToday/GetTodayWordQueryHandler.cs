using MediatR;
using Wordle.Domain.DailyWords;
using Wordle.Application.DailyWords.DTOs;

namespace Wordle.Application.DailyWords.Queries.GetToday;

public class GetTodayWordQueryHandler : IRequestHandler<GetTodayWordQuery, GetDailyWordDto?>
{
    private readonly IDailyWordRepository _repository;

    public GetTodayWordQueryHandler(IDailyWordRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetDailyWordDto?> Handle(GetTodayWordQuery request, CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(3));
        var word = await _repository.GetTodayWordAsync(today);

        if (word is null)
            return null;

        return new GetDailyWordDto
        {
            Word = word.Word
        };
    }
}

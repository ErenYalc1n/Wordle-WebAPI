using MediatR;
using Wordle.Application.Common.Exceptions;
using Wordle.Application.DailyWords.DTOs;
using Wordle.Domain.DailyWords;

namespace Wordle.Application.DailyWords.Queries.Search;

public class SearchDailyWordQueryHandler : IRequestHandler<SearchDailyWordQuery, SearchDailyWordDto?>
{
    private readonly IDailyWordRepository _repository;

    public SearchDailyWordQueryHandler(IDailyWordRepository repository)
    {
        _repository = repository;
    }

    public async Task<SearchDailyWordDto?> Handle(SearchDailyWordQuery request, CancellationToken cancellationToken)
    {
        var input = request.SearchInput.Trim();
       
        if (!IsValidInput(input))
            throw new InvalidSearchInputException("Kelime için en az 5 harf gereklidir, tarih ise dd.MM.yyyy formatında olmalıdır.");

        DailyWord? found = null;

        if (DateOnly.TryParseExact(input, "dd.MM.yyyy", out var date))
        {
            found = await _repository.GetByDateAsync(date);
        }
        else
        {
            found = await _repository.GetByWordAsync(input);
        }

        if (found is null)
            return null;

        var today = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(3));
        var foundDate = DateOnly.FromDateTime(found.Date);

        var status = foundDate < today ? "geçmiş"
                    : foundDate == today ? "bugün"
                    : "gelecek";

        return new SearchDailyWordDto
        {
            Id = found.Id,
            Word = found.Word,
            Date = foundDate,
            Status = status
        };
    }

    private bool IsValidInput(string input)
    {
        if (DateOnly.TryParseExact(input, "dd.MM.yyyy", out _))
            return true;

        return input.Length == 5 && input.All(char.IsLetter);
    }
}

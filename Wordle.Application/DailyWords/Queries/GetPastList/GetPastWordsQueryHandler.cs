using MediatR;
using Wordle.Application.DailyWords.DTOs;
using Wordle.Domain.DailyWords;

namespace Wordle.Application.DailyWords.Queries.GetPastList;

public class GetPastWordsQueryHandler : IRequestHandler<GetPastWordsQuery, DailyWordListResultDto>
{
    private readonly IDailyWordRepository _repository;

    public GetPastWordsQueryHandler(IDailyWordRepository repository)
    {
        _repository = repository;
    }

    public async Task<DailyWordListResultDto> Handle(GetPastWordsQuery request, CancellationToken cancellationToken)
    {
        var allPastWords = await _repository.GetPastWordsAsync(request.Page, request.PageSize);
        var totalCount = await _repository.CountPastAsync();


        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        return new DailyWordListResultDto
        {
            CurrentPage = request.Page,
            TotalPages = totalPages,
            Words = allPastWords.Select(w => new DailyWordListItemDto
            {
                Word = w.Word,
                Date = DateOnly.FromDateTime(w.Date)
            }).ToList()
        };
    }
}

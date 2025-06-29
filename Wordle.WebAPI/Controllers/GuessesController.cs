using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wordle.Application.Guesses.Commands.Create;

namespace Wordle.WebAPI.Controllers;

[Authorize(Roles = "Player")]
[ApiController]
[Route("api/[controller]")]
public class GuessesController : ControllerBase
{
    private readonly ISender _mediator;

    public GuessesController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = "Player")]
    public async Task<IActionResult> Create([FromBody] CreateGuessCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

}

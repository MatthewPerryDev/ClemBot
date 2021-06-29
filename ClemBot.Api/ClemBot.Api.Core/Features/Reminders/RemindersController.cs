using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClemBot.Api.Core.Features.Reminders.Bot;
using ClemBot.Api.Core.Security;
using ClemBot.Api.Core.Security.Policies;
using ClemBot.Api.Core.Security.Policies.BotMaster;
using ClemBot.Api.Core.Utilities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace ClemBot.Api.Core.Features.Reminders
{
    [ApiController]
    [Route("api")]
    public class RemindersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RemindersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("bot/[controller]")]
        [BotMasterAuthorize]
        public async Task<IActionResult> Create(Create.Command command) =>
            await _mediator.Send(command) switch
            {
                { Status: QueryStatus.Success } result => Ok(result.Value),
                { Status: QueryStatus.Conflict } => Conflict(),
                _ => throw new InvalidOperationException()
            };

        [HttpDelete("bot/[controller]")]
        [BotMasterAuthorize]
        public async Task<IActionResult> Delete(Delete.Command command) =>
            await _mediator.Send(command) switch
            {
                { Status: QueryStatus.Success } result => Ok(result.Value),
                { Status: QueryStatus.NotFound } => NoContent(),
                _ => throw new InvalidOperationException()
            };

        [HttpGet("bot/[controller]")]
        [BotMasterAuthorize]
        public async Task<IActionResult> Retrieve(Retrieve.Query query) =>
            await _mediator.Send(query) switch
            {
                { Status: QueryStatus.Success } result => Ok(result.Value),
                _ => NoContent()
            };
        [HttpGet("bot/[controller]/BulkRetrieve")]
        [BotMasterAuthorize]
        public async Task<IActionResult> BulkRetrieve() =>
            await _mediator.Send(new Bot.BulkRetrieve.Query()) switch
            {
                { Status: QueryStatus.Success } result => Ok(result.Value),
                _ => Ok(new List<ulong>())
            };

    }

}

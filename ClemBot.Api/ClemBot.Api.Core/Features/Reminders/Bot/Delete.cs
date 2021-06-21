using System;
using System.Threading;
using System.Threading.Tasks;
using ClemBot.Api.Core.Utilities;
using ClemBot.Api.Data.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClemBot.Api.Core.Features.Reminders.Bot
{
    public class Delete
    {
        public class Command : IRequest<Result<Model, QueryStatus>>
        {
            public ulong UserId { get; set; }

            public ulong MessageId { get; set; }
        }

        public class Model
        {
            public string Link { get; set; } = null!;

            public DateTime Time { get; set; }

            public ulong MessageId { get; set; }

            public ulong UserId { get; set; }
        }

        public record QueryHandler(ClemBotContext _context) : IRequestHandler<Command, Result<Model, QueryStatus>>
        {
            public async Task<Result<Model, QueryStatus>> Handle(Command request, CancellationToken cancellationToken)
            {
                var reminder = await _context.Reminders
                    .FirstOrDefaultAsync(g => g.UserId == request.UserId && g.MessageId == request.MessageId);

                if (reminder is null)
                {
                    return QueryResult<Model>.NotFound();
                }

                _context.Reminders.Remove(reminder);

                await _context.SaveChangesAsync();

                return QueryResult<Model>.Success(new Model()
                {
                    Link = reminder.Link,
                    Time = reminder.Time,
                    MessageId = reminder.MessageId,
                    UserId = reminder.UserId
                });
            }
        }
    }
}

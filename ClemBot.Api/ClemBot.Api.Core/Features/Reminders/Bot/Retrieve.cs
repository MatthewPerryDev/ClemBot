using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClemBot.Api.Core.Utilities;
using ClemBot.Api.Data.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClemBot.Api.Core.Features.Reminders.Bot
{
    public class Retrieve
    {
        public class Query : IRequest<Result<Model, QueryStatus>>
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

        public record QueryHandler(ClemBotContext _context) : IRequestHandler<Query, Result<Model, QueryStatus>>
        {
            public async Task<Result<Model, QueryStatus>> Handle(Query request, CancellationToken cancellationToken)
            {
                var reminder = await _context.Reminders
                    .Where(g => g.MessageId == request.MessageId && g.UserId == request.UserId)
                    .FirstOrDefaultAsync();

                if (reminder is null)
                {
                    return QueryResult<Model>.NotFound();
                }

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

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
            public int Id { get; set; }
        }

        public class Model
        {
            public int Id { get; set; }
            public string Link { get; set; } = null!;
            public DateTime Time { get; set; }
            public ulong UserId { get; set; }

        }

        public record QueryHandler(ClemBotContext _context) : IRequestHandler<Query, Result<Model, QueryStatus>>
        {
            public async Task<Result<Model, QueryStatus>> Handle(Query request, CancellationToken cancellationToken)
            {
                var reminder = await _context.Reminders
                    .Where(g => g.Id == request.Id)
                    .FirstOrDefaultAsync();

                if (reminder is null)
                {
                    return QueryResult<Model>.NotFound();
                }

                return QueryResult<Model>.Success(new Model()
                {
                    Id = reminder.Id,
                    Link = reminder.Link,
                    Time = reminder.Time,
                    UserId = reminder.UserId
                });
            }
        }
    }
}

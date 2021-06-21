using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClemBot.Api.Core.Utilities;
using ClemBot.Api.Data.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClemBot.Api.Core.Features.Reminders.Bot
{
    public class BulkRetrieve
    {
        public class Query : IRequest<Result<IEnumerable<Model>, QueryStatus>>
        {
        }

        public class Model
        {
            public string Link { get; set; } = null!;

            public DateTime Time { get; set; }

            public ulong MessageId { get; set; }

            public ulong UserId { get; set; }
        }

        public record Handler(ClemBotContext _context) : IRequestHandler<Query, Result<IEnumerable<Model>, QueryStatus>>
        {
            public async Task<Result<IEnumerable<Model>, QueryStatus>> Handle(Query request,
                CancellationToken cancellationToken)
            {
                var reminders = await _context.Reminders.ToListAsync();

                if (!reminders.Any())
                {
                    return QueryResult<IEnumerable<Model>>.NotFound();
                }

                return QueryResult<IEnumerable<Model>>.Success(
                    reminders.Select(x => new Model
                    {
                        Link = x.Link,
                        Time = x.Time,
                        MessageId = x.MessageId,
                        UserId = x.UserId
                    })
                );
            }
        }
    }
}

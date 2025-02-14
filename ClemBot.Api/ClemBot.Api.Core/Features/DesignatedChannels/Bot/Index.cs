using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClemBot.Api.Common.Utilities;
using ClemBot.Api.Data.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClemBot.Api.Core.Features.DesignatedChannels.Bot;

public class Index
{
    public class Query : IRequest<QueryResult<IEnumerable<ulong>>>
    {
        public Common.Enums.DesignatedChannels Designation { get; set; }
    }

    public record Handler(ClemBotContext _context) :
        IRequestHandler<Query, QueryResult<IEnumerable<ulong>>>
    {
        public async Task<QueryResult<IEnumerable<ulong>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var designatedChannels = await _context.DesignatedChannelMappings
                .Where(x => x.Type == request.Designation)
                .Select(y => y.ChannelId)
                .ToListAsync();

            return QueryResult<IEnumerable<ulong>>.Success(designatedChannels);
        }
    }
}

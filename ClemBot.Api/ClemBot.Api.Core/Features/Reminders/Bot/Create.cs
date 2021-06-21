using System;
using System.Threading;
using System.Threading.Tasks;
using ClemBot.Api.Core.Utilities;
using ClemBot.Api.Data.Contexts;
using ClemBot.Api.Data.Models;
using FluentValidation;
using MediatR;

namespace ClemBot.Api.Core.Features.Reminders.Bot
{
    public class Create
    {
        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(p => p.Link).NotNull();
                RuleFor(p => p.Time).NotNull();
                RuleFor(p => p.MessageId).NotNull();
                RuleFor(p => p.UserId).NotNull();
            }
        }
        public class Command : IRequest<Result<int, QueryStatus>>
        {
            public ulong UserId { get; set; }
            public ulong MessageId { get; set; }
            public string Link { get; set; } = null!;
            public DateTime Time { get; set; }

        }

        public record Handler(ClemBotContext _context) : IRequestHandler<Command, Result<int, QueryStatus>>
        {
            public async Task<Result<int, QueryStatus>> Handle(Command request, CancellationToken cancellationToken)
            {
                var reminder = new Reminder()
                {

                    Link = request.Link,
                    UserId = request.UserId,
                    Time = request.Time,
                    MessageId = request.MessageId,
                };

                _context.Reminders.Add(reminder);
                await _context.SaveChangesAsync();

                return QueryResult<int>.Success(reminder.Id);

            }
        }
    }
}

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ClemBot.Api.Core.Utilities;
using ClemBot.Api.Data.Contexts;
using ClemBot.Api.Data.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClemBot.Api.Core.Features.Guilds
{
    public class UpdateUsers
    {
        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(p => p.Id).NotNull();
                RuleFor(p => p.Users).NotEmpty();
            }
        }

        public class UserDto
        {
            public ulong Id { get; set; }

            public string? Name { get; set; }
        }

        public record Command : IRequest<Result<ulong, QueryStatus>>
        {
            public ulong Id { get; set; }

            public List<UserDto> Users { get; set; } = new();
        }

        public record Handler(ClemBotContext _context) : IRequestHandler<Command, Result<ulong, QueryStatus>>
        {
            public async Task<Result<ulong, QueryStatus>> Handle(Command request, CancellationToken cancellationToken)
            {
                var guild = await _context.Guilds
                    .Where(x => x.Id == request.Id)
                    .Include(y => y.Users)
                    .FirstOrDefaultAsync();

                if (guild is null)
                {
                    return QueryResult<ulong>.Conflict();
                }

                foreach (var user in request.Users)
                {
                    var dbUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == user.Id);

                    if (dbUser is null)
                    {
                        var userEntity = new User() {Id = user.Id, Name = user.Name};
                        _context.Users.Add(userEntity);
                        guild.Users.Add(userEntity);
                        continue;
                    }

                    if (!guild.Users.Contains(dbUser))
                    {
                       guild.Users.Add(dbUser);
                    }
                }

                foreach (var user in guild.Users.Where(x => request.Users.All(y => y.Id != x.Id)).ToList())
                {
                    _context.Users.Remove(user);
                }

                await _context.SaveChangesAsync();

                return QueryResult<ulong>.Success(request.Id);
            }
        }
    }
}

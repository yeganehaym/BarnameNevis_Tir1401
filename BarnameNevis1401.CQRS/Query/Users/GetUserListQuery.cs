using BarnameNevis1401.Data.SqlServer;
using BarnameNevis1401.Domains.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BarnameNevis1401.CQRS.Query.Users;

public class GetUserListQuery:IRequest<List<User>>
{
    public int Skip { get; set; }
    public int Take { get; set; }
}

public class GetUserListQueryHandler : IRequestHandler<GetUserListQuery, List<User>>
{
    private ApplicationDbContext _context;

    public GetUserListQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<User>> Handle(GetUserListQuery request, CancellationToken cancellationToken)
    {
        return await _context
            .Users
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Skip(request.Skip)
            .Take(request.Take)
            .ToListAsync(cancellationToken: cancellationToken);
    }
}
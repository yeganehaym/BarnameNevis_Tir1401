using BarnameNevis1401.CQRS.Notifications;
using BarnameNevis1401.Data.SqlServer;
using BarnameNevis1401.Domains.Users;
using BarnameNevis1401.Infrastructure;
using Mapster;
using MapsterMapper;
using MediatR;

namespace BarnameNevis1401.CQRS.Commands.Users;

public class CreatOrUpdateUserCommand:IRequest<User>
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Mobile { get; set; }
    public string Password { get; set; }
    public string[] Roles { get; set; }
}

public class CreatOrUpdateUserCommandHandler : IRequestHandler<CreatOrUpdateUserCommand, User>
{
    private ApplicationDbContext _context;


    private IMapper _mapper;
    private IMediator _mediator;
    public CreatOrUpdateUserCommandHandler(ApplicationDbContext context, IMapper mapper, IMediator mediator)
    {
        _context = context;
        _mapper = mapper;
        _mediator = mediator;
    }

    public async Task<User> Handle(CreatOrUpdateUserCommand request, CancellationToken cancellationToken)
    {
        User user = null;
        if (request.Id > 0)
        {
            user = await _context.Users.FindAsync(request.Id);
        }
        else
        {
            user = new()
            {
                SerialNumber = "434343"
            };
            _context.Add(user);
        }


        _mapper.Map(request, user);
        //request.Adapt(user);
        user.Password = Converter.Hash(user.Password);

        var rows =await _context.SaveChangesAsync(cancellationToken);

        if (rows > 0)
        {
            _mediator.Publish(new NewUserNotification() { User = user });
            return user;
        }
        return null;
    }
}
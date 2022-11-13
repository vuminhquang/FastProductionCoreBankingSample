using MediatR;

namespace CoreBanking.Application.Core.Services;

public class AccountsService
{
    private readonly IMediator _mediator;

    public AccountsService(IMediator mediator)
    {
        _mediator = mediator;
    }
}
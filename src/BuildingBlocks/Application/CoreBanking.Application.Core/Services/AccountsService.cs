using CoreBanking.Domain.Core.Commands;
using CoreBanking.Domain.Core.Models;
using MediatR;

namespace CoreBanking.Application.Core.Services;

public class AccountsService
{
    private readonly IMediator _mediator;

    public AccountsService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<Guid> Create(Guid ownerId, Currency currency, CancellationToken cancellationToken = default)
    {
        var accountId = Guid.NewGuid();
        var command = new CreateAccount(ownerId, accountId, currency);
        await _mediator.Publish(command, cancellationToken);
        return accountId;
    }
}
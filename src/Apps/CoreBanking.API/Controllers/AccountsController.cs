using CoreBanking.Application.Core.DTOs;
using CoreBanking.Application.Core.Services;
using CoreBanking.Domain.Core.Commands;
using CoreBanking.Domain.Core.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CoreBanking.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly AccountsService _accountsService;

        public AccountsController(IMediator mediator, AccountsService accountsService)
        {
            _mediator = mediator;
            _accountsService = accountsService;
        }

        [HttpGet, Route("{id:guid}", Name = "GetAccount")]
        public async Task<IActionResult> GetAccount(Guid id, CancellationToken cancellationToken = default)
        {
            return NotFound();
            // var query = new AccountById(id);
            // var result = await _mediator.Send(query, cancellationToken);
            // if (result is null)
            //     return NotFound();
            // return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountDto dto, CancellationToken cancellationToken = default)
        {
            if (null == dto)
                return BadRequest();

            var currency = Currency.FromCode(dto.CurrencyCode);
            var accountId = await _accountsService.Create(dto.CustomerId, currency, cancellationToken);
            // return CreatedAtAction("GetAccount", "Accounts", new { id = accountId }, command);
            return CreatedAtAction("GetAccount", controllerName: "Accounts", routeValues:new { id = accountId }, accountId);
        }


        [HttpPut, Route("{id:guid}/deposit")]
        public async Task<IActionResult> Deposit([FromRoute]Guid id, [FromBody]DepositDto dto, CancellationToken cancellationToken = default)
        {
            if (null == dto)
                return BadRequest();

            var currency = Currency.FromCode(dto.CurrencyCode);
            var amount = new Money(currency, dto.Amount);
            var command = new Deposit(id, amount);
            await _mediator.Publish(command, cancellationToken);
            return Ok();
        }

        [HttpPut, Route("{id:guid}/withdraw")]
        public async Task<IActionResult> Withdraw([FromRoute]Guid id, [FromBody]WithdrawDto dto, CancellationToken cancellationToken = default)
        {
            if (null == dto)
                return BadRequest();

            var currency = Currency.FromCode(dto.CurrencyCode);
            var amount = new Money(currency, dto.Amount);
            var command = new Withdraw(id, amount);
            await _mediator.Publish(command, cancellationToken);
            return Ok();
        }
    }
}
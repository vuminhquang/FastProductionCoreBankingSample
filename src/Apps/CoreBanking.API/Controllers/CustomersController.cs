using CoreBanking.Application.Core.DTOs;
using CoreBanking.Application.Core.Services;
using CoreBanking.Domain.Core.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CoreBanking.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly CustomersService _customersService;

        public CustomersController(IMediator mediator, CustomersService customersService)
        {
            _mediator = mediator;
            _customersService = customersService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCustomerDto dto, CancellationToken cancellationToken = default)
        {
            if (null == dto)
                return BadRequest();
            var command = new CreateCustomer(Guid.NewGuid(), dto.FirstName, dto.LastName, dto.Email);
            await _mediator.Publish(command, cancellationToken);
            
            return CreatedAtAction("GetCustomer", new { id = command.CustomerId }, command);
        }

        [HttpGet, Route("{id:guid}", Name = "GetCustomer")]
        public async Task<IActionResult> GetCustomer(Guid id, CancellationToken cancellationToken= default)
        {
            var result = _customersService.GetCustomer(id, cancellationToken);
            if (null == result) 
                return NotFound();
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken = default)
        {
            var results = await _customersService.GetCustomers(cancellationToken);
            if (null == results)
                return NotFound();
            return Ok(results);
        }
    }
}